namespace FlightsMetaSubscriber.App.Models;

public class TgUser
{
    public TgUser(long userId, string? userName)
    {
        Id = userId;
        UserName = userName;
        Active = true;
    }

    public TgUser(long id, bool active)
    {
        Id = id;
        Active = active;
    }

    public TgUser(long userId, string? userName, bool active)
    {
        Id = userId;
        UserName = userName;
        Active = active;
    }

    public long Id { get; }
    public string? UserName { get; set; }
    public bool Active { get; set; }
}
