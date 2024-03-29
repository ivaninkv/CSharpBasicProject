using Dapper;
using FlightsMetaSubscriber.App.Models;
using Npgsql;

namespace FlightsMetaSubscriber.App.Repositories;

public static class UserRepository
{
    public static int GetUsersQty(bool onlyActive = true)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        var query = "select count(*) from users";
        if (onlyActive)
        {
            query += " where active = true";
        }
        
        return conn.QueryFirstOrDefault<int>(query);
    }
    
    public static void Save(this TgUser user)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        var query = user.UserName is not null
            ? "insert into users (id, username, active) " +
              "values (@id, @username, @active) " +
              "on conflict (id) do update set active = @active, username = @username"
            : "insert into users (id, active) " +
              "values (@id, @active) " +
              "on conflict (id) do update set active = @active";
        conn.Execute(query, new
        {
            id = user.Id,
            username = user.UserName,
            active = user.Active
        });

        if (!user.Active)
        {
            const string subscriptionQuery =
                "update subscription " +
                "set active = @active " +
                "where user_id = @user_id";

            conn.Execute(subscriptionQuery, new
            {
                active = user.Active,
                user_id = user.Id
            });
        }
    }

    public static List<TgUser> GetUsers(bool onlyActive = true)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        var query = onlyActive
            ? "select id Id, username UserName, active Active " +
              "from users " +
              "where active = true"
            : "select id Id, username UserName, active Active " +
              "from users";
        var result = conn.Query<TgUser>(query).ToList();

        return result;
    }
}