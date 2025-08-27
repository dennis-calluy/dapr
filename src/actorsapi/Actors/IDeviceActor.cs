using Dapr.Actors;

namespace actorsapi.Actors;

public interface IDeviceActor : IActor
{
    Task SetStateAsync(int deviceState);
    Task<(int, int)> GetStateAsync();

    Task RegisterReminder();
    Task UnregisterReminder();
}