#nullable enable

#nullable enable

namespace WhereItWas.Core;

partial class ConnectionDialog
{
    private System.ComponentModel.IContainer? components = null;
    private System.Windows.Forms.TableLayoutPanel _rootLayout = null!;
    private System.Windows.Forms.Label _headerLabel = null!;
    private System.Windows.Forms.GroupBox _connectionGroupBox = null!;
    private System.Windows.Forms.TableLayoutPanel _connectionLayout = null!;
    private System.Windows.Forms.Label _serverLabel = null!;
    private System.Windows.Forms.ComboBox _serverComboBox = null!;
    private System.Windows.Forms.Button _refreshServersButton = null!;
    private System.Windows.Forms.Label _databaseLabel = null!;
    private System.Windows.Forms.ComboBox _databaseComboBox = null!;
    private System.Windows.Forms.Button _loadDatabasesButton = null!;
    private System.Windows.Forms.Label _authenticationLabel = null!;
    private System.Windows.Forms.ComboBox _authenticationComboBox = null!;
    private System.Windows.Forms.Button _testConnectionButton = null!;
    private System.Windows.Forms.TableLayoutPanel _credentialsPanel = null!;
    private System.Windows.Forms.Label _userNameLabel = null!;
    private System.Windows.Forms.TextBox _userNameTextBox = null!;
    private System.Windows.Forms.Label _passwordLabel = null!;
    private System.Windows.Forms.TextBox _passwordTextBox = null!;
    private System.Windows.Forms.CheckBox _rememberCheckBox = null!;
    private System.Windows.Forms.Label _statusLabel = null!;
    private System.Windows.Forms.Label _infoLabel = null!;
    private System.Windows.Forms.FlowLayoutPanel _buttonsPanel = null!;
    private System.Windows.Forms.Button _cancelButton = null!;
    private System.Windows.Forms.Button _okButton = null!;
    private System.Windows.Forms.RowStyle _credentialsHostRow = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _rootLayout = new TableLayoutPanel();
        _headerLabel = new Label();
        _connectionGroupBox = new GroupBox();
        _connectionLayout = new TableLayoutPanel();
        _serverLabel = new Label();
        _serverComboBox = new ComboBox();
        _refreshServersButton = new Button();
        _databaseLabel = new Label();
        _databaseComboBox = new ComboBox();
        _loadDatabasesButton = new Button();
        _authenticationLabel = new Label();
        _authenticationComboBox = new ComboBox();
        _testConnectionButton = new Button();
        _credentialsPanel = new TableLayoutPanel();
        _userNameLabel = new Label();
        _userNameTextBox = new TextBox();
        _passwordLabel = new Label();
        _passwordTextBox = new TextBox();
        _rememberCheckBox = new CheckBox();
        _statusLabel = new Label();
        _infoLabel = new Label();
        _buttonsPanel = new FlowLayoutPanel();
        _cancelButton = new Button();
        _okButton = new Button();
        _rootLayout.SuspendLayout();
        _connectionGroupBox.SuspendLayout();
        _connectionLayout.SuspendLayout();
        _credentialsPanel.SuspendLayout();
        _buttonsPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _rootLayout
        // 
        _rootLayout.ColumnCount = 1;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.Controls.Add(_headerLabel, 0, 0);
        _rootLayout.Controls.Add(_connectionGroupBox, 0, 1);
        _rootLayout.Controls.Add(_infoLabel, 0, 2);
        _rootLayout.Controls.Add(_buttonsPanel, 0, 3);
        _rootLayout.Dock = DockStyle.Fill;
        _rootLayout.Location = new Point(0, 0);
        _rootLayout.Margin = new Padding(4);
        _rootLayout.Name = "_rootLayout";
        _rootLayout.Padding = new Padding(15);
        _rootLayout.RowCount = 4;
        _rootLayout.RowStyles.Add(new RowStyle());
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _rootLayout.RowStyles.Add(new RowStyle());
        _rootLayout.RowStyles.Add(new RowStyle());
        _rootLayout.Size = new Size(775, 555);
        _rootLayout.TabIndex = 0;
        // 
        // _headerLabel
        // 
        _headerLabel.AutoSize = true;
        _headerLabel.Dock = DockStyle.Fill;
        _headerLabel.Location = new Point(19, 15);
        _headerLabel.Margin = new Padding(4, 0, 4, 15);
        _headerLabel.MaximumSize = new Size(725, 0);
        _headerLabel.Name = "_headerLabel";
        _headerLabel.Size = new Size(725, 50);
        _headerLabel.TabIndex = 0;
        _headerLabel.Text = "Wybierz lub wpisz instancję SQL Server, określ bazę oraz sposób logowania. Ustawienia mogą zostać zapisane po udanym połączeniu.";
        // 
        // _connectionGroupBox
        // 
        _connectionGroupBox.Controls.Add(_connectionLayout);
        _connectionGroupBox.Dock = DockStyle.Fill;
        _connectionGroupBox.Location = new Point(19, 84);
        _connectionGroupBox.Margin = new Padding(4);
        _connectionGroupBox.Name = "_connectionGroupBox";
        _connectionGroupBox.Padding = new Padding(15);
        _connectionGroupBox.Size = new Size(737, 336);
        _connectionGroupBox.TabIndex = 1;
        _connectionGroupBox.TabStop = false;
        _connectionGroupBox.Text = "Połączenie";
        // 
        // _connectionLayout
        // 
        _connectionLayout.ColumnCount = 3;
        _connectionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 156F));
        _connectionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _connectionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 162F));
        _connectionLayout.Controls.Add(_serverLabel, 0, 0);
        _connectionLayout.Controls.Add(_serverComboBox, 1, 0);
        _connectionLayout.Controls.Add(_refreshServersButton, 2, 0);
        _connectionLayout.Controls.Add(_databaseLabel, 0, 1);
        _connectionLayout.Controls.Add(_databaseComboBox, 1, 1);
        _connectionLayout.Controls.Add(_loadDatabasesButton, 2, 1);
        _connectionLayout.Controls.Add(_authenticationLabel, 0, 2);
        _connectionLayout.Controls.Add(_authenticationComboBox, 1, 2);
        _connectionLayout.Controls.Add(_testConnectionButton, 2, 2);
        _connectionLayout.Controls.Add(_credentialsPanel, 0, 3);
        _connectionLayout.Controls.Add(_rememberCheckBox, 0, 4);
        _connectionLayout.Controls.Add(_statusLabel, 0, 5);
        _connectionLayout.Dock = DockStyle.Fill;
        _connectionLayout.Location = new Point(15, 39);
        _connectionLayout.Margin = new Padding(4);
        _connectionLayout.Name = "_connectionLayout";
        _connectionLayout.RowCount = 6;
        _connectionLayout.RowStyles.Add(new RowStyle());
        _connectionLayout.RowStyles.Add(new RowStyle());
        _connectionLayout.RowStyles.Add(new RowStyle());
        _connectionLayout.RowStyles.Add(new RowStyle());
        _connectionLayout.RowStyles.Add(new RowStyle());
        _connectionLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _connectionLayout.Size = new Size(707, 282);
        _connectionLayout.TabIndex = 0;
        // 
        // _serverLabel
        // 
        _serverLabel.AutoSize = true;
        _serverLabel.Dock = DockStyle.Fill;
        _serverLabel.Location = new Point(4, 9);
        _serverLabel.Margin = new Padding(4, 9, 10, 9);
        _serverLabel.Name = "_serverLabel";
        _serverLabel.Size = new Size(142, 25);
        _serverLabel.TabIndex = 0;
        _serverLabel.Text = "Serwer SQL";
        _serverLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _serverComboBox
        // 
        _serverComboBox.Dock = DockStyle.Top;
        _serverComboBox.FormattingEnabled = true;
        _serverComboBox.Location = new Point(160, 4);
        _serverComboBox.Margin = new Padding(4);
        _serverComboBox.Name = "_serverComboBox";
        _serverComboBox.Size = new Size(381, 33);
        _serverComboBox.TabIndex = 1;
        // 
        // _refreshServersButton
        // 
        _refreshServersButton.Anchor = AnchorStyles.Left;
        _refreshServersButton.Location = new Point(549, 4);
        _refreshServersButton.Margin = new Padding(4);
        _refreshServersButton.Name = "_refreshServersButton";
        _refreshServersButton.Size = new Size(148, 35);
        _refreshServersButton.TabIndex = 2;
        _refreshServersButton.Text = "Odśwież serwery";
        _refreshServersButton.UseVisualStyleBackColor = true;
        // 
        // _databaseLabel
        // 
        _databaseLabel.AutoSize = true;
        _databaseLabel.Dock = DockStyle.Fill;
        _databaseLabel.Location = new Point(4, 52);
        _databaseLabel.Margin = new Padding(4, 9, 10, 9);
        _databaseLabel.Name = "_databaseLabel";
        _databaseLabel.Size = new Size(142, 25);
        _databaseLabel.TabIndex = 3;
        _databaseLabel.Text = "Baza danych";
        _databaseLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _databaseComboBox
        // 
        _databaseComboBox.Dock = DockStyle.Top;
        _databaseComboBox.FormattingEnabled = true;
        _databaseComboBox.Location = new Point(160, 47);
        _databaseComboBox.Margin = new Padding(4);
        _databaseComboBox.Name = "_databaseComboBox";
        _databaseComboBox.Size = new Size(381, 33);
        _databaseComboBox.TabIndex = 4;
        // 
        // _loadDatabasesButton
        // 
        _loadDatabasesButton.Anchor = AnchorStyles.Left;
        _loadDatabasesButton.Location = new Point(549, 47);
        _loadDatabasesButton.Margin = new Padding(4);
        _loadDatabasesButton.Name = "_loadDatabasesButton";
        _loadDatabasesButton.Size = new Size(148, 35);
        _loadDatabasesButton.TabIndex = 5;
        _loadDatabasesButton.Text = "Pobierz bazy";
        _loadDatabasesButton.UseVisualStyleBackColor = true;
        // 
        // _authenticationLabel
        // 
        _authenticationLabel.AutoSize = true;
        _authenticationLabel.Dock = DockStyle.Fill;
        _authenticationLabel.Location = new Point(4, 95);
        _authenticationLabel.Margin = new Padding(4, 9, 10, 9);
        _authenticationLabel.Name = "_authenticationLabel";
        _authenticationLabel.Size = new Size(142, 25);
        _authenticationLabel.TabIndex = 6;
        _authenticationLabel.Text = "Uwierzytelnianie";
        _authenticationLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _authenticationComboBox
        // 
        _authenticationComboBox.Dock = DockStyle.Top;
        _authenticationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _authenticationComboBox.FormattingEnabled = true;
        _authenticationComboBox.Location = new Point(160, 90);
        _authenticationComboBox.Margin = new Padding(4);
        _authenticationComboBox.Name = "_authenticationComboBox";
        _authenticationComboBox.Size = new Size(381, 33);
        _authenticationComboBox.TabIndex = 7;
        // 
        // _testConnectionButton
        // 
        _testConnectionButton.Anchor = AnchorStyles.Left;
        _testConnectionButton.Location = new Point(549, 90);
        _testConnectionButton.Margin = new Padding(4);
        _testConnectionButton.Name = "_testConnectionButton";
        _testConnectionButton.Size = new Size(148, 35);
        _testConnectionButton.TabIndex = 8;
        _testConnectionButton.Text = "Test połączenia";
        _testConnectionButton.UseVisualStyleBackColor = true;
        // 
        // _credentialsPanel
        // 
        _credentialsPanel.ColumnCount = 2;
        _connectionLayout.SetColumnSpan(_credentialsPanel, 3);
        _credentialsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 156F));
        _credentialsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _credentialsPanel.Controls.Add(_userNameLabel, 0, 0);
        _credentialsPanel.Controls.Add(_userNameTextBox, 1, 0);
        _credentialsPanel.Controls.Add(_passwordLabel, 0, 1);
        _credentialsPanel.Controls.Add(_passwordTextBox, 1, 1);
        _credentialsPanel.Dock = DockStyle.Fill;
        _credentialsPanel.Location = new Point(4, 139);
        _credentialsPanel.Margin = new Padding(4, 10, 4, 0);
        _credentialsPanel.Name = "_credentialsPanel";
        _credentialsPanel.RowCount = 2;
        _credentialsPanel.RowStyles.Add(new RowStyle());
        _credentialsPanel.RowStyles.Add(new RowStyle());
        _credentialsPanel.Size = new Size(699, 78);
        _credentialsPanel.TabIndex = 9;
        // 
        // _userNameLabel
        // 
        _userNameLabel.AutoSize = true;
        _userNameLabel.Dock = DockStyle.Fill;
        _userNameLabel.Location = new Point(4, 9);
        _userNameLabel.Margin = new Padding(4, 9, 10, 9);
        _userNameLabel.Name = "_userNameLabel";
        _userNameLabel.Size = new Size(142, 25);
        _userNameLabel.TabIndex = 0;
        _userNameLabel.Text = "Login";
        _userNameLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _userNameTextBox
        // 
        _userNameTextBox.Dock = DockStyle.Top;
        _userNameTextBox.Location = new Point(160, 4);
        _userNameTextBox.Margin = new Padding(4);
        _userNameTextBox.Name = "_userNameTextBox";
        _userNameTextBox.Size = new Size(535, 31);
        _userNameTextBox.TabIndex = 1;
        // 
        // _passwordLabel
        // 
        _passwordLabel.AutoSize = true;
        _passwordLabel.Dock = DockStyle.Fill;
        _passwordLabel.Location = new Point(4, 52);
        _passwordLabel.Margin = new Padding(4, 9, 10, 9);
        _passwordLabel.Name = "_passwordLabel";
        _passwordLabel.Size = new Size(142, 25);
        _passwordLabel.TabIndex = 2;
        _passwordLabel.Text = "Hasło";
        _passwordLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _passwordTextBox
        // 
        _passwordTextBox.Dock = DockStyle.Top;
        _passwordTextBox.Location = new Point(160, 47);
        _passwordTextBox.Margin = new Padding(4);
        _passwordTextBox.Name = "_passwordTextBox";
        _passwordTextBox.Size = new Size(535, 31);
        _passwordTextBox.TabIndex = 3;
        _passwordTextBox.UseSystemPasswordChar = true;
        // 
        // _rememberCheckBox
        // 
        _rememberCheckBox.AutoSize = true;
        _connectionLayout.SetColumnSpan(_rememberCheckBox, 3);
        _rememberCheckBox.Dock = DockStyle.Fill;
        _rememberCheckBox.Location = new Point(4, 221);
        _rememberCheckBox.Margin = new Padding(4);
        _rememberCheckBox.Name = "_rememberCheckBox";
        _rememberCheckBox.Size = new Size(699, 29);
        _rememberCheckBox.TabIndex = 10;
        _rememberCheckBox.Text = "Zapamiętaj ustawienia po udanym logowaniu";
        _rememberCheckBox.UseVisualStyleBackColor = true;
        // 
        // _statusLabel
        // 
        _statusLabel.AutoSize = true;
        _connectionLayout.SetColumnSpan(_statusLabel, 3);
        _statusLabel.Dock = DockStyle.Fill;
        _statusLabel.ForeColor = SystemColors.GrayText;
        _statusLabel.Location = new Point(4, 254);
        _statusLabel.Margin = new Padding(4, 0, 4, 0);
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Size = new Size(699, 28);
        _statusLabel.TabIndex = 11;
        _statusLabel.Text = "Stan połączenia";
        // 
        // _infoLabel
        // 
        _infoLabel.AutoSize = true;
        _infoLabel.Dock = DockStyle.Fill;
        _infoLabel.ForeColor = SystemColors.GrayText;
        _infoLabel.Location = new Point(19, 436);
        _infoLabel.Margin = new Padding(4, 12, 4, 0);
        _infoLabel.Name = "_infoLabel";
        _infoLabel.Size = new Size(737, 50);
        _infoLabel.TabIndex = 2;
        _infoLabel.Text = "Tryb Windows użyje poświadczeń bieżącego użytkownika systemu. Tryb SQL Server użyje loginu i hasła z formularza.";
        // 
        // _buttonsPanel
        // 
        _buttonsPanel.AutoSize = true;
        _buttonsPanel.Controls.Add(_cancelButton);
        _buttonsPanel.Controls.Add(_okButton);
        _buttonsPanel.Dock = DockStyle.Right;
        _buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
        _buttonsPanel.Location = new Point(512, 501);
        _buttonsPanel.Margin = new Padding(4, 15, 4, 4);
        _buttonsPanel.Name = "_buttonsPanel";
        _buttonsPanel.Size = new Size(244, 35);
        _buttonsPanel.TabIndex = 3;
        _buttonsPanel.WrapContents = false;
        // 
        // _cancelButton
        // 
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Location = new Point(132, 0);
        _cancelButton.Margin = new Padding(10, 0, 0, 0);
        _cancelButton.Name = "_cancelButton";
        _cancelButton.Size = new Size(112, 35);
        _cancelButton.TabIndex = 1;
        _cancelButton.Text = "Anuluj";
        _cancelButton.UseVisualStyleBackColor = true;
        // 
        // _okButton
        // 
        _okButton.Location = new Point(10, 0);
        _okButton.Margin = new Padding(10, 0, 0, 0);
        _okButton.Name = "_okButton";
        _okButton.Size = new Size(112, 35);
        _okButton.TabIndex = 0;
        _okButton.Text = "OK";
        _okButton.UseVisualStyleBackColor = true;
        // 
        // ConnectionDialog
        // 
        AcceptButton = _okButton;
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = _cancelButton;
        ClientSize = new Size(775, 555);
        Controls.Add(_rootLayout);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Margin = new Padding(4);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ConnectionDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterScreen;
        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _connectionGroupBox.ResumeLayout(false);
        _connectionLayout.ResumeLayout(false);
        _connectionLayout.PerformLayout();
        _credentialsPanel.ResumeLayout(false);
        _credentialsPanel.PerformLayout();
        _buttonsPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
