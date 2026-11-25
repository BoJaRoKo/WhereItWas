using System.Diagnostics;
using WhereItWas.Core;

namespace WhereItWas.Gui;

public partial class Form1 : Form
{
    private const string RowNumberColumnName = "RowNumber";
    private readonly SqlSessionManager _sessionManager;
    private readonly Stopwatch _queryStopwatch = new();
    private bool _loginAttempted;
    private bool _isQueryRunning;
    private IReadOnlyList<QueryDefinition> _queries = Array.Empty<QueryDefinition>();

    private string? ActualQuery
    {
        get
        {
            return comboBox1.SelectedItem is QueryInfo info
                ? _queries.FirstOrDefault(query => query.Id == info.Id)?.Sql
                : null;
        }
    }

    public Form1(SqlSessionManager sessionManager)
    {
        _sessionManager = sessionManager;
        
        InitializeComponent();
        Zaloguj();
        textBox3.Text = _sessionManager.CurrentProfile?.Database ?? "Brak bazy danych";
        comboBox1.DisplayMember = nameof(QueryInfo.Name);
        comboBox1.DataSource = Execute.QueryNames().ToList();
        comboBox1.SelectedIndex = 0;
        dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;
        dataGridView1.Sorted += DataGridView1_Sorted;
    }

    private void Zaloguj()
    {
        if (_loginAttempted)
        {
            return;
        }

        _loginAttempted = true;

        if (!_sessionManager.Login(this))
        {
            Close();
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _sessionManager.Dispose();
        base.OnFormClosed(e);
    }


    private void Button1_Click(object sender, EventArgs e)
    {
        var x = this.textBox1.Text;
        try
        {
            _queries = Execute.GetQueries(x);
            this.textBox2.Text = ActualQuery;
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }
    private void Button2_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private async void Button3_Click(object sender, EventArgs e)
    {
        try
        {
            if (_isQueryRunning)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(ActualQuery))
            {
                throw new InvalidOperationException("Najpierw wygeneruj zapytanie.");
            }

            bindingSource1.DataSource = null;
            dataGridView1.Columns.Clear();
            SetQueryExecutionState(true);
            var a = ActualQuery;
            var table = await Task.Run(() => _sessionManager.ExecuteQuery(a));
            dataGridView1.AutoGenerateColumns = true;
            bindingSource1.DataSource = table;
            EnsureRowNumberColumn();
            UpdateRowNumbers();
            tabControl1.SelectedTab = tabPage2;
        }
        catch (Exception ex)
        {
            var database = _sessionManager.CurrentProfile?.Database;
            var message = string.IsNullOrWhiteSpace(database)
                ? ex.Message
                : $"{ex.Message}{Environment.NewLine}{Environment.NewLine}Bieżąca baza: {database}";

            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetQueryExecutionState(false);
        }
    }

    private void DataGridView1_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        EnsureRowNumberColumn();
        UpdateRowNumbers();
    }

    private void DataGridView1_Sorted(object? sender, EventArgs e)
    {
        UpdateRowNumbers();
    }

    private void EnsureRowNumberColumn()
    {
        if (dataGridView1.Columns.Contains(RowNumberColumnName))
        {
            dataGridView1.Columns[RowNumberColumnName].DisplayIndex = 0;
            dataGridView1.Columns[RowNumberColumnName].Frozen = true;
            return;
        }

        var rowNumberColumn = new DataGridViewTextBoxColumn
        {
            Name = RowNumberColumnName,
            HeaderText = "Lp.",
            ReadOnly = true,
            Frozen = true,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        };

        dataGridView1.Columns.Insert(0, rowNumberColumn);
    }

    private void UpdateRowNumbers()
    {
        if (!dataGridView1.Columns.Contains(RowNumberColumnName))
        {
            return;
        }

        foreach (DataGridViewRow row in dataGridView1.Rows)
        {
            if (!row.IsNewRow)
            {
                row.Cells[RowNumberColumnName].Value = row.Index + 1;
            }
        }
    }

    private void QueryTimer_Tick(object? sender, EventArgs e)
    {
        labelExecutionTimer.Text = $"Czas: {_queryStopwatch.Elapsed.ToString(@"hh\:mm\:ss")}";
    }

    private void SetQueryExecutionState(bool isRunning)
    {
        _isQueryRunning = isRunning;
        button1.Enabled = !isRunning;
        button2.Enabled = !isRunning;
        button3.Enabled = !isRunning;
        textBox1.Enabled = !isRunning;
        comboBox1.Enabled = !isRunning;
        labelExecutionTimer.Visible = isRunning;

        if (isRunning)
        {
            _queryStopwatch.Restart();
            labelExecutionTimer.Text = "Czas: 00:00:00";
            queryTimer.Start();
            return;
        }

        queryTimer.Stop();
        _queryStopwatch.Reset();
        labelExecutionTimer.Text = "Czas: 00:00:00";
    }
}
