using WhereItWas.Core;

namespace WhereItWas.Gui;

public partial class Form1 : Form
{
    private readonly SqlSessionManager _sessionManager;
    private bool _loginAttempted;

    public Form1(SqlSessionManager sessionManager)
    {
        _sessionManager = sessionManager;
        InitializeComponent();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

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
        var par = new Stack<string>();
        par.Push(this.textBox1.Text);
        var x = new QueryParameters(par.ToArray());
        try
        {
            string sql = Execute.SqlSelectWithPath(x);
            this.textBox2.Text = sql;
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
}
