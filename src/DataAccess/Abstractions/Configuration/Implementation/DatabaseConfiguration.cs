namespace LShort.Lyoko.DataAccess.Abstractions.Configuration;

public class DatabaseConfiguration : IDatabaseConfiguration
{
    public string Username { get; init; } = "root";

    public string Password { get; init; } = "password";

    public string Host { get; init; } = "localhost";

    public string Database { get; init; } = string.Empty;
}