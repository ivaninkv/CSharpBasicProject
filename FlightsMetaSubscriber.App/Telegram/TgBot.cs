using FlightsMetaSubscriber.App.AviasalesAPI;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlightsMetaSubscriber.App.Telegram;

public class TgBot
{
    private readonly Autocomplete _autocomplete;
    private readonly TelegramBotClient _botClient;
    private readonly ILogger<TgBot> _logger;

    public TgBot(ILogger<TgBot> logger, Autocomplete autocomplete, string? botToken = null)
    {
        _logger = logger;
        _autocomplete = autocomplete;
        var token = botToken ?? Environment.GetEnvironmentVariable("BOT_TOKEN") ?? "";
        _botClient = new TelegramBotClient(token);
    }

    public async Task RunBot()
    {
        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message }
        };
        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            receiverOptions,
            cts.Token
        );

        var me = await _botClient.GetMeAsync(cts.Token);
        _logger.LogInformation($"Start listening for @{me.Username}");
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message is not { Text: { } messageText } message)
            return;

        var chatId = message.Chat.Id;

        _logger.LogInformation($"Received a '{messageText}' message in chat {chatId}.");
        var res = await _autocomplete.GetIataCodeByName(messageText);
        var textResults = string.Join(", ", res.Select(o => o.ToString()).ToArray());
        _logger.LogInformation(
            $"По запросу {messageText} от пользователя {chatId}, найдено: {textResults}");
        var sentMessage = await botClient.SendTextMessageAsync(
            chatId,
            $"По запросу {messageText} найдено:\n{textResults}",
            cancellationToken: cancellationToken);
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogError(errorMessage);
        return Task.CompletedTask;
    }
}
