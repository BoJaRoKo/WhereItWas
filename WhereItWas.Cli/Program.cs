using WhereItWas.Core;

namespace WhereItWas.Cli;

class Program
{
    //[STAThread]
    static void Main(string[] args)
    {
        QueryParameters parameters = new QueryParameters(args);

        if (!parameters.OK)
        {
            Console.WriteLine(parameters.ErrorMessage);
            Console.WriteLine("*** Kończenie programu.");
            return;
        }

        using var sessionManager = new SqlSessionManager();
        var currentConnection = sessionManager.TryLoginByInfoFile();
        if (currentConnection is null)
        {
            currentConnection = sessionManager.LoginByForm(out var saveConnection);
            if (currentConnection is not null)
            {
                if (saveConnection)
                {
                    sessionManager.SaveConnection();
                }
            }
            else
            {
                Console.WriteLine("Logowanie nie powiodło się. Kończenie programu.");
                return;
            }
        }

        Execute.ExecuteQuery(parameters, currentConnection);
    }
}
