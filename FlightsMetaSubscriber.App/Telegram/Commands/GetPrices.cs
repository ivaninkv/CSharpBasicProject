using FlightsMetaSubscriber.App.Scheduler;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class GetPrices : ICommand
{
    private readonly PricesUpdater _pricesUpdater;

    public GetPrices(PricesUpdater pricesUpdater)
    {
        _pricesUpdater = pricesUpdater;
    }

    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        _pricesUpdater.Invoke();

        return true;
    }
}
