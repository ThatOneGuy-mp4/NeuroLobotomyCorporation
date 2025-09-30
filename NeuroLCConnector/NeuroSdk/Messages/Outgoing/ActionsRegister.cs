using NeuroSDKCsharp.Actions;
using NeuroSDKCsharp.Messages.API;
using Newtonsoft.Json;

namespace NeuroSDKCsharp.Messages.Outgoing;

public sealed class ActionsRegister : OutgoingMessageHandler
{
    protected override string Command => "actions/register";

    public ActionsRegister(IEnumerable<INeuroAction> actions)
    {
        Actions = actions.Select(action => action.GetWsAction()).ToList();
    }

    public ActionsRegister(params INeuroAction[] actions) : this((IEnumerable<INeuroAction>)actions)
    {
    }

    [JsonProperty("actions")]
    public readonly List<WSAction> Actions;

    public override bool Merge(OutgoingMessageHandler handler)
    {
        if (handler is ActionsRegister actionsRegister)
        {
            Actions.RemoveAll(existingWsa => actionsRegister.Actions.Any(newWsa => newWsa.Name == existingWsa.Name));
            Actions.AddRange(actionsRegister.Actions);
            return true;
        }

        return false;
    }
}