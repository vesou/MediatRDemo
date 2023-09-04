namespace Mediator7Hangfire;

public class DatabaseConnectionOptions
{
    public string Host { get; set; } = string.Empty;

    public string Database { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string AllowUserVariables { get; set; } = string.Empty;
}