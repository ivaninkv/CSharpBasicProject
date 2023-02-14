using System.Globalization;
using FlightsMetaSubscriber.App.AviasalesAPI;
using FlightsMetaSubscriber.App.Models;
using FlightsMetaSubscriber.App.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class NewSubscription : ICommand
{
    private readonly ILogger<TgUpdateHandler> _logger;
    private readonly Autocomplete _autocomplete;
    private readonly Dictionary<long, int> _userSteps = new();
    private readonly Dictionary<long, Subscription> _userSubscription = new();
    private DateTime _departureMinDate;
    private DateTime _departureMaxDate;

    public NewSubscription(ILogger<TgUpdateHandler> logger, Autocomplete autocomplete)
    {
        _logger = logger;
        _autocomplete = autocomplete;
    }

    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        var chatId = message.Chat.Id;
        var step = _userSteps.ContainsKey(chatId) ? _userSteps[chatId] : 1;
        if (!_userSubscription.ContainsKey(chatId))
        {
            _userSubscription[chatId] = new Subscription(chatId);
        }

        switch (step)
        {
            case 1:
                await botClient.SendTextMessageAsync(chatId,
                    "Введите город вылета");
                _userSteps[chatId] = step + 1;
                break;
            case 2:
                var res = (await _autocomplete.GetIataCodeByName(message.Text))
                    .Select(result => result.ToString())
                    .ToArray();

                if (res.Length > 0)
                {
                    var keyboardMarkup = new KeyboardButton[res.Length][];
                    for (var i = 0; i < res.Length; i++)
                    {
                        keyboardMarkup[i] = new[] { new KeyboardButton(res[i]) };
                    }

                    ReplyKeyboardMarkup step2Keyboard = new(keyboardMarkup) { ResizeKeyboard = true };
                    await botClient.SendTextMessageAsync(chatId, "Выберите пункт отправления из списка",
                        replyMarkup: step2Keyboard);
                    _userSteps[chatId] = step + 1;
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Ничего не найдено, попробуйте еще раз");
                }

                break;
            case 3:
                var depCity = IataObject.FromString(message.Text);
                if (!_userSubscription[chatId].Origin.Contains(depCity))
                {
                    _userSubscription[chatId].Origin.Add(depCity);
                    depCity.Save();
                }

                ReplyKeyboardMarkup step3Keyboard = new(new[]
                    {
                        new KeyboardButton[] { "Добавить город вылета", "Перейти к вводу дат" },
                    })
                    { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(chatId, "Выберите следующее действие",
                    replyMarkup: step3Keyboard);
                _userSteps[chatId] = step + 1;
                break;
            case 4:
                if (message.Text.Equals("Добавить город вылета"))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Введите город вылета",
                        replyMarkup: new ReplyKeyboardRemove());
                    _userSteps[chatId] = 2;
                }
                else if (message.Text.Equals("Перейти к вводу дат"))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Введите диапазон дат в формате: 'dd.MM.yyyy-dd.MM.yyyy'\n" +
                        "Например: <code>01.05.2023-14.05.2023</code>", ParseMode.Html,
                        replyMarkup: new ReplyKeyboardRemove());
                    _userSteps[chatId] = step + 1;
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Некорректный ввод");
                }

                break;
            case 5:
                var split = message.Text.Split("-")
                    .Select(s => s.Trim()).ToArray();
                if (!DateTime.TryParseExact(split[0], "dd.MM.yyyy", null, DateTimeStyles.None, out _departureMinDate) ||
                    !DateTime.TryParseExact(split[1], "dd.MM.yyyy", null, DateTimeStyles.None, out _departureMaxDate))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Некорректно введены даты.\n\n" +
                        "Введите диапазон дат в формате: 'dd.MM.yyyy-dd.MM.yyyy'\n" +
                        "Например: <code>01.05.2023-14.05.2023</code>", ParseMode.Html);
                    break;
                }

                _userSubscription[chatId].DepartureMinDate = _departureMinDate;
                _userSubscription[chatId].DepartureMaxDate = _departureMaxDate;

                await botClient.SendTextMessageAsync(chatId, "Введите город прибытия");

                _userSteps[chatId] = step + 1;
                break;
            case 6:
                var res6Step = (await _autocomplete.GetIataCodeByName(message.Text))
                    .Select(result => result.ToString())
                    .ToArray();

                if (res6Step.Length > 0)
                {
                    var keyboard6Markup = new KeyboardButton[res6Step.Length][];
                    for (var i = 0; i < res6Step.Length; i++)
                    {
                        keyboard6Markup[i] = new[] { new KeyboardButton(res6Step[i]) };
                    }

                    ReplyKeyboardMarkup step6Keyboard = new(keyboard6Markup) { ResizeKeyboard = true };
                    await botClient.SendTextMessageAsync(chatId, "Выберите пункт прибытия из списка",
                        replyMarkup: step6Keyboard);
                    _userSteps[chatId] = step + 1;
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Ничего не найдено, попробуйте еще раз");
                }

                break;
            case 7:
                var arrCity = IataObject.FromString(message.Text);
                if (!_userSubscription[chatId].Destination.Contains(arrCity))
                {
                    _userSubscription[chatId].Destination.Add(arrCity);
                    arrCity.Save();
                }

                ReplyKeyboardMarkup step7Keyboard = new(new[]
                    {
                        new KeyboardButton[] { "Добавить город прибытия", "Следующий шаг" },
                    })
                    { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(chatId, "Выберите действие",
                    replyMarkup: step7Keyboard);
                _userSteps[chatId] = step + 1;
                break;
            case 8:
                if (message.Text.Equals("Добавить город прибытия"))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Добавить город прибытия",
                        replyMarkup: new ReplyKeyboardRemove());
                    _userSteps[chatId] = 6;
                }
                else if (message.Text.Equals("Следующий шаг"))
                {
                    ReplyKeyboardMarkup step8Keyboard = new(new[]
                        {
                            new KeyboardButton[] { "Да", "Нет" },
                        })
                        { ResizeKeyboard = true };
                    await botClient.SendTextMessageAsync(chatId, "Искать только прямые рейсы?",
                        replyMarkup: step8Keyboard);
                    _userSteps[chatId] = step + 1;
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Некорректный ввод");
                }

                break;

            case 9:
                if (message.Text.Equals("Да") || message.Text.Equals("Нет"))
                {
                    _userSubscription[chatId].OnlyDirect = message.Text switch
                    {
                        "Да" => true,
                        "Нет" => false
                    };

                    ReplyKeyboardMarkup step10Keyboard = new(new[]
                        {
                            new KeyboardButton[] { "OK", "Cancel" },
                        })
                        { ResizeKeyboard = true };
                    await botClient.SendTextMessageAsync(chatId, "Подтвердите параметры подписки");
                    await botClient.SendTextMessageAsync(chatId, _userSubscription[chatId].ToString(),
                        ParseMode.Markdown,
                        replyMarkup: step10Keyboard);

                    _userSteps[chatId] = step + 1;
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Некорректный ввод");
                }

                break;

            case 10:
                switch (message.Text)
                {
                    case "OK":
                        _userSubscription[chatId].Save();
                        await botClient.SendTextMessageAsync(chatId,
                            "Подписка сохранена",
                            replyMarkup: new ReplyKeyboardRemove());
                        _userSteps.Remove(chatId);
                        _userSubscription.Remove(chatId);
                        return true;
                    case "Cancel":
                        await botClient.SendTextMessageAsync(chatId,
                            "Ввод отменен",
                            replyMarkup: new ReplyKeyboardRemove());
                        _userSteps.Remove(chatId);
                        _userSubscription.Remove(chatId);
                        return true;
                }

                break;
        }

        return false;
    }
}
