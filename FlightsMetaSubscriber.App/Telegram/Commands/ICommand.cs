using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public interface ICommand
{
    public Task<bool> Handle(ITelegramBotClient botClient, Message message);
}
