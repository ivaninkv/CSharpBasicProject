using FlightsMetaSubscriber.App.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsMetaSubscriber.App.Telegram.Commands;

public class News : ICommand
{
    private readonly ILogger<News> _logger;
    private readonly string commandPrefix = "/news";

    public News(ILogger<News> logger)
    {
        _logger = logger;
    }

    public async Task<bool> Handle(ITelegramBotClient botClient, Message message)
    {
        var chatId = message.Chat.Id;
        if (!Config.AdminIds.Contains(chatId))
        {
            botClient.SendTextMessageAsync(chatId, "У вас нет прав использовать эту команду");
        }
        else
        {
            var newsText = message.Text?.Split(commandPrefix)[1];
            {
                var activeUser = UserRepository.GetUsers(onlyActive: true);
                foreach (var user in activeUser)
                {
                    _logger.LogInformation("Sending news {@News} to user {@User}",
                        newsText, user.Id);
                    botClient.SendTextMessageAsync(user.Id, newsText);
                }
            }
        }

        return true;
    }
}