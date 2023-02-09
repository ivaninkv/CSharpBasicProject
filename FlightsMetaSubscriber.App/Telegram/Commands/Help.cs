using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class Help : ICommand
{
    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id,
            "Сейчас бот знает следующие команды:\n" +
            "/newsubscription - создать новую подписку\n" +
            "/mysubscriptions - просмотреть свои подписки\n" +
            "/delete *number* - удалить подписку под номером *number*\n" +
            "/getprices - запустить поиск цен\n" +
            "/stop - отписаться от всех подписок", ParseMode.Markdown);

        return true;
    }
}
