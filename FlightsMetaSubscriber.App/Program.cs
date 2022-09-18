using FlightsMetaSubscriber.App;
using FlightsMetaSubscriber.App.Telegram;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<TgBot>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
