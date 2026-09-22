using System.Windows.Forms;

namespace WhereItWas.Core;

public sealed partial class ConnectionDialog : Form
{
    private readonly Func<IReadOnlyList<string>>? _serverLoader;
    private readonly Func<SqlConnectionProfile, IReadOnlyList<string>>? _databaseLoader;
    private readonly Func<SqlConnectionProfile, string?>? _connectionTester;

    public ConnectionDialog()
        : this(null, null, null, null)
    {
    }

    public ConnectionDialog(
        SqlConnectionProfile? profile,
        Func<IReadOnlyList<string>>? serverLoader,
        Func<SqlConnectionProfile, IReadOnlyList<string>>? databaseLoader,
        Func<SqlConnectionProfile, string?>? connectionTester)
    {
        InitializeComponent();

        _serverLoader = serverLoader;
        _databaseLoader = databaseLoader;
        _connectionTester = connectionTester;

        ConfigureRuntimeState();
        ApplyAuthenticationMode();
    }

    public SqlConnectionProfile ConnectionProfile => new()
    {
        AuthenticationMode = SelectedAuthenticationMode,
        Server = _serverComboBox.Text.Trim(),
        Database = _databaseComboBox.Text.Trim(),
        UserName = _userNameTextBox.Text.Trim(),
        Password = _passwordTextBox.Text
    };

    public bool SaveConnection => _rememberCheckBox.Checked;

    private SqlAuthenticationMode SelectedAuthenticationMode =>
        _authenticationComboBox.SelectedIndex == 1 ? SqlAuthenticationMode.Windows : SqlAuthenticationMode.SqlServer;

    private void ConfigureRuntimeState()
    {
        Text = "WhereItWas - połączenie z bazą";

        if (_authenticationComboBox.Items.Count == 0)
        {
            _authenticationComboBox.Items.AddRange(new object[]
            {
                "SQL Server",
                "Windows"
            });
        }

        if (_authenticationComboBox.SelectedIndex < 0 && _authenticationComboBox.Items.Count > 0)
        {
            _authenticationComboBox.SelectedIndex = 0;
        }

        ReplaceComboBoxItems(_serverComboBox, GetSuggestedServers(), keepText: false);

        _refreshServersButton.Enabled = _serverLoader is not null;
        _loadDatabasesButton.Enabled = _databaseLoader is not null;
        _testConnectionButton.Enabled = _connectionTester is not null;
        _rememberCheckBox.Checked = true;

        _authenticationComboBox.SelectedIndexChanged += AuthenticationComboBox_SelectedIndexChanged;
        _refreshServersButton.Click += RefreshServersButton_Click;
        _loadDatabasesButton.Click += LoadDatabasesButton_Click;
        _testConnectionButton.Click += TestConnectionButton_Click;
        _okButton.Click += OkButton_Click;
    }

    private void ApplyProfile(SqlConnectionProfile? profile)
    {
        _authenticationComboBox.SelectedIndex = profile?.AuthenticationMode == SqlAuthenticationMode.Windows ? 1 : 0;
        _serverComboBox.Text = profile?.Server ?? string.Empty;
        _databaseComboBox.Text = profile?.Database ?? string.Empty;
        _userNameTextBox.Text = profile?.UserName ?? string.Empty;
        _passwordTextBox.Text = profile?.Password ?? string.Empty;
    }

    private void AuthenticationComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        ApplyAuthenticationMode();
    }

    private void ApplyAuthenticationMode()
    {
        var useWindowsAuthentication = SelectedAuthenticationMode == SqlAuthenticationMode.Windows;
        var credentialsHostRow = GetCredentialsHostRow();

        _credentialsPanel.Visible = !useWindowsAuthentication;
        _credentialsPanel.Enabled = !useWindowsAuthentication;

        if (credentialsHostRow is not null)
        {
            credentialsHostRow.Height = useWindowsAuthentication ? 0 : 70;
            credentialsHostRow.SizeType = useWindowsAuthentication ? SizeType.Absolute : SizeType.AutoSize;
        }

        _userNameTextBox.Enabled = !useWindowsAuthentication;
        _passwordTextBox.Enabled = !useWindowsAuthentication;

        _statusLabel.Text = useWindowsAuthentication
            ? "Tryb Windows użyje poświadczeń bieżącego użytkownika systemu."
            : "Tryb SQL Server wymaga podania loginu i hasła użytkownika bazy.";
    }

    private RowStyle? GetCredentialsHostRow()
    {
        if (_credentialsHostRow is not null)
        {
            return _credentialsHostRow;
        }

        if (_connectionLayout.RowStyles.Count <= 3)
        {
            return null;
        }

        _credentialsHostRow = _connectionLayout.RowStyles[3];
        return _credentialsHostRow;
    }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_serverComboBox.Text) ||
            string.IsNullOrWhiteSpace(_databaseComboBox.Text))
        {
            MessageBox.Show(this, "Uzupełnij serwer i bazę danych.", "WhereItWas - brak danych", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (SelectedAuthenticationMode == SqlAuthenticationMode.SqlServer &&
            (string.IsNullOrWhiteSpace(_userNameTextBox.Text) || string.IsNullOrWhiteSpace(_passwordTextBox.Text)))
        {
            MessageBox.Show(this, "Dla uwierzytelniania SQL Server podaj login i hasło.", "WhereItWas - brak danych", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void RefreshServersButton_Click(object? sender, EventArgs e)
    {
        if (_serverLoader is null)
        {
            return;
        }

        try
        {
            UseWaitCursor = true;
            _refreshServersButton.Enabled = false;
            _statusLabel.Text = "Wyszukiwanie instancji SQL Server...";

            var servers = _serverLoader();
            ReplaceComboBoxItems(_serverComboBox, servers, keepText: true);

            _statusLabel.Text = _serverComboBox.Items.Count > 0
                ? "Odświeżono listę sugerowanych i wykrytych instancji SQL Server."
                : "Nie wykryto dodatkowych instancji SQL Server.";
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "WhereItWas - serwery SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _statusLabel.Text = "Nie udało się odświeżyć listy serwerów.";
        }
        finally
        {
            UseWaitCursor = false;
            _refreshServersButton.Enabled = _serverLoader is not null;
        }
    }

    private void LoadDatabasesButton_Click(object? sender, EventArgs e)
    {
        if (_databaseLoader is null)
        {
            return;
        }

        try
        {
            UseWaitCursor = true;
            _loadDatabasesButton.Enabled = false;
            _statusLabel.Text = "Pobieranie listy baz...";

            var databases = _databaseLoader(ConnectionProfile);
            ReplaceComboBoxItems(_databaseComboBox, databases, keepText: true);

            _statusLabel.Text = _databaseComboBox.Items.Count > 0
                ? "Pobrano listę baz dla wskazanego serwera."
                : "Nie znaleziono baz lub serwer nie zwrócił wyników.";
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "WhereItWas - pobieranie baz", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _statusLabel.Text = "Nie udało się pobrać listy baz.";
        }
        finally
        {
            UseWaitCursor = false;
            _loadDatabasesButton.Enabled = _databaseLoader is not null;
        }
    }

    private void TestConnectionButton_Click(object? sender, EventArgs e)
    {
        if (_connectionTester is null)
        {
            return;
        }

        try
        {
            UseWaitCursor = true;
            _testConnectionButton.Enabled = false;
            _statusLabel.Text = "Testowanie połączenia...";

            var result = _connectionTester(ConnectionProfile);

            if (string.IsNullOrWhiteSpace(result))
            {
                _statusLabel.Text = "Połączenie zakończyło się powodzeniem.";
                MessageBox.Show(this, "Połączenie z bazą danych zakończyło się powodzeniem.", "WhereItWas - test połączenia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _statusLabel.Text = result;
            MessageBox.Show(this, result, "WhereItWas - test połączenia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "WhereItWas - test połączenia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _statusLabel.Text = "Test połączenia zakończył się błędem.";
        }
        finally
        {
            UseWaitCursor = false;
            _testConnectionButton.Enabled = _connectionTester is not null;
        }
    }

    private static void ReplaceComboBoxItems(ComboBox comboBox, IReadOnlyList<string> items, bool keepText)
    {
        var currentText = comboBox.Text;

        comboBox.BeginUpdate();
        comboBox.Items.Clear();
        comboBox.Items.AddRange(items.Distinct(StringComparer.OrdinalIgnoreCase).Cast<object>().ToArray());
        comboBox.EndUpdate();

        if (keepText)
        {
            comboBox.Text = currentText;
        }

        if (comboBox.Items.Count > 0 && string.IsNullOrWhiteSpace(comboBox.Text))
        {
            comboBox.SelectedIndex = 0;
        }
    }

    private static string[] GetSuggestedServers()
    {
        return
        [
            ".",
            "(local)",
            "localhost",
            "(localdb)\\MSSQLLocalDB",
            Environment.MachineName,
            $"{Environment.MachineName}\\SQLEXPRESS",
            ".\\SQLEXPRESS",
            "localhost\\SQLEXPRESS"
        ];
    }
}
