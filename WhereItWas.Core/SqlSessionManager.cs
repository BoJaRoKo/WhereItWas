using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;

namespace WhereItWas.Core;

public sealed class SqlSessionManager : IDisposable
{
    private readonly ConnectionStore _connectionStore = new();
    private SqlConnection? _connection;

    public SqlConnection? Connection => _connection;

    public SqlConnectionProfile? CurrentProfile { get; private set; }

    public SqlConnectionProfile? ReadConnection()
    {
        return _connectionStore.ReadConnection();
    }

    public void WriteConnection(SqlConnectionProfile profile)
    {
        _connectionStore.WriteConnection(profile);
    }

    public bool Login(IWin32Window? owner = null)
    {
        if (TryLoginByInfoFile() is not null)
        {
            return true;
        }

        var profile = LoginByForm(owner);
        if (profile is null)
        {
            return false;
        }

        SaveConnection();
        return true;
    }

    public void Login(SqlConnectionProfile profile, bool saveConnection)
    {
        ArgumentNullException.ThrowIfNull(profile);

        Logout();

        var connectionString = BuildConnectionString(profile, profile.Database);
        var connection = new SqlConnection(connectionString);
        connection.Open();

        _connection = connection;
        CurrentProfile = profile;

        if (saveConnection)
        {
            WriteConnection(profile);
        }
    }

    public void Logout()
    {
        if (_connection is null)
        {
            return;
        }

        try
        {
            _connection.Dispose();
            _connection = null;
            CurrentProfile = null;
        }
        finally
        {
            _connection = null;
            CurrentProfile = null;
        }
    }

    public IReadOnlyList<string> GetAvailableDatabases(SqlConnectionProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        var connectionString = BuildConnectionString(profile, "master");
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        command.CommandText = @"
SELECT [name]
FROM sys.databases
WHERE [state] = 0
ORDER BY [name];";

        var databases = new List<string>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            databases.Add(reader.GetString(0));
        }

        return databases;
    }

    public IReadOnlyList<string> GetAvailableServers()
    {
        var serverNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".",
            "(local)",
            "(localdb)\\MSSQLLocalDB",
            "localhost",
            Environment.MachineName,
            $"{Environment.MachineName}\\SQLEXPRESS",
            ".\\SQLEXPRESS",
            "localhost\\SQLEXPRESS"
        };

        AddServerNames(serverNames, RegistryView.Registry64);
        AddServerNames(serverNames, RegistryView.Registry32);
        AddLocalDbNames(serverNames);

        return serverNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public string? TestConnection(SqlConnectionProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        using var connection = new SqlConnection(BuildConnectionString(profile, profile.Database));
        connection.Open();
        return null;
    }

    private static string BuildConnectionString(SqlConnectionProfile profile, string database)
    {
        return new SqlConnectionStringBuilder
        {
            DataSource = profile.Server,
            InitialCatalog = database,
            UserID = profile.AuthenticationMode == SqlAuthenticationMode.SqlServer ? profile.UserName : string.Empty,
            Password = profile.AuthenticationMode == SqlAuthenticationMode.SqlServer ? profile.Password : string.Empty,
            IntegratedSecurity = profile.AuthenticationMode == SqlAuthenticationMode.Windows,
            TrustServerCertificate = true
        }.ConnectionString;
    }

    private static void AddServerNames(ISet<string> serverNames, RegistryView registryView)
    {
        using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView);
        using var instanceNamesKey = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL");
        if (instanceNamesKey is null)
        {
            return;
        }

        foreach (var instanceName in instanceNamesKey.GetValueNames())
        {
            serverNames.Add(string.Equals(instanceName, "MSSQLSERVER", StringComparison.OrdinalIgnoreCase)
                ? Environment.MachineName
                : $"{Environment.MachineName}\\{instanceName}");
        }
    }

    private static void AddLocalDbNames(ISet<string> serverNames)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sqllocaldb",
                    Arguments = "i",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return;
            }

            foreach (var line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var instanceName = line.Trim();
                if (instanceName.Length > 0)
                {
                    serverNames.Add($"(localdb)\\{instanceName}");
                }
            }
        }
        catch
        {
        }
    }

    public SqlConnectionProfile? TryLoginByInfoFile()
    {
        var storedProfile = ReadConnection();
        if (storedProfile is null)
        {
            return null;
        }

        try
        {
            Login(storedProfile, saveConnection: false);
            return CurrentProfile;
        }
        catch
        {
            Logout();
            return null;
        }
    }

    public SqlConnectionProfile? LoginByForm()
    {
        return LoginByForm(owner: null);
    }

    public SqlConnectionProfile? LoginByForm(IWin32Window? owner)
    {
        var profile = ReadConnection() ?? CurrentProfile;

        while (true)
        {
            using var dialog = new ConnectionDialog(profile, GetAvailableServers, GetAvailableDatabases, TestConnection);
            if (dialog.ShowDialog(owner) != DialogResult.OK)
            {
                return null;
            }

            profile = dialog.ConnectionProfile;

            try
            {
                Login(profile, saveConnection: false);
                return CurrentProfile;
            }
            catch (Exception exception)
            {
                MessageBox.Show(owner, exception.Message, "WhereItWas - błąd logowania", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public void SaveConnection()
    {
        if (CurrentProfile is null)
        {
            return;
        }

        WriteConnection(CurrentProfile);
    }

    public void Dispose()
    {
        Logout();
    }
}
