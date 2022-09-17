using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FlightsMetaSubscriber.App.Telegram;

public class TgBot
{
    private readonly TelegramBotClient _botClient;

    public TgBot(TelegramBotClient? botClient = null)
    {
        var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN") ?? "";
        _botClient = botClient ?? new TelegramBotClient(botToken);
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

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { Text: { } messageText } message)
                return;

            var chatId = message.Chat.Id;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            var sentMessage = await botClient.SendTextMessageAsync(
                chatId,
                "Вы написали:\n" + messageText,
                cancellationToken: cancellationToken);
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}
