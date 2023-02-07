using Telegram.Bot;

namespace FlightsMetaSubscriber.App.Telegram;

public class TgBotClient
{
    public readonly TelegramBotClient BotClient;

    public TgBotClient()
    {
        BotClient = new TelegramBotClient(Config.BotToken);
    }
}
