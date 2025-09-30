using System.Net.Sockets;
using NeuroSDKCsharp.Actions;
using NeuroSDKCsharp.Messages.API;
using NeuroSDKCsharp.Websocket;

namespace NeuroSDKCsharp.Messages.Incoming;

public sealed class ActionsReregisterAll : IncomingMessageHandler
{
    public override bool CanHandle(string command) => command == "actions/reregister_all";
    
    protected override ExecutionResult Validate(string command, IncomingData incomingData) => ExecutionResult.Success();

    protected override void ReportResult(ExecutionResult result)
    {
    }

    protected override void Execute()
    {
        NeuroActionHandler.ResendRegisteredActions();
    }
}