using FlightsMetaSubscriber.App.Models;
using FlightsMetaSubscriber.App.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class Start : ICommand
{
    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id,
            "Привет!\n\n" +
            "Сейчас бот знает следующие команды:\n" +
            "/newsubscription - создать новую подписку\n" +
            "/mysubscriptions - просмотреть свои подписки\n" +
            "/getprices - запустить поиск цен\n" +
            "/stop - отписаться от всех подписок");
        new TgUser(message.Chat.Id, message.Chat.Username).Save();

        return true;
    }
}
