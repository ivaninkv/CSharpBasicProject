using Coravel.Invocable;
using FlightsMetaSubscriber.App.Repositories;
using FlightsMetaSubscriber.App.Telegram;
using Telegram.Bot;

namespace FlightsMetaSubscriber.App.Scheduler;

public class UserInfoUpdater : IInvocable
{
    private readonly ILogger<UserInfoUpdater> _logger;
    private readonly TgBotClient _tgBotClient;

    public UserInfoUpdater(ILogger<UserInfoUpdater> logger, TgBotClient tgBotClient)
    {
        _logger = logger;
        _tgBotClient = tgBotClient;
    }

    public async Task Invoke()
    {
        var tgUsers = UserRepository.GetUsers(onlyActive: false);
        foreach (var tgUser in tgUsers)
        {
            try
            {
                var userName = _tgBotClient.BotClient.GetChatAsync(tgUser.Id).Result.Username;
                tgUser.UserName = userName;
                UserRepository.Save(tgUser);
            }
            catch (Exception e)
            {
                _logger.LogWarning("UserInfoUpdater for {@User} has error: {@Error}",
                    tgUser, e.Message);
            }
        }
    }
}
