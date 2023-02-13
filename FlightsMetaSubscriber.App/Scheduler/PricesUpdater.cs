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

        var tgUsers = UserRepository.GetUsers(onlyActive: true);

        _logger.LogInformation("{@qty} active users found", tgUsers.Count);

        foreach (var tgUser in tgUsers)
        {
            try
            {
                await UpdatePricesByUser(tgUser.Id);
            }
            catch (Exception e)
            {
                _logger.LogWarning("PricesUpdater for {@User} has error: {@Error}",
                    tgUser.Id, e.Message);
            }
        }
    }

    public async Task UpdatePricesByUser(long chatId)
    {
        var userSubscriptions = SubscriptionRepository.GetByUserId(chatId);

        _logger.LogInformation("{@qty} subscriptions found of {@user} user",
            userSubscriptions.Count, chatId);

        double sumSubscriptions = 0;

        foreach (var subscription in userSubscriptions)
        {
            try
            {
                var pricesForSubscription = await _pricesOneWay.FindPricesForSubscription(subscription);
                var minResult = pricesForSubscription.OrderBy(result => result.Value)
                    .FirstOrDefault(new SearchResult());

                _logger.LogInformation("Found {@qty} search results", pricesForSubscription.Count);

                SearchResultRepository.SaveAll(pricesForSubscription);
                SendMinPriceBySubscription(subscription, minResult);
                sumSubscriptions += minResult.Value;

                _logger.LogInformation("Price update completed");
            }
            catch (Exception e)
            {
                _logger.LogWarning("UpdatePricesByUser for {@User} user, " +
                                   "subscription {@Subscription} has error: {@Error}",
                    subscription.UserId, subscription.Id, e.Message);
            }
        }

        if (sumSubscriptions > 0)
        {
            await _tgBotClient.BotClient.SendTextMessageAsync(chatId,
                $"Сумма билетов по всем вашим подпискам = {sumSubscriptions}");
        }

        if (userSubscriptions.Count == 0)
        {
            await _tgBotClient.BotClient.SendTextMessageAsync(chatId, "У вас пока нет подписок");
        }
    }

    private async Task SendMinPriceBySubscription(Subscription subscription, SearchResult minResult)
    {
        if (minResult.Value > 0)
        {
            var message = "Результаты поиска по подписке:\n" +
                          $"{subscription}" +
                          "\n============\n" +
                          $"{minResult}";

            _logger.LogDebug("Search result - {@minResult}", minResult.ToString());
            await _tgBotClient.BotClient.SendTextMessageAsync(subscription.UserId, message, ParseMode.Markdown);
        }
        else
        {
            await _tgBotClient.BotClient.SendTextMessageAsync(subscription.UserId,
                $"Билетов по вашей подписке не найдено\n{subscription}");
        }
    }
}
