using actorsapi.Actors;
using Dapr.Actors;
using Dapr.Actors.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<DeviceActor>();
    options.ActorScanInterval = TimeSpan.FromSeconds(30);
    options.ActorIdleTimeout = TimeSpan.FromMinutes(1);

    //https://github.com/dapr/dapr/pull/8542
    options.ReentrancyConfig = new ActorReentrancyConfig { Enabled = true, MaxStackDepth = 32 };
});

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-9.0
//following line is already specified via an environmental variable in the launch settings: ASPNETCORE_HTTP_PORTS
//builder.WebHost.ConfigureKestrel(serverOptions => { serverOptions.ListenAnyIP(8080); });

var app = builder.Build();

//app.UseHttpsRedirection(); Don't use this, this middleware blocks Dapr calls

app.MapActorsHandlers();

//test force garbage collection (deactivated actors)
app.MapPost("/gc", async () =>
{
    GC.Collect();
});

app.MapGet("/", () => "Hello World!");

#region Actor client functionality

app.MapPost("/setstate", async (int numberState) =>
{
    var actorProxyFactory = new ActorProxyFactory();
    var deviceActorId = new ActorId("device");
    var deviceProxy = actorProxyFactory.CreateActorProxy<IDeviceActor>(deviceActorId, "DeviceActor");
    await deviceProxy.SetStateAsync(numberState);
});

app.MapPost("/getstate", async () =>
{
    var actorProxyFactory = new ActorProxyFactory();
    var deviceActorId = new ActorId("device");
    var deviceProxy = actorProxyFactory.CreateActorProxy<IDeviceActor>(deviceActorId, "DeviceActor");
    var state = await deviceProxy.GetStateAsync();
    return Results.Json(new { Actual = state.Item1, Stale = state.Item2 });
});

app.MapPost("/registerreminder", async () =>
{
    var actorProxyFactory = new ActorProxyFactory();
    var deviceActorId = new ActorId("device");
    var deviceProxy = actorProxyFactory.CreateActorProxy<IDeviceActor>(deviceActorId, "DeviceActor");
    await deviceProxy.RegisterReminder();
});

app.MapPost("/unregisterreminder", async () =>
{
    var actorProxyFactory = new ActorProxyFactory();
    var deviceActorId = new ActorId("device");
    var deviceProxy = actorProxyFactory.CreateActorProxy<IDeviceActor>(deviceActorId, "DeviceActor");
    await deviceProxy.UnregisterReminder();
});

#endregion

app.Run();
