using WhereItWas.Core;

namespace WhereItWas.Tests;

public sealed class SqlSessionManagerTests
{
    [Fact]
    public void TryLoginByInfoFile_ReturnsNull_WhenLoginFileDoesNotExist()
    {
        using var scope = new LoginFileScope();
        scope.DeleteLoginFile();
        using var sessionManager = new SqlSessionManager();

        var profile = sessionManager.TryLoginByInfoFile();

        Assert.Null(profile);
        Assert.Null(sessionManager.Connection);
        Assert.Null(sessionManager.CurrentProfile);
    }

    [Fact]
    public void SaveConnection_DoesNothing_WhenThereIsNoCurrentProfile()
    {
        using var scope = new LoginFileScope();
        scope.DeleteLoginFile();
        using var sessionManager = new SqlSessionManager();

        sessionManager.SaveConnection();

        Assert.False(File.Exists(scope.FilePath));
    }

    [Fact]
    public void TryLoginByInfoFile_ReturnsNull_WhenStoredConnectionCannotBeOpened()
    {
        using var scope = new LoginFileScope();
        scope.DeleteLoginFile();
        using var sessionManager = new SqlSessionManager();

        sessionManager.WriteConnection(new SqlConnectionProfile
        {
            AuthenticationMode = SqlAuthenticationMode.SqlServer,
            Server = "invalid-host-name-for-whereitwas-tests",
            Database = "missing-database",
            UserName = "invalid-user",
            Password = "invalid-password"
        });

        var profile = sessionManager.TryLoginByInfoFile();

        Assert.Null(profile);
        Assert.Null(sessionManager.Connection);
        Assert.Null(sessionManager.CurrentProfile);
    }
}
