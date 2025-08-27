## Dapr Issues

### Stale Actor State in Reminder with Reentrancy Enabled

Description:

When using a Dapr Actor with reentrancy enabled, the actor's state is stale when accessed within a reminder callback. The actor's state is being updated by an external method call, but the reminder's ReceiveReminderAsync method receives only the initial state.

The ReceiveReminderAsync method should access the most current, up-to-date state of the actor, regardless of whether a separate, call had updated the state. Instead, the ReceiveReminderAsync method retrieves a stale version of the actor's state.

Environments:

V1.15.10 dotnet-sdk: v1.15.4
V1.16.0-rc.4 dotnet-sdk: v1.16.0-rc15

To reproduce:

Enable reentrancy
```
options.ReentrancyConfig = new ActorReentrancyConfig { Enabled = true, MaxStackDepth = 32 };
```

After the reminder has started, the ReceiveReminderAsync method will not pick up new state!
```
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
```


References:
```
https://github.com/dapr/dapr/issues/8538
https://github.com/dapr/dotnet-sdk/issues/1471
```

[docker compose configuration](./manifests/compose/readme.md)

[k8s configuration](./manifests/k3d/readme.md)





### Reentrancy fails with reminders

https://github.com/dapr/dapr/issues/8514
