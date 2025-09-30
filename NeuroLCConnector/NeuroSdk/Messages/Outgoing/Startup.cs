using NeuroSDKCsharp.Messages.API;

namespace NeuroSDKCsharp.Messages.Outgoing;

public sealed class Startup : OutgoingMessageHandler
{
    protected override string Command => "startup";
    protected override object? Data => null;
}