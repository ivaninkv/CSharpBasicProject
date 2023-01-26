namespace FlightsMetaSubscriber.App;

public static class Config
{
    private static readonly string dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    private static readonly string dbName = Environment.GetEnvironmentVariable("DB_NAME");
    private static readonly string dbPass = Environment.GetEnvironmentVariable("DB_PASS");

    public static readonly string ConnectionString =
        $"User ID=postgres;Password={dbPass};Host={dbHost};Port=5432;Database={dbName};";
}
