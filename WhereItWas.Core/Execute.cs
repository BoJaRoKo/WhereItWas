using System.Text;

namespace WhereItWas.Core;

public static class Execute
{
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

    public static string SqlSelect(QueryParameters x, uint limit = 0, string orderBy = "")
    {
        ArgumentNullException.ThrowIfNull(x);
        // Pusty element jest dozwolony wyłącznie jako ostatni element LikeStrings.
        // Oznacza wtedy root (początkowy '/' w zapytaniu).
        for (int i = 0; i < x.LikeStrings.Length - 1; i++)
        {
            if (x.LikeStrings[i].Length == 0)
                throw new ArgumentException(
                    "Empty path element is allowed only for root.",
                    nameof(x));
        }

        if (!x.OK)
            throw new ArgumentException(
                $"Invalid query parameters: {x.ErrorMessage}",
                nameof(x));

        if (x.LikeStrings.Length == 0)
            throw new ArgumentException("Query contains no path elements.", nameof(x));

        if (x.DisplyedNodeNr < 0 || x.DisplyedNodeNr >= x.LikeStrings.Length)
            throw new ArgumentOutOfRangeException(
                nameof(x),
                "Displayed node number is outside the path.");

        var sql = new StringBuilder();

        // LikeStrings są w odwrotnej kolejności:
        // n0 = ostatni element ścieżki,
        // n1 = jego rodzic itd.

        if (x.DisplyedNodeNr == 0)
            sql.AppendLine("SELECT n0.*");
        else
            sql.AppendLine($"SELECT DISTINCT n{x.DisplyedNodeNr}.*");

        sql.AppendLine("FROM dbo.TreeNodeSearch AS n0");

        for (int i = 1; i < x.LikeStrings.Length; i++)
        {
            sql.AppendLine(
                $"JOIN dbo.TreeNodeSearch AS n{i} " +
                $"ON n{i}.Id = n{i - 1}.ParentId");
        }

        sql.AppendLine("WHERE");

        for (int i = 0; i < x.LikeStrings.Length; i++)
        {
            if (i > 0)
                sql.AppendLine("  AND");

            if (i == x.LikeStrings.Length - 1 &&
                x.LikeStrings[i].Length == 0)
            {
                sql.Append($"    n{i}.LevelInTree = 0");
            }
            else
            {
                sql.Append(
                    $"    n{i}.Name LIKE N'{EscapeSqlString(x.LikeStrings[i])}'");
            }
        }

        // NodeType dotyczy ostatniego elementu ścieżki, czyli n0.
        switch (x.NodeType)
        {
            case NodeType.File:
                sql.AppendLine();
                sql.Append("  AND n0.NodeType = N'File'");
                break;

            case NodeType.Directory:
                sql.AppendLine();
                sql.Append("  AND n0.NodeType = N'Directory'");
                break;

            case NodeType.Any:
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(x),
                    $"Unknown NodeType: {x.NodeType}");
        }

        // sql.AppendLine(";"); // zakomentowano aby można użyć jako elementu wewnątrz "WITH Found"

        return sql.ToString();
    }
    public static string SqlSelectWithPath(QueryParameters x)
    {
        ArgumentNullException.ThrowIfNull(x);

        var nodeSelect = SqlSelect(x);

        var sql = new StringBuilder();

        sql.AppendLine("WITH Found AS");
        sql.AppendLine("(");
        sql.AppendLine("    SELECT q.Id");
        sql.AppendLine("    FROM");
        sql.AppendLine("    (");

        foreach (var line in nodeSelect.Split('\n'))
            sql.Append("        ").AppendLine(line.TrimEnd('\r'));

        sql.AppendLine("    ) AS q");
        sql.AppendLine("),");
        sql.AppendLine("Paths AS");
        sql.AppendLine("(");

        // Początek: znaleziony węzeł.
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

        // Rekurencja w stronę roota.
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
        sql.AppendLine(
            "    JOIN dbo.TreeNodeSearch AS n ON n.Id = p.ParentId");

        sql.AppendLine(")");
        sql.AppendLine("SELECT FullPath");
        sql.AppendLine("FROM Paths");
        sql.AppendLine("WHERE LevelInTree = 0");
        sql.AppendLine("    AND ParentId IS NOT NULL");
        return sql.ToString();
    }
    private static string EscapeSqlString(string value)
    {
        return value.Replace("'", "''");
    }
}
