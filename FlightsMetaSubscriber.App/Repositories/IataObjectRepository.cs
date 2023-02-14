using Dapper;
using FlightsMetaSubscriber.App.Models;
using Npgsql;

namespace FlightsMetaSubscriber.App.Repositories;

public static class IataObjectRepository
{
    public static void Save(this IataObject iataObject)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string query =
            "insert into iata_object (code, name) " +
            "values (@code, @name) " +
            "on conflict (code) do update set name = @name";
        conn.Execute(query, new
        {
            code = iataObject.Code,
            name = iataObject.Name
        });
    }

    public static IataObject GetByCode(string iataCode)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string query =
            "select code Code, name Name " +
            "from iata_object " +
            "where code = @code";
        var iataObject = conn.Query<IataObject>(query, new
        {
            code = iataCode
        }).First();

        return iataObject;
    }
}
