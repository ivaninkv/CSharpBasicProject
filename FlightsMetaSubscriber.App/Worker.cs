using FlightsMetaSubscriber.App.Telegram;

namespace FlightsMetaSubscriber.App;

public class Worker : IHostedService
{
    private readonly TgBot _bot;
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger, TgBot bot)
    {
        _logger = logger;
        _bot = bot;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting bot...");
        await _bot.RunBot();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stop bot.");
        return Task.CompletedTask;
    }
}
