using FlightsMetaSubscriber.App.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class Users : ICommand
{
    private readonly ILogger<Users> _logger;
    
    public Users(ILogger<Users> logger)
    {
        _logger = logger;
    }

    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        var chatId = message.Chat.Id;
        if (!TgBot.CheckAdmin(chatId))
        {
            await botClient.SendTextMessageAsync(chatId, "У вас нет прав использовать эту команду");
        }
        else
        {
            var activeUsers = UserRepository.GetUsersQty();
            _logger.LogInformation("Found {@ActiveUsers} active users", activeUsers);
            
            await botClient.SendTextMessageAsync(chatId, $"Всего активных пользователей: {activeUsers}");
        }

        return true;
    }
}