using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace FlightsMetaSubscriber.App.Telegram;

public class TgErrorHandler
{
    private readonly ILogger<TgErrorHandler> _logger;

    public TgErrorHandler(ILogger<TgErrorHandler> logger)
    {
        _logger = logger;
    }

    public Task HandlePollingErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
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
