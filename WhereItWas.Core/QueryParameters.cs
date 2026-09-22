using System.Text;

namespace WhereItWas.Core;

public sealed class QueryParameters
{
    public QueryParameters(string[] parameters)
    {
        if (parameters is null || parameters.Length == 0)
        {
            ErrorMessage = "Query cannot be null or empty.";
            return;
        }

        try
        {
            var (queryText, nodeType) = SplitNodeType(parameters[0]);
            NodeType = nodeType;
            Elements = MakeElements(queryText);
            DisplyedNodeNr = GetNodeNr(queryText);
            OK = true;
        }
        catch (ArgumentException exception)
        {
            ErrorMessage = exception.Message;
        }
    }

    private static (string QueryText, NodeType NodeType) SplitNodeType(string input)
    {
        var pipes = FindUnescapedDelimiters(input, '|');
        if (pipes.Count > 1)
            throw new ArgumentException("Too many pipes.", nameof(input));

        if (pipes.Count == 0)
            return (input, NodeType.Any);

        int pipe = pipes[0];
        var queryText = input[..pipe];
        var typeText = input[(pipe + 1)..];

        var nodeType = typeText.ToLowerInvariant() switch
        {
            "f" => NodeType.File,
            "d" => NodeType.Directory,
            "a" => NodeType.Any,
            _ => throw new ArgumentException($"Unknown node type: {typeText}", nameof(input))
        };

        return (queryText, nodeType);
    }

    private static int GetNodeNr(string queryText)
    {
        var parts = SplitPath(queryText);
        int colonPart = -1;

        for (int i = 0; i < parts.Count; i++)
        {
            if (!parts[i].StartsWith(':'))
                continue;

            if (colonPart >= 0)
                throw new ArgumentException("Too many colons.", nameof(queryText));

            colonPart = i;
        }

        return colonPart < 0 ? 0 : parts.Count - 1 - colonPart;
    }

    private static TranslatedElement[] MakeElements(string queryText)
    {
        var result = new Stack<TranslatedElement>();

        foreach (var rawPart in SplitPath(queryText))
        {
            var part = rawPart.StartsWith(':') ? rawPart[1..] : rawPart;
            result.Push(TranslateElement(part));
        }

        return result.ToArray();
    }

    private static List<string> SplitPath(string queryText)
    {
        var separators = FindUnescapedDelimiters(queryText, '/');
        var result = new List<string>(separators.Count + 1);
        int start = 0;

        foreach (int separator in separators)
        {
            result.Add(queryText[start..separator]);
            start = separator + 1;
        }

        result.Add(queryText[start..]);
        return result;
    }

    private static List<int> FindUnescapedDelimiters(string text, char delimiter)
    {
        var result = new List<int>();
        bool escaped = false;
        bool inClass = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (escaped)
            {
                escaped = false;
                continue;
            }

            if (c == '\\')
            {
                escaped = true;
                continue;
            }

            if (c == '<')
            {
                if (inClass)
                    throw new ArgumentException("Nested character classes are not allowed.", nameof(text));
                inClass = true;
                continue;
            }

            if (c == '>')
            {
                if (!inClass)
                    throw new ArgumentException("Unmatched '>' in pattern.", nameof(text));
                inClass = false;
                continue;
            }

            if (c == delimiter && !inClass)
                result.Add(i);
        }

        if (escaped)
            throw new ArgumentException("Escape character cannot terminate a pattern.", nameof(text));
        if (inClass)
            throw new ArgumentException("Unclosed '<...>' character class.", nameof(text));

        return result;
    }

    private static TranslatedElement TranslateElement(string element)
    {
        if (element.Length == 0)
            return TranslatedElement.Root;

        int dot = FindLastUnescapedDot(element);
        if (dot < 0)
        {
            return new(
                TranslatePattern(element, false, false),
                null,
                null,
                false);
        }

        string name = element[..dot];
        string extension = element[(dot + 1)..];

        return new(
            null,
            TranslateSplitPart(name, false, true),
            TranslateSplitPart(extension, true, false),
            false);
    }

    private static int FindLastUnescapedDot(string element)
    {
        var dots = FindUnescapedDelimiters(element, '.');
        return dots.Count == 0 ? -1 : dots[^1];
    }

    private static SqlPattern TranslateSplitPart(
        string pattern,
        bool forceAnchorStart,
        bool forceAnchorEnd)
    {
        if (pattern.Length == 0)
            return new(PatternKind.Exact, string.Empty);

        return TranslatePattern(pattern, forceAnchorStart, forceAnchorEnd);
    }

    private static SqlPattern TranslatePattern(
        string pattern,
        bool forceAnchorStart,
        bool forceAnchorEnd)
    {
        bool anchorStart = forceAnchorStart;
        bool anchorEnd = forceAnchorEnd;

        if (pattern.StartsWith('^'))
        {
            anchorStart = true;
            pattern = pattern[1..];
        }

        if (pattern.Length > 0 && pattern[^1] == '^' && !IsEscaped(pattern, pattern.Length - 1))
        {
            anchorEnd = true;
            pattern = pattern[..^1];
        }

        var result = new StringBuilder();
        bool requiresLike = false;

        for (int i = 0; i < pattern.Length; i++)
        {
            char c = pattern[i];

            if (c == '\\')
            {
                if (++i >= pattern.Length)
                    throw new ArgumentException("Escape character cannot terminate a pattern.", nameof(pattern));

                AppendLiteralForLike(result, pattern[i]);
                continue;
            }

            switch (c)
            {
                case '^':
                    throw new ArgumentException(
                        "Unescaped '^' is allowed only as an anchor at the beginning or end of a pattern.",
                        nameof(pattern));

                case ':':
                    throw new ArgumentException(
                        "Unescaped ':' is allowed only at the beginning of a path element.",
                        nameof(pattern));

                case '*':
                    result.Append('%');
                    requiresLike = true;
                    break;

                case '?':
                    result.Append('_');
                    requiresLike = true;
                    break;

                case '<':
                    i = TranslateCharacterClass(pattern, i, result);
                    requiresLike = true;
                    break;

                case '>':
                    throw new ArgumentException("Unmatched '>' in pattern.", nameof(pattern));

                default:
                    AppendLiteralForLike(result, c);
                    break;
            }
        }

        if (!anchorStart)
        {
            result.Insert(0, '%');
            requiresLike = true;
        }

        if (!anchorEnd)
        {
            result.Append('%');
            requiresLike = true;
        }

        if (IsOnlyPercent(result))
            return new(PatternKind.Any, string.Empty);

        return requiresLike
            ? new(PatternKind.Like, result.ToString())
            : new(PatternKind.Exact, result.ToString());
    }

    private static int TranslateCharacterClass(string pattern, int start, StringBuilder result)
    {
        int end = FindClassEnd(pattern, start + 1);
        if (end < 0)
            throw new ArgumentException("Unclosed '<...>' character class.", nameof(pattern));
        if (end == start + 1)
            throw new ArgumentException("Character class cannot be empty.", nameof(pattern));

        result.Append('[');

        for (int i = start + 1; i < end; i++)
        {
            char c = pattern[i];

            if (c == '\\')
            {
                if (++i >= end)
                    throw new ArgumentException("Escape character cannot terminate a character class.", nameof(pattern));
                result.Append(pattern[i]);
                continue;
            }

            if (c == '<')
                throw new ArgumentException("Nested character classes are not allowed.", nameof(pattern));

            result.Append(c);
        }

        result.Append(']');
        return end;
    }

    private static int FindClassEnd(string pattern, int start)
    {
        bool escaped = false;

        for (int i = start; i < pattern.Length; i++)
        {
            char c = pattern[i];
            if (escaped)
            {
                escaped = false;
                continue;
            }
            if (c == '\\')
            {
                escaped = true;
                continue;
            }
            if (c == '>')
                return i;
        }

        return -1;
    }

    private static bool IsEscaped(string text, int index)
    {
        int slashes = 0;
        for (int i = index - 1; i >= 0 && text[i] == '\\'; i--)
            slashes++;
        return (slashes & 1) != 0;
    }

    private static void AppendLiteralForLike(StringBuilder result, char c)
    {
        switch (c)
        {
            case '%': result.Append("[%]"); break;
            case '_': result.Append("[_]"); break;
            case '[': result.Append("[[]"); break;
            default: result.Append(c); break;
        }
    }

    private static bool IsOnlyPercent(StringBuilder value)
    {
        if (value.Length == 0)
            return false;

        for (int i = 0; i < value.Length; i++)
            if (value[i] != '%')
                return false;

        return true;
    }

    public string ErrorMessage { get; private set; } = string.Empty;
    public bool OK { get; private set; }
    public TranslatedElement[] Elements { get; private set; } = Array.Empty<TranslatedElement>();
    public NodeType NodeType { get; private set; } = NodeType.Any;
    public int DisplyedNodeNr { get; private set; }
}

public enum NodeType
{
    Any,
    File,
    Directory
}

public enum PatternKind
{
    Any,
    Exact,
    Like
}

public readonly record struct SqlPattern(PatternKind Kind, string Value);

public readonly record struct TranslatedElement(
    SqlPattern? Name,
    SqlPattern? NameWithoutExtension,
    SqlPattern? Extension,
    bool IsRoot)
{
    public static TranslatedElement Root => new(null, null, null, true);
    public bool IsSplit => !IsRoot && Name is null;
}
