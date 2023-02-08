using Coravel.Invocable;
using FlightsMetaSubscriber.App.AviasalesAPI;
using FlightsMetaSubscriber.App.Models;
using FlightsMetaSubscriber.App.Repositories;
using FlightsMetaSubscriber.App.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace FlightsMetaSubscriber.App.Scheduler;

public class PricesUpdater : IInvocable
{
    private readonly PricesOneWay _pricesOneWay;
    private readonly ILogger<PricesUpdater> _logger;
    private readonly TgBotClient _tgBotClient;

    public PricesUpdater(PricesOneWay pricesOneWay, ILogger<PricesUpdater> logger, TgBotClient tgBotClient)
    {
        _pricesOneWay = pricesOneWay;
        _logger = logger;
        _tgBotClient = tgBotClient;
    }

    public async Task Invoke()
    {
        _logger.LogInformation("Starting price update");

        var tgUsers = UserRepository.GetActiveUsers();

        _logger.LogInformation("{@qty} active users found", tgUsers.Count);

        foreach (var tgUser in tgUsers)
        {
            await UpdatePricesByUser(tgUser.Id);
        }
    }

    public async Task UpdatePricesByUser(long chatId)
    {
        var userSubscriptions = SubscriptionRepository.GetByUserId(chatId);

        _logger.LogInformation("{@qty} subscriptions found of {@user} user",
            userSubscriptions.Count, chatId);

        foreach (var subscription in userSubscriptions)
        {
            var pricesForSubscription = await _pricesOneWay.FindPricesForSubscription(subscription);

            _logger.LogInformation("Found {@qty} search results", pricesForSubscription.Count);

            SearchResultRepository.SaveAll(pricesForSubscription);

            _logger.LogInformation("Price update completed");

            SendMinPriceBySubscription(subscription, pricesForSubscription);
        }
    }

    private async Task SendMinPriceBySubscription(Subscription subscription, List<SearchResult> searchResults)
    {
        var minResult = searchResults.OrderBy(result => result.Value).FirstOrDefault(new SearchResult());
        if (minResult.Value > 0)
        {
            await _tgBotClient.BotClient.SendTextMessageAsync(subscription.UserId, "Результаты поиска по подписке:");
            await _tgBotClient.BotClient.SendTextMessageAsync(subscription.UserId, subscription.ToString());
            _logger.LogDebug("Search result - {@minResult}", minResult.ToString());
            await _tgBotClient.BotClient.SendTextMessageAsync(subscription.UserId, minResult.ToString(), ParseMode.Markdown);
        }
        else
        {
            await _tgBotClient.BotClient.SendTextMessageAsync(subscription.UserId, "Билетов по вашей подписке не найдено");
            await _tgBotClient.BotClient.SendTextMessageAsync(subscription.UserId, subscription.ToString());
        }

    }
}
