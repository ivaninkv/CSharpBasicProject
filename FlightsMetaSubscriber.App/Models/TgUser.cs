namespace FlightsMetaSubscriber.App.Models;

public class TgUser
{
    public TgUser(long userId)
    {
        Id = userId;
        Active = true;
    }

    public long Id { get; }
    public bool Active { get; set; }
}
