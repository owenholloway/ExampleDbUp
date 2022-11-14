using System.Text;

namespace DbUp.Lib;

public static class ConnectionString
{
    public static string GetString()
    {
        var dbHost 
            = Environment.GetEnvironmentVariable("DB_HOST");
        var dbUser 
            = Environment.GetEnvironmentVariable("DB_USER");
        var dbPass 
            = Environment.GetEnvironmentVariable("DB_PASS");
        var dbSchema 
            = Environment.GetEnvironmentVariable("DB_SCHEMA");

        var connectionStringBuilder = new StringBuilder();
        connectionStringBuilder.Append("Host=");
        connectionStringBuilder.Append(dbHost);
        connectionStringBuilder.Append(';');
        connectionStringBuilder.Append("Username=");
        connectionStringBuilder.Append(dbUser);
        connectionStringBuilder.Append(';');
        connectionStringBuilder.Append("Password=");
        connectionStringBuilder.Append(dbPass);
        connectionStringBuilder.Append(';');
        connectionStringBuilder.Append("Database=");
        connectionStringBuilder.Append(dbSchema);

        return connectionStringBuilder.ToString();
    }
}