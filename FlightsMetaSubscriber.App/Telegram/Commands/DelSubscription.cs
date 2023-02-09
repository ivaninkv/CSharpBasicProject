using FlightsMetaSubscriber.App.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class DelSubscription : ICommand
{
    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        try
        {
            var subscriptionId = int.Parse(message.Text.Split(" ")[1]);
            var subscriptions = SubscriptionRepository.GetByUserId(message.Chat.Id);
            var subscription = subscriptions.First(s => s.Id == subscriptionId);
            SubscriptionRepository.DisableSubscription(subscription);
        }
        catch (Exception e)
        {
            botClient.SendTextMessageAsync(message.Chat.Id,
                "Некорректный ввод\n" +
                "Введите команду в формате:\n" +
                "/delete **number**,\n" +
                "где **number** - ID подписки");
        }

        return true;
    }
}
