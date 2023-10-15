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
    private readonly string _stepLogTemplate = "User {@user}, step {@step}, input message: {@message}";
    private readonly ILogger<TgUpdateHandler> _logger;
    private readonly Autocomplete _autocomplete;
    private readonly Dictionary<long, int> _userSteps = new();
    private readonly Dictionary<long, Subscription> _userSubscription = new();
    private DateTime _departureMinDate;
    private DateTime _departureMaxDate;
    private DateTime _returnMinDate;
    private DateTime _returnMaxDate;

    public NewSubscription(ILogger<TgUpdateHandler> logger, Autocomplete autocomplete)
    {
        _logger = logger;
        _autocomplete = autocomplete;
    }

    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        var chatId = message.Chat.Id;
        if (message.Text is "/new")
        {
            _userSteps.Remove(chatId);
            _userSubscription.Remove(chatId);
        }

        var step = _userSteps.ContainsKey(chatId) ? _userSteps[chatId] : 1;
        if (!_userSubscription.ContainsKey(chatId))
        {
            _userSubscription[chatId] = new Subscription(chatId);
        }

        switch (step)
        {
            case 1:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                await botClient.SendTextMessageAsync(chatId,
                    "Введите город вылета");
                _userSteps[chatId] = step + 1;
                break;
            case 2:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                var res = (await _autocomplete.GetIataCodeByName(message.Text!))
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
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                var depCity = IataObject.FromString(message.Text!);
                if (!_userSubscription[chatId].Origin.Contains(depCity))
                {
                    _userSubscription[chatId].Origin.Add(depCity);
                    depCity.Save();
                }

                ReplyKeyboardMarkup step3Keyboard = new(new[]
                    {
                        new KeyboardButton[] { "Добавить город вылета", "Следующий шаг" },
                    })
                    { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(chatId, "Выберите следующее действие",
                    replyMarkup: step3Keyboard);
                _userSteps[chatId] = step + 1;
                break;
            case 4:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                if (message.Text!.Equals("Добавить город вылета"))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Введите город вылета",
                        replyMarkup: new ReplyKeyboardRemove());
                    _userSteps[chatId] = 2;
                }
                else if (message.Text.Equals("Следующий шаг"))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Введите город прибытия", replyMarkup: new ReplyKeyboardRemove());
                    _userSteps[chatId] = step + 1;
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Некорректный ввод");
                }

                break;
            case 5:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                var res5Step = (await _autocomplete.GetIataCodeByName(message.Text!))
                    .Select(result => result.ToString())
                    .ToArray();

                if (res5Step.Length > 0)
                {
                    var keyboard6Markup = new KeyboardButton[res5Step.Length][];
                    for (var i = 0; i < res5Step.Length; i++)
                    {
                        keyboard6Markup[i] = new[] { new KeyboardButton(res5Step[i]) };
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
            case 6:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                var arrCity = IataObject.FromString(message.Text!);
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
            case 7:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                if (message.Text!.Equals("Добавить город прибытия"))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Введите город прибытия",
                        replyMarkup: new ReplyKeyboardRemove());
                    _userSteps[chatId] = 5;
                }
                else if (message.Text.Equals("Следующий шаг"))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Введите диапазон дат вылета в формате: 'dd.MM.yyyy-dd.MM.yyyy'\n" +
                        "Например: <code>01.05.2023-14.05.2023</code>", ParseMode.Html,
                        replyMarkup: new ReplyKeyboardRemove());
                    _userSteps[chatId] = step + 1;
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Некорректный ввод");
                }

                break;
            case 8:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                var split = message.Text!.Split("-")
                    .Select(s => s.Trim()).ToArray();
                if (!DateTime.TryParseExact(split[0], "dd.MM.yyyy", null, DateTimeStyles.None, out _departureMinDate) ||
                    !DateTime.TryParseExact(split[1], "dd.MM.yyyy", null, DateTimeStyles.None, out _departureMaxDate))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Некорректно введены даты.\n\n" +
                        "Введите диапазон дат вылета в формате: 'dd.MM.yyyy-dd.MM.yyyy'\n" +
                        "Например: <code>01.05.2023-14.05.2023</code>", ParseMode.Html);
                    break;
                }

                _userSubscription[chatId].DepartureMinDate = _departureMinDate;
                _userSubscription[chatId].DepartureMaxDate = _departureMaxDate;

                ReplyKeyboardMarkup step8Keyboard = new(new[]
                    {
                        new KeyboardButton[] { "Да", "Нет" },
                    })
                    { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(chatId, "Нужны обратные билеты?",
                    replyMarkup: step8Keyboard);
                _userSteps[chatId] = step + 1;

                break;

            case 9:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                if (message.Text!.Equals("Да"))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Введите диапазон дат возвращения в формате: 'dd.MM.yyyy-dd.MM.yyyy'\n" +
                        "Например: <code>15.05.2023-31.05.2023</code>", ParseMode.Html,
                        replyMarkup: new ReplyKeyboardRemove());
                    _userSteps[chatId] = step + 1;
                }
                else if (message.Text.Equals("Нет"))
                {
                    ReplyKeyboardMarkup step9Keyboard = new(new[]
                        {
                            new KeyboardButton[] { "Да", "Нет" },
                        })
                        { ResizeKeyboard = true };
                    await botClient.SendTextMessageAsync(chatId, "Искать только прямые рейсы?",
                        replyMarkup: step9Keyboard);
                    _userSteps[chatId] = step + 2;
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Некорректный ввод");
                }

                break;
            case 10:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                var split10step = message.Text!.Split("-")
                    .Select(s => s.Trim()).ToArray();
                if (!DateTime.TryParseExact(split10step[0], "dd.MM.yyyy", null, DateTimeStyles.None,
                        out _returnMinDate) ||
                    !DateTime.TryParseExact(split10step[1], "dd.MM.yyyy", null, DateTimeStyles.None,
                        out _returnMaxDate))
                {
                    await botClient.SendTextMessageAsync(chatId,
                        "Некорректно введены даты.\n\n" +
                        "Введите диапазон дат возвращения в формате: 'dd.MM.yyyy-dd.MM.yyyy'\n" +
                        "Например: <code>15.05.2023-31.05.2023</code>", ParseMode.Html);
                    break;
                }

                _userSubscription[chatId].ReturnMinDate = _returnMinDate;
                _userSubscription[chatId].ReturnMaxDate = _returnMaxDate;

                ReplyKeyboardMarkup step10Keyboard = new(new[]
                    {
                        new KeyboardButton[] { "Да", "Нет" },
                    })
                    { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(chatId, "Искать только прямые рейсы?",
                    replyMarkup: step10Keyboard);
                _userSteps[chatId] = step + 1;

                break;
            case 11:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                if (message.Text!.Equals("Да") || message.Text.Equals("Нет"))
                {
                    _userSubscription[chatId].OnlyDirect = message.Text switch
                    {
                        "Да" => true,
                        "Нет" => false,
                        _ => throw new ArgumentOutOfRangeException(message.Text, "Incorrect input")
                    };

                    ReplyKeyboardMarkup step11Keyboard = new(new[]
                        {
                            new KeyboardButton[] { "Да", "Нет" },
                        })
                        { ResizeKeyboard = true };
                    await botClient.SendTextMessageAsync(chatId, "Искать только рейсы с багажом?",
                        replyMarkup: step11Keyboard);
                    _userSteps[chatId] = step + 1;
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Некорректный ввод");
                }

                break;
            case 12:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                if (message.Text!.Equals("Да") || message.Text.Equals("Нет"))
                {
                    _userSubscription[chatId].Baggage = message.Text switch
                    {
                        "Да" => true,
                        "Нет" => false,
                        _ => throw new ArgumentOutOfRangeException(message.Text, "Incorrect input")
                    };

                    ReplyKeyboardMarkup step12Keyboard = new(new[]
                        {
                            new KeyboardButton[] { "OK", "Cancel" },
                        })
                        { ResizeKeyboard = true };
                    await botClient.SendTextMessageAsync(chatId, "Подтвердите параметры подписки");
                    await botClient.SendTextMessageAsync(chatId, _userSubscription[chatId].ToString(),
                        ParseMode.Markdown,
                        replyMarkup: step12Keyboard);

                    _userSteps[chatId] = step + 1;
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "Некорректный ввод");
                }

                break;
            case 13:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
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