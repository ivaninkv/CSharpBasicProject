using Dapper;
using FlightsMetaSubscriber.App.Models;
using Npgsql;

namespace FlightsMetaSubscriber.App.Repositories;

public class UserRepository
{
    public static void Save(TgUser user)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string query = $"insert into users (id, active) values (@id, @active)";
        conn.Execute(query, new
        {
            id = user.Id,
            active = user.Active
        });
    }

    public static List<TgUser> GetActiveUsers()
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string query =
            $"select id Id, active Active " +
            $"from users " +
            $"where active = true";
        var result = conn.Query<TgUser>(query).ToList();

        return result;
    }
}
