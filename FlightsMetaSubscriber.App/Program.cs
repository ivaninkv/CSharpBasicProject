using FlightsMetaSubscriber.App;
using FlightsMetaSubscriber.App.Telegram;
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
