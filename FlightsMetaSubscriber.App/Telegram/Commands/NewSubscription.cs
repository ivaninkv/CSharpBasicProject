using FlightsMetaSubscriber.App.AviasalesAPI;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class NewSubscription : ICommand
{
    private readonly ILogger<TgUpdateHandler> _logger;
    private readonly Autocomplete _autocomplete;
    private readonly Dictionary<long, int> userSteps = new();

    public NewSubscription(ILogger<TgUpdateHandler> logger, Autocomplete autocomplete)
    {
        _logger = logger;
        _autocomplete = autocomplete;
    }

    public async Task Handle(ITelegramBotClient botClient, Message message)
    {
        var step = userSteps.ContainsKey(message.Chat.Id) ? userSteps[message.Chat.Id] : 1;

        _logger.LogInformation("Received {@MessageText} message from {@UserId} user, step - {@Step}",
            message.Text, message.Chat.Id, step);

        switch (step)
        {
            case 1:
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Введите город вылета");
                userSteps[message.Chat.Id] = step + 1;
                break;
            case 2:
                var res = (await _autocomplete.GetIataCodeByName(message.Text))
                    .Select(result => result.ToString())
                    .ToArray();

                var keyboardMarkup = new KeyboardButton[res.Length][];
                for (var i = 0; i < res.Length; i++)
                {
                    keyboardMarkup[i] = new[] { new KeyboardButton(res[i]) };
                }

                ReplyKeyboardMarkup step2Keyboard = new(keyboardMarkup) { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите пункт отправления из списка",
                    replyMarkup: step2Keyboard);
                userSteps[message.Chat.Id] = step + 1;
                break;
            case 3:
                ReplyKeyboardMarkup step3Keyboard = new(new[]
                    {
                        new KeyboardButton[] { "Добавить город вылета", "Перейти к вводу дат" },
                    })
                    { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите следующее действие",
                    replyMarkup: step3Keyboard);
                userSteps[message.Chat.Id] = step + 1;
                break;
            case 4:
                if (message.Text.Equals("Добавить город вылета"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Введите город вылета");
                    userSteps[message.Chat.Id] = 2;
                }
                else if (message.Text.Equals("Перейти к вводу дат"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Введите диапазон дат в формате: 'dd.MM.yyyy-dd.MM.yyyy'\n" +
                        "Например - `01.05.2023-14.05.2023`");
                    userSteps[message.Chat.Id] = step + 1;
                }

                break;
            case 5:
                userSteps[message.Chat.Id] = step + 1;
                break;
        }
    }
}
