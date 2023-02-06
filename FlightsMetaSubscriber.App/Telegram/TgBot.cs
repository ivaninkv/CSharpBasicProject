using FlightsMetaSubscriber.App.AviasalesAPI;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace FlightsMetaSubscriber.App.Telegram;

public class TgBot
{
    public readonly TelegramBotClient BotClient;
    private readonly TgErrorHandler _errorHandling;
    private readonly ILogger<TgBot> _logger;
    private readonly TgUpdateHandler _updateHandler;

    public TgBot(ILogger<TgBot> logger, Autocomplete autocomplete, TgErrorHandler errorHandling,
        TgUpdateHandler updateHandler, string? botToken = null)
    {
        _logger = logger;
        _errorHandling = errorHandling;
        _updateHandler = updateHandler;
        var token = botToken ?? Environment.GetEnvironmentVariable("BOT_TOKEN") ?? "";
        BotClient = new TelegramBotClient(token);
    }

    public async Task RunBot()
    {
        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message }
        };
        BotClient.StartReceiving(
            _updateHandler.HandleUpdateAsync,
            _errorHandling.HandlePollingErrorAsync,
            receiverOptions,
            cts.Token
        );

        var me = await BotClient.GetMeAsync(cts.Token);
        _logger.LogInformation("Start listening for @{@botName}", me.Username);
    }
}
