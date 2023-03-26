using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace FlightsMetaSubscriber.App.Telegram;

public class TgBot
{
    private readonly TgBotClient _tgBotClient;
    private readonly TgErrorHandler _errorHandling;
    private readonly ILogger<TgBot> _logger;
    private readonly TgUpdateHandler _updateHandler;

    public TgBot(
        ILogger<TgBot> logger,
        TgErrorHandler errorHandling,
        TgUpdateHandler updateHandler,
        TgBotClient tgBotClient)
    {
        _logger = logger;
        _errorHandling = errorHandling;
        _updateHandler = updateHandler;
        _tgBotClient = tgBotClient;
    }

    public async Task RunBot()
    {
        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message }
        };
        _tgBotClient.BotClient.StartReceiving(
            _updateHandler.HandleUpdateAsync,
            _errorHandling.HandlePollingErrorAsync,
            receiverOptions,
            cts.Token
        );

        var me = await _tgBotClient.BotClient.GetMeAsync(cts.Token);
        _logger.LogInformation("Start listening for @{@botName}", me.Username);
    }

    public static bool CheckAdmin(long chatId)
    {
        return Config.AdminIds.Contains(chatId);
    }
    
}
