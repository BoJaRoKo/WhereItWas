WITH Found AS
(
    SELECT q.Id
    FROM
    (
        SELECT n0.*
        FROM dbo.TreeNodeSearch AS n0
        WHERE
            n0.Name LIKE N'%.scad%'
    ) AS q
),
Paths AS
(
    SELECT
        f.Id AS ResultId,
        n.Id,
        n.ParentId,
        n.LevelInTree,
        CAST(n.Name AS nvarchar(max)) AS FullPath
    FROM Found AS f
    JOIN dbo.TreeNodeSearch AS n ON n.Id = f.Id

    UNION ALL

    SELECT
        p.ResultId,
        n.Id,
        n.ParentId,
        n.LevelInTree,
        CAST(n.Name +
            CASE
                WHEN RIGHT(n.Name, 1) = N'\' THEN N''
                ELSE N'\'
            END +
            p.FullPath AS nvarchar(max)) AS FullPath
    FROM Paths AS p
    JOIN dbo.TreeNodeSearch AS n ON n.Id = p.ParentId
)
SELECT FullPath
FROM Paths
WHERE LevelInTree = 0
    AND ParentId IS NOT NULL
