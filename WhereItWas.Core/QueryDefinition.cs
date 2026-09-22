namespace WhereItWas.Core;

public sealed record QueryDefinition(
    string Id,
    string Sql,
    string Name,
    string Description);

public sealed record QueryInfo(
    string Id,
    string Name,
    string Description);
