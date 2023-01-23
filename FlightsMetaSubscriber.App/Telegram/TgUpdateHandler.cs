using FlightsMetaSubscriber.App.AviasalesAPI;
using FlightsMetaSubscriber.App.Telegram.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlightsMetaSubscriber.App.Telegram;

public class TgUpdateHandler
{
    private readonly Autocomplete _autocomplete;
    private readonly ILogger<TgUpdateHandler> _logger;
    private readonly NewSubscription _newSubscription;
    private readonly Dictionary<long, string> userCommands = new();

    public TgUpdateHandler(ILogger<TgUpdateHandler> logger, Autocomplete autocomplete, NewSubscription newSubscription)
    {
        _logger = logger;
        _autocomplete = autocomplete;
        _newSubscription = newSubscription;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message is not { Text: { } command } message)
            return;

        var userId = message.Chat.Id;
        if (userCommands.ContainsKey(userId))
        {
            command = userCommands[userId];
        }
        else
        {
            userCommands[userId] = command;
        }


        // await botClient.SendTextMessageAsync(userId, "",
        //     replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);

        switch (command)
        {
            case "/newsubscription":
                await _newSubscription.Handle(botClient, message);
                break;
        }
/*
        var chatId = message.Chat.Id;

        _logger.LogInformation("Received a '{@messageText}' message in chat {@chatId}.",
            messageText, chatId);
        var res = await _autocomplete.GetIataCodeByName(messageText);
        var textResults = string.Join(", ", res.Select(o => o.ToString()).ToArray());
        _logger.LogInformation(
            "По запросу {@messageText} от пользователя {@chatId}, найдено: {@textResults}",
            messageText, chatId, textResults);
        var sentMessage = await botClient.SendTextMessageAsync(
            chatId,
            $"По запросу {messageText} найдено:\n{textResults}",
            cancellationToken: cancellationToken);
*/
    }
}
