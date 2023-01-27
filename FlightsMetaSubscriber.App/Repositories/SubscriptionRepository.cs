using Dapper;
using FlightsMetaSubscriber.App.Models;
using Npgsql;

namespace FlightsMetaSubscriber.App.Repositories;

public class SubscriptionRepository
{
    public static List<Subscription> GetByUserId(long userId)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string query =
            $"select id Id, user_id UserId, departure_min_date DepartureMinDate, departure_max_date DepartureMaxDate " +
            $"from subscription " +
            $"where user_id = @user_id";
        var subscriptions = conn.Query<Subscription>(query, new { user_id = userId }).ToList();

        foreach (var subscription in subscriptions)
        {
            const string originQuery =
                $"select iata_code Code, iata_name Name " +
                $"from origin " +
                $"where subscribe_id = @subscription_id";
            subscription.Origin =
                conn.Query<IataObject>(originQuery, new { subscription_id = subscription.Id })
                    .ToList();

            const string destinationQuery =
                $"select iata_code Code, iata_name Name " +
                $"from destination " +
                $"where subscribe_id = @subscription_id";
            subscription.Destination =
                conn.Query<IataObject>(destinationQuery, new { subscription_id = subscription.Id })
                    .ToList();
        }

        return subscriptions;
    }

    public static void Save(Subscription subscription)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string subscriptionQuery =
            $"insert into subscription(user_id, departure_min_date, departure_max_date) " +
            $"values (@user_id, @departure_min_date, @departure_max_date) returning id";
        var insertedId = conn.ExecuteScalar<int>(subscriptionQuery, new
        {
            user_id = subscription.UserId,
            departure_min_date = subscription.DepartureMinDate,
            departure_max_date = subscription.DepartureMaxDate
        });


        const string originQuery = $"insert into origin(subscribe_id, user_id, iata_code, iata_name) " +
                                   $"values (@subscribe_id, @user_id, @iata_code, @iata_name)";
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

        const string destinationQuery = $"insert into destination(subscribe_id, user_id, iata_code, iata_name) " +
                                        $"values (@subscribe_id, @user_id, @iata_code, @iata_name)";
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
