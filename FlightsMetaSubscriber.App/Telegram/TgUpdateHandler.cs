using FlightsMetaSubscriber.App.AviasalesAPI;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram;

public class TgUpdateHandler
{
    private readonly Autocomplete _autocomplete;
    private readonly ILogger<TgUpdateHandler> _logger;

    public TgUpdateHandler(ILogger<TgUpdateHandler> logger, Autocomplete autocomplete)
    {
        _logger = logger;
        _autocomplete = autocomplete;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
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
}
