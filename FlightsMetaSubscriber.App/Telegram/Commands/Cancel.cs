using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class Cancel : ICommand
{
    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id,
            "Текущее действие отменено", ParseMode.Markdown,
            replyMarkup: new ReplyKeyboardRemove());

        return true;
    }
}