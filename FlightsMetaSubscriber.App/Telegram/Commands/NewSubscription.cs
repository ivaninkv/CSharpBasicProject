using System.Globalization;
using FlightsMetaSubscriber.App.AviasalesAPI;
using FlightsMetaSubscriber.App.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class NewSubscription : ICommand
{
    private readonly ILogger<TgUpdateHandler> _logger;
    private readonly Autocomplete _autocomplete;
    private readonly Dictionary<long, int> userSteps = new();
    private readonly Dictionary<long, List<IataObject>> userIata = new();
    private DateOnly _departureMinDate;
    private DateOnly _departureMaxDate;
    private readonly Subscription _subscription = new();

    public NewSubscription(ILogger<TgUpdateHandler> logger, Autocomplete autocomplete)
    {
        _logger = logger;
        _autocomplete = autocomplete;
    }

    public async Task Handle(ITelegramBotClient botClient, Message message)
    {
        var chatId = message.Chat.Id;
        var step = userSteps.ContainsKey(chatId) ? userSteps[chatId] : 1;
        if (!userIata.ContainsKey(chatId))
        {
            userIata[chatId] = new List<IataObject>();
        }

        _logger.LogInformation("Received {@MessageText} message from {@UserId} user, step - {@Step}",
            message.Text, chatId, step);

        switch (step)
        {
            case 1:
                await botClient.SendTextMessageAsync(chatId,
                    "Введите город вылета");
                userSteps[chatId] = step + 1;
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
                await botClient.SendTextMessageAsync(chatId, "Выберите пункт отправления из списка",
                    replyMarkup: step2Keyboard);
                userSteps[chatId] = step + 1;
                break;
            case 3:
                var depCity = IataObject.GetObjectByString(message.Text);
                userIata[chatId].Add(depCity);
                ReplyKeyboardMarkup step3Keyboard = new(new[]
                    {
                        new KeyboardButton[] { "Добавить город вылета", "Перейти к вводу дат" },
                    })
                    { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(chatId, "Выберите следующее действие",
                    replyMarkup: step3Keyboard);
                userSteps[chatId] = step + 1;
                break;
            case 4:
                if (message.Text.Equals("Добавить город вылета"))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Введите город вылета",
                        replyMarkup: new ReplyKeyboardRemove());
                    userSteps[chatId] = 2;
                }
                else if (message.Text.Equals("Перейти к вводу дат"))
                {
                    _subscription.Origin = userIata[chatId];
                    userIata[chatId] = new List<IataObject>();

                    await botClient.SendTextMessageAsync(chatId,
                        "Введите диапазон дат в формате: 'dd.MM.yyyy-dd.MM.yyyy'\n" +
                        "Например: <code>01.05.2023-14.05.2023</code>", ParseMode.Html,
                        replyMarkup: new ReplyKeyboardRemove());
                    userSteps[chatId] = step + 1;
                }

                break;
            case 5:
                var split = message.Text.Split("-")
                    .Select(s => s.Trim()).ToArray();
                if (!DateOnly.TryParseExact(split[0], "dd.MM.yyyy", out _departureMinDate) ||
                    !DateOnly.TryParseExact(split[1], "dd.MM.yyyy", out _departureMaxDate))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Некорректно введены даты.\n\n" +
                        "Введите диапазон дат в формате: 'dd.MM.yyyy-dd.MM.yyyy'\n" +
                        "Например: <code>01.05.2023-14.05.2023</code>", ParseMode.Html);
                    break;
                }

                _subscription.DepartureMinDate = _departureMinDate;
                _subscription.DepartureMaxDate = _departureMaxDate;

                botClient.SendTextMessageAsync(chatId, "Введите город прибытия");

                userSteps[chatId] = step + 1;
                break;
            case 6:
                var res6Step = (await _autocomplete.GetIataCodeByName(message.Text))
                    .Select(result => result.ToString())
                    .ToArray();

                var keyboard6Markup = new KeyboardButton[res6Step.Length][];
                for (var i = 0; i < res6Step.Length; i++)
                {
                    keyboard6Markup[i] = new[] { new KeyboardButton(res6Step[i]) };
                }

                ReplyKeyboardMarkup step6Keyboard = new(keyboard6Markup) { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(chatId, "Выберите пункт прибытия из списка",
                    replyMarkup: step6Keyboard);
                userSteps[chatId] = step + 1;
                break;
            case 7:
                var arrCity = IataObject.GetObjectByString(message.Text);
                userIata[chatId].Add(arrCity);
                ReplyKeyboardMarkup step7Keyboard = new(new[]
                    {
                        new KeyboardButton[] { "Добавить город прибытия", "Завершить ввод" },
                    })
                    { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(chatId, "Выберите следующее действие",
                    replyMarkup: step7Keyboard);
                userSteps[chatId] = step + 1;
                break;
            case 8:
                if (message.Text.Equals("Добавить город прибытия"))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Добавить город прибытия",
                        replyMarkup: new ReplyKeyboardRemove());
                    userSteps[chatId] = 6;
                }
                else if (message.Text.Equals("Завершить ввод"))
                {
                    _subscription.Destination = userIata[chatId];
                    userIata[chatId] = new List<IataObject>();

                    ReplyKeyboardMarkup step9Keyboard = new(new[]
                        {
                            new KeyboardButton[] { "OK", "Cancel" },
                        })
                        { ResizeKeyboard = true };
                    await botClient.SendTextMessageAsync(chatId, "Подтвердите параметры подписки");
                    await botClient.SendTextMessageAsync(chatId, _subscription.ToString(),
                        replyMarkup: step9Keyboard);

                    userSteps[chatId] = step + 1;
                }

                break;
            case 9:
                switch (message.Text)
                {
                    case "OK":
                        // TODO - сохранить подписку в БД
                        await botClient.SendTextMessageAsync(chatId,
                            "Подписка сохранена",
                            replyMarkup: new ReplyKeyboardRemove());
                        userSteps.Remove(chatId);
                        break;
                    case "Cancel":
                        await botClient.SendTextMessageAsync(chatId,
                            "Ввод отменен",
                            replyMarkup: new ReplyKeyboardRemove());
                        userSteps.Remove(chatId);
                        break;
                }

                break;
        }
    }
}
