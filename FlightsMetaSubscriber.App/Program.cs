using FlightsMetaSubscriber.App;
using FlightsMetaSubscriber.App.AviasalesAPI;
using FlightsMetaSubscriber.App.Telegram;
using FlightsMetaSubscriber.App.Telegram.Commands;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    using var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<TgBot>();
            services.AddSingleton<TgErrorHandler>();
            services.AddSingleton<TgUpdateHandler>();
            services.AddSingleton<Start>();
            services.AddSingleton<NewSubscription>();
            services.AddSingleton<MySubscriptions>();
            services.AddScoped<Autocomplete>();
            services.AddHostedService<Worker>();
        })
        .UseSerilog()
        .Build();

    await host.RunAsync();
}
catch (Exception e)
{
    Log.Fatal(e, "Unhandled exception");
}
finally
{
    Log.Information("Shutting down");
    Log.CloseAndFlush();
}
