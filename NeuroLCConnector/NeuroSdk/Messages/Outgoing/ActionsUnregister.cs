using NeuroSDKCsharp.Actions;
using NeuroSDKCsharp.Messages.API;
using Newtonsoft.Json;

namespace NeuroSDKCsharp.Messages.Outgoing;

public sealed class ActionsUnregister : OutgoingMessageHandler
{
    protected override string Command => "actions/unregister";

    public ActionsUnregister(IEnumerable<string> actionNames)
    {
        Names = actionNames.ToList();
    }

    public ActionsUnregister(IEnumerable<INeuroAction> actions) : this(actions.Select(a => a.Name))
    {}

    public ActionsUnregister(params INeuroAction[] actions) : this((IEnumerable<INeuroAction>) actions)
    {}
    
    public ActionsUnregister(params string[] actionNames) : this((IEnumerable<string>) actionNames)
    {}

    [JsonProperty("action_names")]
    public readonly List<string> Names;
    
    public override bool Merge(OutgoingMessageHandler handler)
    {
        if (handler is ActionsUnregister actionsUnregister)
        {
            Names.RemoveAll(existingNames => actionsUnregister.Names.Contains(existingNames));
            Names.AddRange(actionsUnregister.Names);
            return true;
        }

        return false;
    }
}