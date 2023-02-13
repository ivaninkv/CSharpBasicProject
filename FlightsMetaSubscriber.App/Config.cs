namespace FlightsMetaSubscriber.App;

public static class Config
{
    private static readonly string? dbHost = Environment.GetEnvironmentVariable("DB_NAME");
    private static readonly string? dbName = Environment.GetEnvironmentVariable("DB_NAME");
    private static readonly string? dbPass = Environment.GetEnvironmentVariable("DB_PASS");

    public static readonly string ConnectionString =
        $"User ID=postgres;Password={dbPass};Host={dbHost};Port=5432;Database={dbName};";

    public static readonly string? BotToken = Environment.GetEnvironmentVariable("BOT_TOKEN");
    public static readonly string? AviaSalesApiToken = Environment.GetEnvironmentVariable("AVIASALES_TOKEN");

    public static readonly string QueryTemplate = @"
{0}: prices_one_way(
    params: {{
        origin: \""{1}\"",
        destination: \""{2}\"",
        depart_date_min: \""{3}\"",
        depart_date_max: \""{4}\"",
        no_lowcost: false,
        direct: {5}
    }},
    grouping: NONE,
    paging: {{
        limit: 3,
        offset: 0
    }},
    sorting: VALUE_ASC
)   {{
        origin_city_iata
        destination_city_iata
        departure_at
		value
		ticket_link
        number_of_changes
    }},
";
}
