using Coravel.Invocable;
using FlightsMetaSubscriber.App.Repositories;

namespace FlightsMetaSubscriber.App.Scheduler;

public class OverdueSubscriptionDisabler : IInvocable
{
    private readonly ILogger<OverdueSubscriptionDisabler> _logger;

    public OverdueSubscriptionDisabler(ILogger<OverdueSubscriptionDisabler> logger)
    {
        _logger = logger;
    }

    public async Task Invoke()
    {
        _logger.LogInformation("Start disabling overdue subscription");

        var overdueSubscriptions = SubscriptionRepository.GetOverdueSubscriptions();

        _logger.LogDebug("Found {@Qty} overdue subscription", overdueSubscriptions.Count);

        foreach (var overdueSubscription in overdueSubscriptions)
        {
            overdueSubscription.DisableSubscription();
        }

        _logger.LogInformation("Disabling overdue subscription finished");
    }
}
