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
            "/cancel - отменить ввод подписки и начать с начала\n" +
            "/getprices - запустить поиск цен\n" +
            "/delete *number* - удалить подписку под номером *number*\n" +
            "/feedback - написать разработчикам\n" +
            "/help - показать список команд\n" +
            "/stop - отписаться от всех подписок", ParseMode.Markdown);

        return true;
    }
}