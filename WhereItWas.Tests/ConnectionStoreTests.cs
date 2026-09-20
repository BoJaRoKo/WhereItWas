using WhereItWas.Core;

namespace WhereItWas.Tests;

public sealed class ConnectionStoreTests
{
    [Fact]
    public void ReadConnection_ReturnsNull_WhenLoginFileDoesNotExist()
    {
        using var scope = new LoginFileScope();
        scope.DeleteLoginFile();
        var store = new ConnectionStore();

        var profile = store.ReadConnection();

        Assert.Null(profile);
    }

    [Fact]
    public void WriteConnection_ThenReadConnection_RoundTripsProfile()
    {
        using var scope = new LoginFileScope();
        scope.DeleteLoginFile();
        var store = new ConnectionStore();
        var expected = new SqlConnectionProfile
        {
            AuthenticationMode = SqlAuthenticationMode.Windows,
            Server = "(localdb)\\MSSQLLocalDB",
            Database = "master",
            UserName = string.Empty,
            Password = string.Empty
        };

        store.WriteConnection(expected);
        var actual = store.ReadConnection();

        Assert.NotNull(actual);
        Assert.Equal(expected.AuthenticationMode, actual!.AuthenticationMode);
        Assert.Equal(expected.Server, actual.Server);
        Assert.Equal(expected.Database, actual.Database);
        Assert.Equal(expected.UserName, actual.UserName);
        Assert.Equal(expected.Password, actual.Password);
    }
}
