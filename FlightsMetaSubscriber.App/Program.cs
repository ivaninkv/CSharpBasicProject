using Coravel;
using FlightsMetaSubscriber.App;
using FlightsMetaSubscriber.App.AviasalesAPI;
using FlightsMetaSubscriber.App.Scheduler;
using FlightsMetaSubscriber.App.Telegram;
using FlightsMetaSubscriber.App.Telegram.Commands;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Debug()
    .CreateBootstrapLogger();

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<TgBotClient>();
        services.AddSingleton<TgBot>();
        services.AddSingleton<TgErrorHandler>();
        services.AddSingleton<TgUpdateHandler>();
        services.AddSingleton<Start>();
        services.AddSingleton<Stop>();
        services.AddSingleton<Help>();
        services.AddSingleton<GetPrices>();
        services.AddSingleton<NewSubscription>();
        services.AddSingleton<MySubscriptions>();
        services.AddSingleton<DelSubscription>();
        services.AddScoped<Autocomplete>();
        services.AddScoped<PricesOneWay>();
        services.AddHostedService<Worker>();
        services.AddTransient<PricesUpdater>();
        services.AddTransient<OverdueSubscriptionDisabler>();
        services.AddScheduler();
    })
    .UseSerilog()
    .Build();

host.Services.UseScheduler(scheduler =>
{
    scheduler.ScheduleAsync(async () =>
    {
        using var scope = host.Services.CreateScope();
        var updater = scope.ServiceProvider.GetRequiredService<PricesUpdater>();
        try
        {
            await updater.Invoke();
        }
        catch (Exception e)
        {
            Log.Logger.Warning("PricesUpdater return error: {@ErrorMessage}",
                e.Message);
        }
    }).DailyAt(7, 0);

    scheduler.ScheduleAsync(async () =>
    {
        using var scope = host.Services.CreateScope();
        var disabler = scope.ServiceProvider.GetRequiredService<OverdueSubscriptionDisabler>();
        try
        {
            await disabler.Invoke();
        }
        catch (Exception e)
        {
            Log.Logger.Warning("OverdueSubscriptionDisabler return error: {@ErrorMessage}",
                e.Message);
        }
    }).Daily();
});
try
{
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
