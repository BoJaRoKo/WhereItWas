using WhereItWas.Core;

namespace WhereItWas.Gui;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    //[STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1(new SqlSessionManager()));
    }    
}