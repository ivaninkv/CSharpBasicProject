using Serilog;

namespace FlightsMetaSubscriber.App;

public static class Config
{
    private static readonly Serilog.ILogger _logger = Log.ForContext(typeof(Config));
    private static readonly string? dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    private static readonly string? dbName = Environment.GetEnvironmentVariable("DB_NAME");
    private static readonly string? dbPass = Environment.GetEnvironmentVariable("DB_PASS");

    public static readonly string ConnectionString =
        $"User ID=postgres;Password={dbPass};Host={dbHost};Port=5432;Database={dbName};";

    public static readonly string? BotToken = Environment.GetEnvironmentVariable("BOT_TOKEN");
    public static readonly string? AviaSalesApiToken = Environment.GetEnvironmentVariable("AVIASALES_TOKEN");
    public static readonly List<long> AdminIds = GetAdminIds();

    public static readonly string? LogDirectory = Environment.GetEnvironmentVariable("LOG_DIR") ??
                                                  throw new InvalidOperationException(
                                                      "You can provide ENV VAR - LOG_DIR");

    public static readonly string PricesOneWayQueryTemplate = @"
{0}: prices_one_way(
    params: {{
        origin: \""{1}\"",
        destination: \""{2}\"",
        depart_date_min: \""{3}\"",
        depart_date_max: \""{4}\"",
        no_lowcost: false,
        convenient: true,
        direct: {5},
        with_baggage: {6}
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
        with_baggage
    }},
";

    public static readonly string PricesRoundTripQueryTemplate = @"
{0}: prices_round_trip(
    params: {{
        origin: \""{1}\"",
        destination: \""{2}\"",
        depart_date_min: \""{3}\"",
        depart_date_max: \""{4}\"",
        return_date_min: \""{5}\"",
        return_date_max: \""{6}\"",
        no_lowcost: false,
        convenient: true,
        direct: {7},
        with_baggage: {8}
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
        return_at
		value
		ticket_link
        number_of_changes
        with_baggage
    }},
";


    private static List<long> GetAdminIds()
    {
        try
        {
            var adminIds = Environment.GetEnvironmentVariable("ADMIN_IDS")
                .Split(",")
                .Select(long.Parse)
                .ToList();

            _logger.Debug("Admin ids is {@adminIds}", adminIds);

            return adminIds;
        }
        catch (Exception e)
        {
            _logger.Warning("Can't parse ADMIN_IDS, error - {@Error}", e.Message);
        }

        return new List<long>();
    }
}