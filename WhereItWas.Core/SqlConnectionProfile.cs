namespace WhereItWas.Core;

public sealed class SqlConnectionProfile
{
    public SqlAuthenticationMode AuthenticationMode { get; set; } = SqlAuthenticationMode.SqlServer;

    public string Server { get; set; } = string.Empty;

    public string Database { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
