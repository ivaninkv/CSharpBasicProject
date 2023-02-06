using Coravel.Invocable;
using FlightsMetaSubscriber.App.AviasalesAPI;
using FlightsMetaSubscriber.App.Repositories;

namespace FlightsMetaSubscriber.App.Scheduler;

public class PricesUpdater : IInvocable
{
    private readonly PricesOneWay _pricesOneWay;
    private readonly ILogger<PricesUpdater> _logger;


    public PricesUpdater(PricesOneWay pricesOneWay, ILogger<PricesUpdater> logger)
    {
        _pricesOneWay = pricesOneWay;
        _logger = logger;
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
            }
        }
    }
}
