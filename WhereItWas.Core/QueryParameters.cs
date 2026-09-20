using System;
using System.Text;

namespace WhereItWas.Core
{
    public sealed class QueryParameters
    {
        public QueryParameters(string[] parameters)
        {
            if (parameters is null || parameters.Length == 0)
            {
                OK = false;

                ErrorMessage = "Query cannot be null or empty.";
                return;
            } 
            OK = true; // Kolejne sprawdzenia i przetwarzanie parametrów mogą to zmienić na false, jeśli wystąpią błędy.
            var s0 = parameters[0];
            LikeStrings = MakeLieStrings(s0);
            NodeType = MakeNodeType(s0);
            DisplyedNodeNr = GetNodeNr(s0);

        }

        private int GetNodeNr(string s0)
        {
            var parts = s0.Split('/');

            int colonPart = -1;

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].StartsWith(':'))
                {
                    if (colonPart >= 0)
                    {
                        OK = false;
                        ErrorMessage += "Too many colons  ";
                        return -1;
                    }

                    colonPart = i;
                }
            }

            // Brak ':' -> ostatni element, czyli n0.
            if (colonPart < 0)
                return 0;

            // n0 jest ostatnim elementem ścieżki.
            return parts.Length - 1 - colonPart;
        }
        private NodeType MakeNodeType(string s0)
        {
            var s = s0.Split('|');
            if (s.Length > 2)
            {
                OK = false;
                ErrorMessage += "Too many pipes  ";
                return NodeType.Any;
            }
            else if (s.Length == 2)
            {
                var s1 = s[1].ToLower();
                switch (s1)
                {
                    case "f":
                    case "F":
                        return NodeType.File;
                    case "d":
                    case "D":
                        return NodeType.Directory;
                    case "a":
                    case "A":
                        return NodeType.Any;
                    default:
                        OK = false;
                        ErrorMessage += $"Unknown node type: {s1}  ";
                        return NodeType.Any;
                }
            }
            else
            {
                return NodeType.Any;
            }
        }

        private string[] MakeLieStrings(string s0)
        {
            var outStack = new Stack<string>();
            var s = s0.Split('|');
            var s1 = s[0].Replace(":","").Split('/');            
            foreach (var part in s1)
            {
                outStack.Push(Translate(part));
            }
            return outStack.ToArray();
        }

        private static string Translate(string element)
        {
            ArgumentNullException.ThrowIfNull(element);

            // Pusty element pozostaje pusty.
            // W kontekście ścieżki może oznaczać root.
            if (element.Length == 0)
                return string.Empty;

            bool anchorStart = false;
            bool anchorEnd = false;

            // Kotwica początku
            if (element.StartsWith('^'))
            {
                anchorStart = true;
                element = element[1..];
            }

            // Kotwica końca.
            // Końcowe \^ jest literalnym ^, więc nie jest kotwicą.
            if (element.EndsWith('^') && !element.EndsWith(@"\^"))
            {
                anchorEnd = true;
                element = element[..^1];
            }

            var result = new StringBuilder();

            for (int i = 0; i < element.Length; i++)
            {
                char c = element[i];

                switch (c)
                {
                    case '\\':
                        if (i + 1 < element.Length && element[i + 1] == '^')
                        {
                            result.Append('^');
                            i++;
                        }
                        else
                        {
                            // Na razie '\' nie ma innej funkcji.
                            result.Append('\\');
                        }
                        break;

                    case '^':
                        throw new ArgumentException(
                            "Unescaped '^' is allowed only as an anchor " +
                            "at the beginning or end of an element.",
                            nameof(element));

                    case '%':
                        result.Append("[%]");
                        break;

                    case '_':
                        result.Append("[_]");
                        break;

                    case '*':
                        result.Append('%');
                        break;

                    case '?':
                        result.Append('_');
                        break;

                    default:
                        result.Append(c);
                        break;
                }
            }

            if (!anchorStart && (result.Length == 0 || result[0] != '%'))
                result.Insert(0, '%');

            if (!anchorEnd && (result.Length == 0 || result[^1] != '%'))
                result.Append('%');

            return result.ToString();
        }


        public string ErrorMessage { get; private set; } = string.Empty;
        public bool OK { get; private set; } = false;
        public string[] LikeStrings { get; private set; } = Array.Empty<string>();
        public NodeType NodeType { get; private set; } = NodeType.Any;
        public int DisplyedNodeNr { get; private set; } = 0; // in reverse order, 0 - last, 1 - second last, 2 - third last, etc.
    }

    public enum NodeType
    {
        Any,
        File,
        Directory
    }
}
