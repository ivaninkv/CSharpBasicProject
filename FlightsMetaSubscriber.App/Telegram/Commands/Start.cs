using FlightsMetaSubscriber.App.Models;
using FlightsMetaSubscriber.App.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class Start : ICommand
{
    public async Task Handle(ITelegramBotClient botClient, Message message)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id,
            "Привет!\n\n" +
            "Сейчас бот знает следующие команды:\n" +
            "/newsubscription - создать новую подписку\n" +
            "/mysubscription - просмотреть свои подписки");
        UserRepository.Save(new TgUser(message.Chat.Id));
    }
}
