using FlightsMetaSubscriber.App.AviasalesAPI;
using FlightsMetaSubscriber.App.Telegram.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram;

public class TgUpdateHandler
{
    private readonly Start _start;
    private readonly NewSubscription _newSubscription;
    private readonly MySubscriptions _mySubscriptions;
    private readonly Dictionary<long, string> userCommands = new();

    public TgUpdateHandler(NewSubscription newSubscription, Start start, MySubscriptions mySubscriptions)
    {
        _newSubscription = newSubscription;
        _start = start;
        _mySubscriptions = mySubscriptions;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message is not { Text: { } command } message)
            return;

        var chatId = message.Chat.Id;
        if (userCommands.ContainsKey(chatId))
        {
            command = userCommands[chatId];
        }
        else
        {
            userCommands[chatId] = command;
        }

        switch (command)
        {
            case "/start":
                await _start.Handle(botClient, message);
                userCommands.Remove(chatId);
                break;
            case "/newsubscription":
                await _newSubscription.Handle(botClient, message);
                break;
            case "/mysubscription":
                await _mySubscriptions.Handle(botClient, message);
                userCommands.Remove(chatId);
                break;
        }
    }
}
