

namespace Mediator7Hangfire;

public static class ConnectionHelper
{
    public static string CreateMySqlConnectionString(DatabaseConnectionOptions options)
    {
        return $"server={options.Host};database={options.Database};user={options.Username};password={options.Password};allowUserVariables={options.AllowUserVariables}";
    }
}