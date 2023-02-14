using FlightsMetaSubscriber.App.Models;
using FlightsMetaSubscriber.App.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class Stop : ICommand
{
    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id,
            "Привет!\n\n" +
            "Жаль, что вы уходите, отключаем ваши подписки.");
        new TgUser(message.Chat.Id, false).Save();

        return true;
    }
}
