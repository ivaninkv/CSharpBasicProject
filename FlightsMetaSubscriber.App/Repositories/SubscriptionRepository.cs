using Dapper;
using FlightsMetaSubscriber.App.Models;
using Npgsql;

namespace FlightsMetaSubscriber.App.Repositories;

public static class SubscriptionRepository
{
    public static int GetSubscriptionQty(bool onlyActive = true)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        var query = "select count(*) from subscriptions";
        if (onlyActive)
        {
            query += " where active = true";
        }
        return conn.QueryFirstOrDefault<int>(query);
    }
    
    public static bool DisableSubscription(this Subscription subscription)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string query =
            "update subscription " +
            "set active = false " +
            "where user_id = @user_id " +
            "   and id = @id";
        conn.Execute(query, new
        {
            user_id = subscription.UserId,
            id = subscription.Id
        });

        return true;
    }

    public static List<Subscription> GetOverdueSubscriptions()
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string query =
            "select id Id " +
            ", user_id UserId " +
            ", departure_min_date DepartureMinDate " +
            ", departure_max_date DepartureMaxDate " +
            ", return_min_date ReturnMinDate " +
            ", return_max_date ReturnMaxDate " +
            ", only_direct OnlyDirect " +
            ", baggage Baggage " +
            ", active Active " +
            "from subscription " +
            "where departure_max_date < @max_date " +
            "   and active = true";
        var subscriptions = conn.Query<Subscription>(query, new
        {
            max_date = DateTime.Today
        }).ToList();

        return subscriptions;
    }

    public static List<Subscription> GetByUserId(long userId)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string query =
            "select id Id " +
            ", user_id UserId " +
            ", departure_min_date DepartureMinDate " +
            ", departure_max_date DepartureMaxDate " +
            ", return_min_date ReturnMinDate " +
            ", return_max_date ReturnMaxDate " +
            ", only_direct OnlyDirect " +
            ", baggage Baggage " +
            ", active Active " +
            "from subscription " +
            "where user_id = @user_id " +
            "   and active = true";
        var subscriptions = conn.Query<Subscription>(query, new { user_id = userId }).ToList();

        foreach (var subscription in subscriptions)
        {
            const string originQuery =
                "select o.iata_code Code, io.name Name " +
                "from origin o " +
                "join iata_object io on io.code = o.iata_code " +
                "where o.subscribe_id = @subscription_id";
            subscription.Origin =
                conn.Query<IataObject>(originQuery, new { subscription_id = subscription.Id })
                    .ToList();

            const string destinationQuery =
                "select d.iata_code Code, io.name Name " +
                "from destination d " +
                "join iata_object io on io.code = d.iata_code " +
                "where d.subscribe_id = @subscription_id";
            subscription.Destination =
                conn.Query<IataObject>(destinationQuery, new { subscription_id = subscription.Id })
                    .ToList();
        }

        return subscriptions;
    }

    public static void Save(this Subscription subscription)
    {
        var tgUser = new TgUser(subscription.UserId, true);
        tgUser.Save();

        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string subscriptionQuery =
            "insert into subscription(user_id " +
            ", departure_min_date " +
            ", departure_max_date " +
            ", return_min_date " +
            ", return_max_date " +
            ", only_direct " +
            ", baggage " +
            ", active) " +
            "values (@user_id " +
            ", @departure_min_date " +
            ", @departure_max_date " +
            ", @return_min_date " +
            ", @return_max_date " +
            ", @only_direct " +
            ", @baggage " +
            ", @active) returning id";
        var insertedId = conn.ExecuteScalar<int>(subscriptionQuery, new
        {
            user_id = subscription.UserId,
            departure_min_date = subscription.DepartureMinDate,
            departure_max_date = subscription.DepartureMaxDate,
            return_min_date = subscription.ReturnMinDate,
            return_max_date = subscription.ReturnMaxDate,
            only_direct = subscription.OnlyDirect,
            baggage = subscription.Baggage,
            active = subscription.Active
        });


        const string originQuery = "insert into origin(subscribe_id, user_id, iata_code, iata_name) " +
                                   "values (@subscribe_id, @user_id, @iata_code, @iata_name)";
        foreach (var iataObject in subscription.Origin)
        {
            conn.Execute(originQuery, new
            {
                subscribe_id = insertedId,
                user_id = subscription.UserId,
                iata_code = iataObject.Code,
                iata_name = iataObject.Name
            });
        }

        const string destinationQuery = "insert into destination(subscribe_id, user_id, iata_code, iata_name) " +
                                        "values (@subscribe_id, @user_id, @iata_code, @iata_name)";
        foreach (var iataObject in subscription.Destination)
        {
            conn.Execute(destinationQuery, new
            {
                subscribe_id = insertedId,
                user_id = subscription.UserId,
                iata_code = iataObject.Code,
                iata_name = iataObject.Name
            });
        }
    }
}
