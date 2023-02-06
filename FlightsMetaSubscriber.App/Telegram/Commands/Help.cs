using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class Help : ICommand
{
    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id,
            "Сейчас бот знает следующие команды:\n" +
            "/newsubscription - создать новую подписку\n" +
            "/mysubscriptions - просмотреть свои подписки");

        return true;
    }
}
