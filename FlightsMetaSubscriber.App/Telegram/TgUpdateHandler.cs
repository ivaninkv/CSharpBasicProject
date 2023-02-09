using FlightsMetaSubscriber.App.AviasalesAPI;
using FlightsMetaSubscriber.App.Telegram.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram;

public class TgUpdateHandler
{
    private readonly ILogger<TgUpdateHandler> _logger;
    private readonly Start _start;
    private readonly Stop _stop;
    private readonly Help _help;
    private readonly GetPrices _getPrices;
    private readonly NewSubscription _newSubscription;
    private readonly MySubscriptions _mySubscriptions;
    private readonly DelSubscription _delSubscription;
    private readonly Dictionary<long, string> userCommands = new();

    public TgUpdateHandler(NewSubscription newSubscription, Start start, MySubscriptions mySubscriptions,
        ILogger<TgUpdateHandler> logger, Help help, GetPrices getPrices, Stop stop, DelSubscription delSubscription)
    {
        _logger = logger;
        _help = help;
        _getPrices = getPrices;
        _stop = stop;
        _delSubscription = delSubscription;
        _start = start;
        _newSubscription = newSubscription;
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

        _logger.LogInformation("Received {@MessageText} message from {@ChatId} chat",
            message.Text, chatId);

        switch (command)
        {
            case "/start":
                try
                {
                    await _start.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogInformation("Command {@command}, exception message: {@message}", "start", e.Message);
                }
                userCommands.Remove(chatId);
                break;
            case "/stop":
                try
                {
                    await _stop.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogInformation("Command {@command}, exception message: {@message}", "stop", e.Message);
                }
                userCommands.Remove(chatId);
                break;
            case "/help":
                try
                {
                    await _help.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogInformation("Command {@command}, exception message: {@message}", "help", e.Message);
                }
                userCommands.Remove(chatId);
                break;
            case "/getprices":
                try
                {
                    await _getPrices.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogInformation("Command {@command}, exception message: {@message}", "getprices", e.Message);
                }
                userCommands.Remove(chatId);
                break;
            case "/newsubscription":
                try
                {
                    var completed = await _newSubscription.Handle(botClient, message);
                    if (completed)
                    {
                        userCommands.Remove(chatId);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation("Command {@command}, exception message: {@message}", "newsubscription", e.Message);
                }

                break;
            case "/mysubscriptions":
                try
                {
                    await _mySubscriptions.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogInformation("Command {@command}, exception message: {@message}", "mysubscriptions", e.Message);
                }
                userCommands.Remove(chatId);
                break;
            case var _ when command.Contains("/delete"):
                try
                {
                    await _delSubscription.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogInformation("Command {@command}, exception message: {@message}", "delete", e.Message);
                }
                userCommands.Remove(chatId);
                break;
            default:
                userCommands.Remove(chatId);
                break;
        }
    }
}
