using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public interface ICommand
{
    public async Task Handle(Message message)
    {
    }
}
