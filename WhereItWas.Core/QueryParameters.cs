using System.Text;

namespace WhereItWas.Core;

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

public sealed record QueryParameters
{
    public required TranslatedElement[] Elements { get; init; }

    public NodeType NodeType { get; init; } = NodeType.Any;

    public int DisplayedNodeNr { get; init; } = 0;

    public uint Limit { get; init; } = 0;

    public string OrderBy { get; init; } = string.Empty;
}
