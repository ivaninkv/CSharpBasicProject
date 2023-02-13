using Dapper;
using FlightsMetaSubscriber.App.Models;
using Npgsql;

namespace FlightsMetaSubscriber.App.Repositories;

public class UserRepository
{
    public static void Save(TgUser user)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string query = "insert into users (id, username, active) " +
                             "values (@id, @username, @active) " +
                             "on conflict (id) do update set active = @active";
        conn.Execute(query, new
        {
            id = user.Id,
            username = user.UserName,
            active = user.Active
        });

        if (!user.Active)
        {
            const string subscriptionQuery = "update subscription " +
                                             "set active = @active " +
                                             "where user_id = @user_id";
            conn.Execute(subscriptionQuery, new
            {
                active = user.Active,
                user_id = user.Id
            });
        }
    }

    public static List<TgUser> GetActiveUsers()
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string query =
            "select id Id, username UserName, active Active " +
            "from users " +
            "where active = true";
        var result = conn.Query<TgUser>(query).ToList();

        return result;
    }
}
