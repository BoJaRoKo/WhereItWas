using System.Text;

namespace WhereItWas.Core;

public static class Execute
{
    private static readonly IReadOnlyList<QueryInfo> QueryCatalog = Array.AsReadOnly<QueryInfo>(
    [
        new("fields", "Pola", "Zwraca pola znalezionych węzłów."),
        new("fields-with-path", "Pola ze ścieżką", "Zwraca pola znalezionych węzłów wraz z pełną ścieżką."),
        new("path-only", "Tylko ścieżka", "Zwraca tylko pełne ścieżki znalezionych węzłów."),
        new("count", "Liczba", "Zwraca liczbę znalezionych węzłów.")
    ]);

    public static bool SimpleVeryfication(string query)
    {
       throw new NotImplementedException("Metoda SimpleVeryfication nie została jeszcze zaimplementowana.");
    }

    public static string? VerifyQuery(string query)
    {
        throw new NotImplementedException("Metoda VerifyQuery nie została jeszcze zaimplementowana.");
    }

    public static void ExecuteQuery(QueryParameters parameters, SqlConnectionProfile currentConnection)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(currentConnection);
        throw new NotImplementedException("Metoda ExecuteQuery nie została jeszcze zaimplementowana.");
    }

    public static IReadOnlyList<QueryInfo> QueryNames() => QueryCatalog;

    public static IReadOnlyList<QueryDefinition> GetQueries(QueryParameters x)
    {
        ArgumentNullException.ThrowIfNull(x);

        return
        [
            MakeQueryDefinition("fields", SqlSelect(x)),
            MakeQueryDefinition("fields-with-path", SqlSelectWithPath(x)),
            MakeQueryDefinition("path-only", SqlSelectPathOnly(x)),
            MakeQueryDefinition("count", SqlSelectCount(x))
        ];
    }

    private static QueryDefinition MakeQueryDefinition(string id, string sql)
    {
        var info = QueryCatalog.First(q => q.Id == id);
        return new QueryDefinition(info.Id, sql, info.Name, info.Description);
    }

    public static string SqlSelect(QueryParameters x, uint limit = 0, string orderBy = "")
    {
        ValidateQueryParameters(x);

        var sql = new StringBuilder();

        if (x.DisplyedNodeNr == 0)
            sql.AppendLine("SELECT n0.*");
        else
            sql.AppendLine($"SELECT DISTINCT n{x.DisplyedNodeNr}.*");

        sql.AppendLine("FROM dbo.TreeNodeSearch AS n0");

        for (int i = 1; i < x.Elements.Length; i++)
        {
            sql.AppendLine(
                $"JOIN dbo.TreeNodeSearch AS n{i} " +
                $"ON n{i}.Id = n{i - 1}.ParentId");
        }

        var conditions = BuildConditions(x);
        if (conditions.Count > 0)
        {
            sql.AppendLine("WHERE");
            sql.Append("    ");
            sql.Append(string.Join(Environment.NewLine + "  AND ", conditions));
        }

        return sql.ToString();
    }

    private static void ValidateQueryParameters(QueryParameters x)
    {
        ArgumentNullException.ThrowIfNull(x);

        if (!x.OK)
            throw new ArgumentException($"Invalid query parameters: {x.ErrorMessage}", nameof(x));

        if (x.Elements.Length == 0)
            throw new ArgumentException("Query contains no path elements.", nameof(x));

        for (int i = 0; i < x.Elements.Length - 1; i++)
        {
            if (x.Elements[i].IsRoot)
                throw new ArgumentException("Root is allowed only as the first path element.", nameof(x));
        }

        if (x.DisplyedNodeNr < 0 || x.DisplyedNodeNr >= x.Elements.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(x),
                "Displayed node number is outside the path.");
        }
    }

    private static List<string> BuildConditions(QueryParameters x)
    {
        var conditions = new List<string>();

        for (int i = 0; i < x.Elements.Length; i++)
            AddElementConditions(conditions, $"n{i}", x.Elements[i]);

        switch (x.NodeType)
        {
            case NodeType.File:
                conditions.Add("n0.NodeType = N'File'");
                break;
            case NodeType.Directory:
                conditions.Add("n0.NodeType = N'Directory'");
                break;
            case NodeType.Any:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(x), $"Unknown NodeType: {x.NodeType}");
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
            conditions.Add($"{alias}.LevelInTree = 0");
            return;
        }

        if (!element.IsSplit)
        {
            if (element.Name is null)
                throw new InvalidOperationException("Translated element has no name pattern.");

            AddPatternCondition(conditions, $"{alias}.Name", element.Name.Value);
            return;
        }

        if (element.NameWithoutExtension is null || element.Extension is null)
            throw new InvalidOperationException("Split translated element is incomplete.");

        AddPatternCondition(conditions, $"{alias}.Extension", element.Extension.Value);
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
                conditions.Add($"{column} = N'{EscapeSqlString(pattern.Value)}'");
                return;
            case PatternKind.Like:
                conditions.Add($"{column} LIKE N'{EscapeSqlString(pattern.Value)}'");
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(pattern), $"Unknown PatternKind: {pattern.Kind}");
        }
    }

    public static string SqlSelectPathOnly(QueryParameters x)
    {
        ArgumentNullException.ThrowIfNull(x);

        var nodeSelect = SqlSelect(x);
        var sql = new StringBuilder();

        sql.AppendLine("WITH Found AS");
        sql.AppendLine("(");
        sql.AppendLine("    SELECT q.Id");
        sql.AppendLine("    FROM");
        sql.AppendLine("    (");
        AppendIndentedSql(sql, nodeSelect, 8);
        sql.AppendLine("    ) AS q");
        sql.AppendLine("),");
        AppendPathsCte(sql);
        sql.AppendLine("SELECT FullPath");
        sql.AppendLine("FROM Paths");
        sql.AppendLine("WHERE LevelInTree = 0");
        sql.AppendLine("    AND ParentId IS NOT NULL");

        return sql.ToString();
    }

    public static string SqlSelectWithPath(QueryParameters x)
    {
        ArgumentNullException.ThrowIfNull(x);

        var nodeSelect = SqlSelect(x);
        var sql = new StringBuilder();

        sql.AppendLine("WITH Found AS");
        sql.AppendLine("(");
        AppendIndentedSql(sql, nodeSelect, 4);
        sql.AppendLine("),");
        AppendPathsCte(sql);
        sql.AppendLine("SELECT");
        sql.AppendLine("    f.*,");
        sql.AppendLine("    p.FullPath");
        sql.AppendLine("FROM Found AS f");
        sql.AppendLine("JOIN Paths AS p ON p.ResultId = f.Id");
        sql.AppendLine("WHERE p.LevelInTree = 0");
        sql.AppendLine("  AND p.ParentId IS NOT NULL");

        return sql.ToString();
    }

    public static string SqlSelectCount(QueryParameters x)
    {
        ArgumentNullException.ThrowIfNull(x);

        var nodeSelect = SqlSelect(x);
        var sql = new StringBuilder();

        sql.AppendLine("SELECT COUNT(*)");
        sql.AppendLine("FROM");
        sql.AppendLine("(");
        AppendIndentedSql(sql, nodeSelect, 4);
        sql.AppendLine(") AS q");

        return sql.ToString();
    }

    private static void AppendPathsCte(StringBuilder sql)
    {
        sql.AppendLine("Paths AS");
        sql.AppendLine("(");
        sql.AppendLine("    SELECT");
        sql.AppendLine("        f.Id AS ResultId,");
        sql.AppendLine("        n.Id,");
        sql.AppendLine("        n.ParentId,");
        sql.AppendLine("        n.LevelInTree,");
        sql.AppendLine("        CAST(n.Name AS nvarchar(max)) AS FullPath");
        sql.AppendLine("    FROM Found AS f");
        sql.AppendLine("    JOIN dbo.TreeNodeSearch AS n ON n.Id = f.Id");
        sql.AppendLine();
        sql.AppendLine("    UNION ALL");
        sql.AppendLine();
        sql.AppendLine("    SELECT");
        sql.AppendLine("        p.ResultId,");
        sql.AppendLine("        n.Id,");
        sql.AppendLine("        n.ParentId,");
        sql.AppendLine("        n.LevelInTree,");
        sql.AppendLine(
            @"        CAST(n.Name +
            CASE
                WHEN RIGHT(n.Name, 1) = N'\' THEN N''
                ELSE N'\'
            END +
            p.FullPath AS nvarchar(max)) AS FullPath");
        sql.AppendLine("    FROM Paths AS p");
        sql.AppendLine("    JOIN dbo.TreeNodeSearch AS n ON n.Id = p.ParentId");
        sql.AppendLine(")");
    }

    private static void AppendIndentedSql(StringBuilder target, string sql, int spaces)
    {
        var indentation = new string(' ', spaces);
        foreach (var line in sql.Split('\n'))
            target.Append(indentation).AppendLine(line.TrimEnd('\r'));
    }

    private static string EscapeSqlString(string value)
    {
        return value.Replace("'", "''");
    }
}
