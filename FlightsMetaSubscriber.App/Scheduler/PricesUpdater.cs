using Coravel.Invocable;
using FlightsMetaSubscriber.App.AviasalesAPI;
using FlightsMetaSubscriber.App.Models;
using FlightsMetaSubscriber.App.Repositories;
using FlightsMetaSubscriber.App.Telegram;
using Telegram.Bot;

namespace FlightsMetaSubscriber.App.Scheduler;

public class PricesUpdater : IInvocable
{
    private readonly PricesOneWay _pricesOneWay;
    private readonly ILogger<PricesUpdater> _logger;
    private readonly TgBot _tgBot;

    public PricesUpdater(PricesOneWay pricesOneWay, ILogger<PricesUpdater> logger, TgBot tgBot)
    {
        _pricesOneWay = pricesOneWay;
        _logger = logger;
        _tgBot = tgBot;
    }

    public async Task Invoke()
    {
        _logger.LogInformation("Starting price update");

        var tgUsers = UserRepository.GetActiveUsers();

        _logger.LogInformation("{@qty} active users found", tgUsers.Count);

        foreach (var tgUser in tgUsers)
        {
            var userSubscriptions = SubscriptionRepository.GetByUserId(tgUser.Id);

            _logger.LogInformation("{@qty} subscriptions found", userSubscriptions.Count);

            foreach (var subscription in userSubscriptions)
            {
                var pricesForSubscription = await _pricesOneWay.FindPricesForSubscription(subscription);

                _logger.LogInformation("Found {@qty} search results", pricesForSubscription.Count);

                SearchResultRepository.SaveAll(pricesForSubscription);

                _logger.LogInformation("Price update completed");

                SendMinPriceBySubscription(subscription, pricesForSubscription);
            }
        }
    }

    private async Task SendMinPriceBySubscription(Subscription subscription, List<SearchResult> searchResults)
    {
        var minResult = searchResults.OrderBy(result => result.Value).FirstOrDefault(new SearchResult());
        if (minResult.Value > 0)
        {
            await _tgBot.BotClient.SendTextMessageAsync(subscription.UserId, "Результаты поиска по подписке:");
            await _tgBot.BotClient.SendTextMessageAsync(subscription.UserId, subscription.ToString());
            await _tgBot.BotClient.SendTextMessageAsync(subscription.UserId, minResult.ToString());
        }

    }
}
