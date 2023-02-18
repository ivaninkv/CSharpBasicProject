using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class UnknownCommand : ICommand
{
    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
         await botClient.SendTextMessageAsync(message.Chat.Id,
             "Неизвестная команда\nДля получения списка команд введите /help");

         return true;
    }
}
