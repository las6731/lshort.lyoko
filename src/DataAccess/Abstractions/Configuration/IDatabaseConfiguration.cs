namespace LShort.Lyoko.DataAccess.Abstractions.Configuration;

public interface IDatabaseConfiguration
{
    public string Username { get; init; }

    public string Password { get; init; }

    public string Host { get; init; }

    public string Database { get; init; }
}