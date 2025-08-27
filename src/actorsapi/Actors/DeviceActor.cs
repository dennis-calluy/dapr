using Dapr.Actors.Runtime;

namespace actorsapi.Actors;

[Actor(TypeName = "DeviceActor")]
public class DeviceActor : Actor, IDeviceActor, IRemindable
{
    private const string ACTUALSTATE = "device-state-key";
    private const string STALESTATE = "reminder-state-key";
    private const string REMINDER = "device-reminder-key";

    public DeviceActor(ActorHost host) : base(host)
    {
        Logger.LogInformation($"Actor id {Id}: Constructed.");
    }

    ~DeviceActor()
    {
        Logger.LogInformation($"Actor id {Id}: Destructed.");
    }

    protected override async Task OnActivateAsync()
    {
        await base.OnActivateAsync();
        Logger.LogInformation($"Actor id {Id}: Activated.");
    }

    protected override async Task OnDeactivateAsync()
    {
        await base.OnDeactivateAsync();
        Logger.LogInformation($"Actor id {Id}: Deactivated.");
    }




    async Task IDeviceActor.RegisterReminder()
    {
        await RegisterReminderAsync(REMINDER, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        Logger.LogInformation($"Actor id {Id}: Reminder registered.");
    }

    async Task IDeviceActor.UnregisterReminder()
    {
        await UnregisterReminderAsync(REMINDER);
        Logger.LogInformation($"Actor id {Id}: Reminder unregistered.");
    }

    async Task IDeviceActor.SetStateAsync(int actualState)
    {
        await StateManager.SetStateAsync(ACTUALSTATE, actualState);
        await StateManager.SaveStateAsync();
        Logger.LogInformation($"Actor id {Id}: State set to {actualState}");
    }

    async Task<(int, int)> IDeviceActor.GetStateAsync()
    {
        var deviceState = await StateManager.GetStateAsync<int>(ACTUALSTATE);
        var staleState = await StateManager.TryGetStateAsync<int>(STALESTATE);
        Logger.LogInformation($"Actor id {Id}: Actual state: {deviceState} - Stale (reminder) state: {staleState.Value} ");
        return (deviceState, staleState.Value);
    }

    async Task IRemindable.ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        if (reminderName == REMINDER)
        {
            var reminderState = await StateManager.GetStateAsync<int>(ACTUALSTATE);
            Logger.LogInformation($"Actor id {Id}: Reminder gets state {reminderState}.");

            await StateManager.SetStateAsync(STALESTATE, reminderState);
            await StateManager.SaveStateAsync();
        }
    }
}