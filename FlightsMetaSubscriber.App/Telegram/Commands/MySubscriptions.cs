using FlightsMetaSubscriber.App.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class MySubscriptions : ICommand
{
    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        var chatId = message.Chat.Id;
        var subscriptions = SubscriptionRepository.GetByUserId(chatId);
        foreach (var subscription in subscriptions)
        {
            await botClient.SendTextMessageAsync(chatId, subscription.ToString(), ParseMode.Markdown);
        }

        if (subscriptions.Count == 0)
        {
            await botClient.SendTextMessageAsync(chatId, "У вас пока нет подписок");
        }

        return true;
    }
}
