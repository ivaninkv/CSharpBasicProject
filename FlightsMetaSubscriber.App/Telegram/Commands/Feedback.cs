using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class Feedback : ICommand
{
    private readonly ILogger<Feedback> _logger;
    private readonly string _stepLogTemplate = "User {@user}, step {@step}, input message: {@message}";
    private readonly Dictionary<long, int> _userSteps = new();

    public Feedback(ILogger<Feedback> logger)
    {
        _logger = logger;
    }

    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        var chatId = message.Chat.Id;
        var step = _userSteps.ContainsKey(chatId) ? _userSteps[chatId] : 1;

        switch (step)
        {
            case 1:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                await botClient.SendTextMessageAsync(chatId, 
                    "Напишите ваше пожелание, предложение или замечание");
                _userSteps[chatId] = step + 1;
                break;
            case 2:
                _logger.LogInformation(_stepLogTemplate, chatId, step, message.Text);
                foreach (var adminId in Config.AdminIds)
                {
                    await botClient.SendTextMessageAsync(adminId, message.Text);
                }

                await botClient.SendTextMessageAsync(chatId, 
                    "Ваше сообщение отправлено разработчикам. Спасибо за обратную связь!");
                
                _userSteps.Remove(chatId);
                return true;
        }

        return false;
    }
}