using FlightsMetaSubscriber.App.Telegram.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram;

public class TgUpdateHandler
{
    private readonly string _commandLogTemplate = "Command {@command}, exception message: {@message}";
    private readonly ILogger<TgUpdateHandler> _logger;
    private readonly Start _start;
    private readonly Stop _stop;
    private readonly Help _help;
    private readonly GetPrices _getPrices;
    private readonly NewSubscription _newSubscription;
    private readonly MySubscriptions _mySubscriptions;
    private readonly DelSubscription _delSubscription;
    private readonly UnknownCommand _unknownCommand;
    private readonly News _news;
    private readonly Feedback _feedback;
    private readonly Users _users;
    private readonly Dictionary<long, string> _userCommands = new();

    public TgUpdateHandler(
        NewSubscription newSubscription, 
        Start start, 
        MySubscriptions mySubscriptions,
        ILogger<TgUpdateHandler> logger, 
        Help help, 
        GetPrices getPrices, 
        Stop stop, 
        DelSubscription delSubscription,
        UnknownCommand unknownCommand,
        News news, 
        Feedback feedback, 
        Users users)
    {
        _logger = logger;
        _help = help;
        _getPrices = getPrices;
        _stop = stop;
        _delSubscription = delSubscription;
        _unknownCommand = unknownCommand;
        _start = start;
        _newSubscription = newSubscription;
        _mySubscriptions = mySubscriptions;
        _news = news;
        _feedback = feedback;
        _users = users;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message is not { Text: { } command } message)
            return;

        var chatId = message.Chat.Id;
        if (_userCommands.ContainsKey(chatId))
        {
            command = _userCommands[chatId];
        }
        else
        {
            _userCommands[chatId] = command;
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
                    _logger.LogWarning(_commandLogTemplate, command, e.Message);
                }

                _userCommands.Remove(chatId);
                break;
            case "/stop":
                try
                {
                    await _stop.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(_commandLogTemplate, command, e.Message);
                }

                _userCommands.Remove(chatId);
                break;
            case "/help":
                try
                {
                    await _help.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(_commandLogTemplate, command, e.Message);
                }

                _userCommands.Remove(chatId);
                break;
            case "/getprices":
                try
                {
                    await _getPrices.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(_commandLogTemplate, command, e.Message);
                }

                _userCommands.Remove(chatId);
                break;
            case "/newsubscription":
                try
                {
                    var completed = await _newSubscription.Handle(botClient, message);
                    if (completed)
                    {
                        _userCommands.Remove(chatId);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogWarning(_commandLogTemplate, command, e.Message);
                }

                break;
            case "/mysubscriptions":
                try
                {
                    await _mySubscriptions.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(_commandLogTemplate, command, e.Message);
                }

                _userCommands.Remove(chatId);
                break;
            case var _ when command.Contains("/delete"):
                try
                {
                    await _delSubscription.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(_commandLogTemplate, command, e.Message);
                }

                _userCommands.Remove(chatId);
                break;
            case var _ when command.Contains("/news"):
                try
                {
                    await _news.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(_commandLogTemplate, command, e.Message);
                }

                _userCommands.Remove(chatId);
                break;
            case "/feedback":
                try
                {
                    var completed = await _feedback.Handle(botClient, message);
                    if (completed)
                    {
                        _userCommands.Remove(chatId);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogWarning(_commandLogTemplate, command, e.Message);
                }
                break;
            case "/users":
                try
                {
                    await _users.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(_commandLogTemplate, command, e.Message);
                }

                _userCommands.Remove(chatId);
                break;
            default:
                try
                {
                    await _unknownCommand.Handle(botClient, message);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(_commandLogTemplate, command, e.Message);
                }

                _userCommands.Remove(chatId);
                break;
        }
    }
}