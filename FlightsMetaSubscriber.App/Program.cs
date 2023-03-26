using Coravel;
using FlightsMetaSubscriber.App;
using FlightsMetaSubscriber.App.AviasalesAPI;
using FlightsMetaSubscriber.App.Scheduler;
using FlightsMetaSubscriber.App.Telegram;
using FlightsMetaSubscriber.App.Telegram.Commands;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:dd-MM-yyyy HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(Path.Combine(Config.LogDirectory!, "fms.txt"), rollingInterval: RollingInterval.Day)
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
        services.AddSingleton<UnknownCommand>();
        services.AddSingleton<News>();
        services.AddSingleton<Feedback>();
        services.AddSingleton<Users>();
        services.AddScoped<Autocomplete>();
        services.AddScoped<GraphQLClient>();
        services.AddScoped<PricesOneWay>();
        services.AddScoped<PricesRoundTrip>();
        services.AddHostedService<Worker>();
        services.AddTransient<PricesUpdater>();
        services.AddTransient<OverdueSubscriptionDisabler>();
        services.AddTransient<UserInfoUpdater>();
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
    }).DailyAt(hour: 7, minute: 0);

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

    scheduler.ScheduleAsync(async () =>
    {
        using var scope = host.Services.CreateScope();
        var userInfoUpdater = scope.ServiceProvider.GetRequiredService<UserInfoUpdater>();
        try
        {
            await userInfoUpdater.Invoke();
        }
        catch (Exception e)
        {
            Log.Logger.Warning("UserInfoUpdater return error: {@ErrorMessage}",
                e.Message);
        }
    }).Weekly().RunOnceAtStart();
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