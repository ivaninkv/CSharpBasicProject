using FlightsMetaSubscriber.App.AviasalesAPI;
using FlightsMetaSubscriber.App.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class MySubscriptions : ICommand
{
    private readonly PricesOneWay _pricesOneWay;

    public MySubscriptions(PricesOneWay pricesOneWay)
    {
        _pricesOneWay = pricesOneWay;
    }

    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        var chatId = message.Chat.Id;
        var subscriptions = SubscriptionRepository.GetByUserId(chatId);
        foreach (var subscription in subscriptions)
        {
            await botClient.SendTextMessageAsync(chatId, subscription.ToString());
            await _pricesOneWay.GetMinPrices(subscription);
        }

        return true;
    }
}
