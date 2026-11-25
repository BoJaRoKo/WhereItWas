using System.Text;

namespace WhereItWas.Core;

public static class Execute
{
    private static readonly IReadOnlyList<QueryInfo> QueryCatalog =
        Array.AsReadOnly(
            new QueryInfo[]
            {
                new(
                    "fields",
                    "Pola",
                    "Zwraca pola znalezionych węzłów."),

                new(
                    "fields-with-path",
                    "Pola ze ścieżką",
                    "Zwraca pola znalezionych węzłów wraz z pełną ścieżką."),

                new(
                    "path-only",
                    "Tylko ścieżka",
                    "Zwraca tylko pełne ścieżki znalezionych węzłów."),

                new(
                    "count",
                    "Liczba",
                    "Zwraca liczbę znalezionych węzłów.")
            });

    // -----------------------------------------------------------------
    // Publiczne API
    // -----------------------------------------------------------------

    public static IReadOnlyList<QueryInfo> QueryNames()
    {
        return QueryCatalog;
    }

    public static IReadOnlyList<QueryDefinition> GetQueries(
        string query,
        uint limit = 0,
        string orderBy = "")
    {
        var parsedQuery = QueryParser.Parse(query);

        return
        [
            MakeQueryDefinition(
                "fields",
                SqlSelect(parsedQuery, limit, orderBy)),

            MakeQueryDefinition(
                "fields-with-path",
                SqlSelectWithPath(parsedQuery, limit, orderBy)),

            MakeQueryDefinition(
                "path-only",
                SqlSelectPathOnly(parsedQuery, limit, orderBy)),

            // Count zawsze dotyczy pełnego wyniku.
            MakeQueryDefinition(
                "count",
                SqlSelectCount(parsedQuery))
        ];
    }

    private static QueryDefinition MakeQueryDefinition(
        string id,
        string sql)
    {
        var info = QueryCatalog.First(q => q.Id == id);

        return new QueryDefinition(
            info.Id,
            sql,
            info.Name,
            info.Description);
    }

    // -----------------------------------------------------------------
    // Generowanie SQL
    // -----------------------------------------------------------------

    private static string SqlSelect(
        ParsedQuery query,
        uint limit = 0,
        string orderBy = "")
    {
        ValidateQuery(query);

        var sql = new StringBuilder();

        if (query.DisplayedNodeNr == 0)
        {
            if (limit == 0)
                sql.AppendLine("SELECT n0.*");
            else
                sql.AppendLine($"SELECT TOP ({limit}) n0.*");
        }
        else
        {
            if (limit == 0)
            {
                sql.AppendLine(
                    $"SELECT DISTINCT n{query.DisplayedNodeNr}.*");
            }
            else
            {
                sql.AppendLine(
                    $"SELECT DISTINCT TOP ({limit}) " +
                    $"n{query.DisplayedNodeNr}.*");
            }
        }

        sql.AppendLine("FROM dbo.TreeNodeSearch AS n0");

        for (int i = 1; i < query.Elements.Length; i++)
        {
            sql.AppendLine(
                $"JOIN dbo.TreeNodeSearch AS n{i} " +
                $"ON n{i}.Id = n{i - 1}.ParentId");
        }

        var conditions = BuildConditions(query);

        if (conditions.Count > 0)
        {
            sql.AppendLine("WHERE");
            sql.Append("    ");
            sql.Append(
                string.Join(
                    Environment.NewLine + "  AND ",
                    conditions));
        }

        /*
         * orderBy jest już częścią publicznego API,
         * ale na razie celowo nie jest generowany.
         *
         * Trzeba najpierw ustalić jego składnię i miejsce:
         * SqlSelect jest również używany jako podzapytanie/CTE,
         * więc bezmyślne dopisanie ORDER BY może wygenerować
         * niepoprawny SQL.
         */
        _ = orderBy;

        return sql.ToString();
    }

    private static string SqlSelectPathOnly(
        ParsedQuery query,
        uint limit = 0,
        string orderBy = "")
    {
        var nodeSelect = SqlSelect(
            query,
            limit,
            orderBy);

        var sql = new StringBuilder();

        sql.AppendLine("WITH Found AS");
        sql.AppendLine("(");
        sql.AppendLine("    SELECT q.Id");
        sql.AppendLine("    FROM");
        sql.AppendLine("    (");

        AppendIndentedSql(
            sql,
            nodeSelect,
            8);

        sql.AppendLine("    ) AS q");
        sql.AppendLine("),");

        AppendPathsCte(sql);

        sql.AppendLine("SELECT FullPath");
        sql.AppendLine("FROM Paths");
        sql.AppendLine("WHERE LevelInTree = 0");
        sql.AppendLine("  AND ParentId IS NOT NULL");

        return sql.ToString();
    }

    private static string SqlSelectWithPath(
        ParsedQuery query,
        uint limit = 0,
        string orderBy = "")
    {
        var nodeSelect = SqlSelect(
            query,
            limit,
            orderBy);

        var sql = new StringBuilder();

        sql.AppendLine("WITH Found AS");
        sql.AppendLine("(");

        AppendIndentedSql(
            sql,
            nodeSelect,
            4);

        sql.AppendLine("),");

        AppendPathsCte(sql);

        sql.AppendLine("SELECT");
        sql.AppendLine("    f.*,");
        sql.AppendLine("    p.FullPath");
        sql.AppendLine("FROM Found AS f");
        sql.AppendLine(
            "JOIN Paths AS p ON p.ResultId = f.Id");
        sql.AppendLine("WHERE p.LevelInTree = 0");
        sql.AppendLine("  AND p.ParentId IS NOT NULL");

        return sql.ToString();
    }

    private static string SqlSelectCount(
        ParsedQuery query)
    {
        // Bez limitu i bez ORDER BY.
        var nodeSelect = SqlSelect(
            query,
            0,
            "");

        var sql = new StringBuilder();

        sql.AppendLine("SELECT COUNT(*)");
        sql.AppendLine("FROM");
        sql.AppendLine("(");

        AppendIndentedSql(
            sql,
            nodeSelect,
            4);

        sql.AppendLine(") AS q");

        return sql.ToString();
    }

    // -----------------------------------------------------------------
    // Warunki WHERE
    // -----------------------------------------------------------------

    private static List<string> BuildConditions(
        ParsedQuery query)
    {
        var conditions = new List<string>();

        for (int i = 0; i < query.Elements.Length; i++)
        {
            AddElementConditions(
                conditions,
                $"n{i}",
                query.Elements[i]);
        }

        switch (query.NodeType)
        {
            case ParsedNodeType.File:
                conditions.Add(
                    "n0.NodeType = N'File'");
                break;

            case ParsedNodeType.Directory:
                conditions.Add(
                    "n0.NodeType = N'Directory'");
                break;

            case ParsedNodeType.Any:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        return conditions;
    }

    private static void AddElementConditions(
        ICollection<string> conditions,
        string alias,
        TranslatedElement element)
    {
        if (element.IsRoot)
        {
            conditions.Add(
                $"{alias}.LevelInTree = 0");

            conditions.Add(
                $"{alias}.ParentId IS NOT NULL");

            return;
        }

        if (!element.IsSplit)
        {
            if (element.Name is null)
            {
                throw new InvalidOperationException(
                    "Translated element has no name pattern.");
            }

            AddPatternCondition(
                conditions,
                $"{alias}.Name",
                element.Name.Value);

            return;
        }

        if (element.NameWithoutExtension is null ||
            element.Extension is null)
        {
            throw new InvalidOperationException(
                "Split translated element is incomplete.");
        }

        // Extension najpierw — odpowiada to również
        // indeksowi zaczynającemu się od Extension.
        AddPatternCondition(
            conditions,
            $"{alias}.Extension",
            element.Extension.Value);

        AddPatternCondition(
            conditions,
            $"{alias}.NameWithoutExtension",
            element.NameWithoutExtension.Value);
    }

    private static void AddPatternCondition(
        ICollection<string> conditions,
        string column,
        SqlPattern pattern)
    {
        switch (pattern.Kind)
        {
            case PatternKind.Any:
                return;

            case PatternKind.Exact:
                conditions.Add(
                    $"{column} = N'{EscapeSqlString(pattern.Value)}'");
                return;

            case PatternKind.Like:
                conditions.Add(
                    $"{column} LIKE N'{EscapeSqlString(pattern.Value)}'");
                return;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(pattern));
        }
    }

    // -----------------------------------------------------------------
    // Walidacja wyniku parsera
    // -----------------------------------------------------------------

    private static void ValidateQuery(
        ParsedQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.Elements.Length == 0)
        {
            throw new ArgumentException(
                "Query contains no path elements.",
                nameof(query));
        }

        /*
         * Elements są odwrócone:
         *
         * n0 = ostatni element ścieżki,
         * n1 = jego rodzic,
         * ...
         *
         * Root może więc wystąpić wyłącznie jako ostatni
         * element tablicy.
         */
        for (int i = 0;
             i < query.Elements.Length - 1;
             i++)
        {
            if (query.Elements[i].IsRoot)
            {
                throw new ArgumentException(
                    "Empty path element is allowed only " +
                    "as the leading root marker.",
                    nameof(query));
            }
        }

        if (query.DisplayedNodeNr < 0 ||
            query.DisplayedNodeNr >= query.Elements.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(query),
                "Displayed node number is outside the path.");
        }
    }

    // -----------------------------------------------------------------
    // CTE budujące pełną ścieżkę
    // -----------------------------------------------------------------

    private static void AppendPathsCte(
        StringBuilder sql)
    {
        sql.AppendLine("Paths AS");
        sql.AppendLine("(");

        sql.AppendLine("    SELECT");
        sql.AppendLine("        f.Id AS ResultId,");
        sql.AppendLine("        n.Id,");
        sql.AppendLine("        n.ParentId,");
        sql.AppendLine("        n.LevelInTree,");
        sql.AppendLine(
            "        CAST(n.Name AS nvarchar(max)) AS FullPath");

        sql.AppendLine("    FROM Found AS f");
        sql.AppendLine(
            "    JOIN dbo.TreeNodeSearch AS n " +
            "ON n.Id = f.Id");

        sql.AppendLine();
        sql.AppendLine("    UNION ALL");
        sql.AppendLine();

        sql.AppendLine("    SELECT");
        sql.AppendLine("        p.ResultId,");
        sql.AppendLine("        n.Id,");
        sql.AppendLine("        n.ParentId,");
        sql.AppendLine("        n.LevelInTree,");

        sql.AppendLine(
            @"        CAST(
            n.Name +
            CASE
                WHEN RIGHT(n.Name, 1) = N'\' THEN N''
                ELSE N'\'
            END +
            p.FullPath
            AS nvarchar(max)
        ) AS FullPath");

        sql.AppendLine("    FROM Paths AS p");

        sql.AppendLine(
            "    JOIN dbo.TreeNodeSearch AS n " +
            "ON n.Id = p.ParentId");

        sql.AppendLine(")");
    }

    private static void AppendIndentedSql(
        StringBuilder target,
        string sql,
        int spaces)
    {
        var indentation =
            new string(' ', spaces);

        foreach (var line in sql.Split('\n'))
        {
            target
                .Append(indentation)
                .AppendLine(line.TrimEnd('\r'));
        }
    }

    private static string EscapeSqlString(
        string value)
    {
        return value.Replace("'", "''");
    }

    // =================================================================
    // Parser języka zapytań
    // =================================================================

    private static class QueryParser
    {
        public static ParsedQuery Parse(
            string query)
        {
            ArgumentNullException.ThrowIfNull(query);

            if (query.Length == 0)
            {
                throw new ArgumentException(
                    "Query cannot be empty.",
                    nameof(query));
            }

            var (queryText, nodeType) =
                SplitNodeType(query);

            var parts =
                SplitPath(queryText);

            int displayedNodeNr =
                GetDisplayedNodeNr(parts);

            var elements =
                TranslateElements(parts);

            return new ParsedQuery(
                elements,
                nodeType,
                displayedNodeNr);
        }

        // -------------------------------------------------------------
        // |f |d |a
        // -------------------------------------------------------------

        private static (
            string QueryText,
            ParsedNodeType NodeType)
            SplitNodeType(string input)
        {
            var pipes =
                FindUnescapedDelimiters(
                    input,
                    '|');

            if (pipes.Count > 1)
            {
                throw new ArgumentException(
                    "Too many pipes.",
                    nameof(input));
            }

            if (pipes.Count == 0)
            {
                return (
                    input,
                    ParsedNodeType.Any);
            }

            int pipe = pipes[0];

            var queryText =
                input[..pipe];

            var typeText =
                input[(pipe + 1)..];

            var nodeType =
                typeText.ToLowerInvariant() switch
                {
                    "f" => ParsedNodeType.File,
                    "d" => ParsedNodeType.Directory,
                    "a" => ParsedNodeType.Any,

                    _ => throw new ArgumentException(
                        $"Unknown node type: {typeText}",
                        nameof(input))
                };

            return (
                queryText,
                nodeType);
        }

        // -------------------------------------------------------------
        // Ścieżka /
        // -------------------------------------------------------------

        private static List<string> SplitPath(
            string query)
        {
            /*
             * Sam "/" oznacza wszystkie właściwe rooty.
             *
             * Normalne Split dałoby dwa puste elementy,
             * a semantycznie potrzebujemy jednego Root.
             */
            if (query == "/")
            {
                return
                [
                    string.Empty
                ];
            }

            var separators =
                FindUnescapedDelimiters(
                    query,
                    '/');

            var result =
                new List<string>(
                    separators.Count + 1);

            int start = 0;

            foreach (int separator in separators)
            {
                result.Add(
                    query[start..separator]);

                start = separator + 1;
            }

            result.Add(
                query[start..]);

            return result;
        }

        // -------------------------------------------------------------
        // :
        // -------------------------------------------------------------

        private static int GetDisplayedNodeNr(
            IReadOnlyList<string> parts)
        {
            int colonPart = -1;

            for (int i = 0;
                 i < parts.Count;
                 i++)
            {
                if (!parts[i].StartsWith(':'))
                    continue;

                if (colonPart >= 0)
                {
                    throw new ArgumentException(
                        "Too many projection markers ':'.");
                }

                if (parts[i].Length == 1)
                {
                    throw new ArgumentException(
                        "Projection marker ':' must precede a path element.");
                }

                colonPart = i;
            }

            // Bez ':' zwracamy ostatni element ścieżki = n0.
            if (colonPart < 0)
                return 0;

            return parts.Count - 1 - colonPart;
        }

        // -------------------------------------------------------------
        // Elementy ścieżki
        // -------------------------------------------------------------

        private static TranslatedElement[] TranslateElements(
            IReadOnlyList<string> parts)
        {
            var result =
                new Stack<TranslatedElement>();

            foreach (var rawPart in parts)
            {
                string part = rawPart;

                if (part.StartsWith(':'))
                {
                    part = part[1..];
                }

                result.Push(
                    TranslateElement(part));
            }

            return result.ToArray();
        }

        private static TranslatedElement TranslateElement(
            string element)
        {
            if (element.Length == 0)
                return TranslatedElement.Root;

            int dot =
                FindLastUnescapedDot(element);

            if (dot < 0)
            {
                return new TranslatedElement(
                    TranslatePattern(element),
                    null,
                    null,
                    false);
            }

            string name =
                element[..dot];

            string extension =
                element[(dot + 1)..];

            return new TranslatedElement(
                null,
                TranslateSplitPart(name),
                TranslateSplitPart(extension),
                false);
        }

        private static SqlPattern TranslateSplitPart(
            string pattern)
        {
            if (pattern.Length == 0)
            {
                return new SqlPattern(
                    PatternKind.Exact,
                    string.Empty);
            }

            return TranslatePattern(pattern);
        }

        // -------------------------------------------------------------
        // .
        // -------------------------------------------------------------

        private static int FindLastUnescapedDot(
            string element)
        {
            var dots =
                FindUnescapedDelimiters(
                    element,
                    '.');

            return dots.Count == 0
                ? -1
                : dots[^1];
        }

        // -------------------------------------------------------------
        // Wzorzec
        //
        // abc     => Exact("abc")
        // abc*    => Like("abc%")
        // *abc    => Like("%abc")
        // *abc*   => Like("%abc%")
        // *       => Any
        // -------------------------------------------------------------

        private static SqlPattern TranslatePattern(
            string pattern)
        {
            var exact =
                new StringBuilder();

            var like =
                new StringBuilder();

            bool requiresLike = false;
            bool onlyStars = pattern.Length > 0;

            for (int i = 0;
                 i < pattern.Length;
                 i++)
            {
                char c = pattern[i];

                if (c == '\\')
                {
                    onlyStars = false;

                    if (++i >= pattern.Length)
                    {
                        throw new ArgumentException(
                            "Escape character cannot terminate a pattern.",
                            nameof(pattern));
                    }

                    char literal = pattern[i];

                    exact.Append(literal);
                    AppendLiteralForLike(
                        like,
                        literal);

                    continue;
                }

                switch (c)
                {
                    case ':':
                        onlyStars = false;

                        throw new ArgumentException(
                            "Unescaped ':' is allowed only " +
                            "at the beginning of a path element.",
                            nameof(pattern));

                    case '*':
                        like.Append('%');
                        requiresLike = true;
                        break;

                    case '?':
                        onlyStars = false;

                        like.Append('_');
                        requiresLike = true;
                        break;

                    case '<':
                        onlyStars = false;

                        i = TranslateCharacterClass(
                            pattern,
                            i,
                            like);

                        requiresLike = true;
                        break;

                    case '>':
                        onlyStars = false;

                        throw new ArgumentException(
                            "Unmatched '>' in pattern.",
                            nameof(pattern));

                    default:
                        onlyStars = false;

                        exact.Append(c);

                        AppendLiteralForLike(
                            like,
                            c);

                        break;
                }
            }

            if (onlyStars)
            {
                return new SqlPattern(
                    PatternKind.Any,
                    string.Empty);
            }

            if (requiresLike)
            {
                return new SqlPattern(
                    PatternKind.Like,
                    like.ToString());
            }

            return new SqlPattern(
                PatternKind.Exact,
                exact.ToString());
        }

        // -------------------------------------------------------------
        // <...>
        // -------------------------------------------------------------

        private static int TranslateCharacterClass(
            string pattern,
            int start,
            StringBuilder result)
        {
            int end =
                FindClassEnd(
                    pattern,
                    start + 1);

            if (end < 0)
            {
                throw new ArgumentException(
                    "Unclosed '<...>' character class.",
                    nameof(pattern));
            }

            if (end == start + 1)
            {
                throw new ArgumentException(
                    "Character class cannot be empty.",
                    nameof(pattern));
            }

            result.Append('[');

            for (int i = start + 1;
                 i < end;
                 i++)
            {
                char c = pattern[i];

                if (c == '\\')
                {
                    if (++i >= end)
                    {
                        throw new ArgumentException(
                            "Escape character cannot terminate " +
                            "a character class.",
                            nameof(pattern));
                    }

                    result.Append(pattern[i]);
                    continue;
                }

                if (c == '<')
                {
                    throw new ArgumentException(
                        "Nested character classes are not allowed.",
                        nameof(pattern));
                }

                result.Append(c);
            }

            result.Append(']');

            return end;
        }

        private static int FindClassEnd(
            string pattern,
            int start)
        {
            bool escaped = false;

            for (int i = start;
                 i < pattern.Length;
                 i++)
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

        // -------------------------------------------------------------
        // Szukanie separatorów z uwzględnieniem \ i <...>
        // -------------------------------------------------------------

        private static List<int> FindUnescapedDelimiters(
            string text,
            char delimiter)
        {
            var result =
                new List<int>();

            bool escaped = false;
            bool inClass = false;

            for (int i = 0;
                 i < text.Length;
                 i++)
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
                    {
                        throw new ArgumentException(
                            "Nested character classes are not allowed.",
                            nameof(text));
                    }

                    inClass = true;
                    continue;
                }

                if (c == '>')
                {
                    if (!inClass)
                    {
                        throw new ArgumentException(
                            "Unmatched '>' in pattern.",
                            nameof(text));
                    }

                    inClass = false;
                    continue;
                }

                if (c == delimiter &&
                    !inClass)
                {
                    result.Add(i);
                }
            }

            if (escaped)
            {
                throw new ArgumentException(
                    "Escape character cannot terminate a pattern.",
                    nameof(text));
            }

            if (inClass)
            {
                throw new ArgumentException(
                    "Unclosed '<...>' character class.",
                    nameof(text));
            }

            return result;
        }

        // -------------------------------------------------------------
        // Literały wewnątrz SQL LIKE
        // -------------------------------------------------------------

        private static void AppendLiteralForLike(
            StringBuilder result,
            char c)
        {
            switch (c)
            {
                case '%':
                    result.Append("[%]");
                    break;

                case '_':
                    result.Append("[_]");
                    break;

                case '[':
                    result.Append("[[]");
                    break;

                default:
                    result.Append(c);
                    break;
            }
        }
    }

    // =================================================================
    // Prywatny model parsera
    // =================================================================

    private sealed record ParsedQuery(
        TranslatedElement[] Elements,
        ParsedNodeType NodeType,
        int DisplayedNodeNr);

    private enum ParsedNodeType
    {
        Any,
        File,
        Directory
    }

    private enum PatternKind
    {
        Any,
        Exact,
        Like
    }

    private readonly record struct SqlPattern(
        PatternKind Kind,
        string Value);

    private readonly record struct TranslatedElement(
        SqlPattern? Name,
        SqlPattern? NameWithoutExtension,
        SqlPattern? Extension,
        bool IsRoot)
    {
        public static TranslatedElement Root =>
            new(
                null,
                null,
                null,
                true);

        public bool IsSplit =>
            !IsRoot &&
            Name is null;
    }
}