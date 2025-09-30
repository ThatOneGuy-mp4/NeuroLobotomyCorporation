using NeuroSDKCsharp.Actions;
using NeuroSDKCsharp.Messages.API;
using Newtonsoft.Json;

namespace NeuroSDKCsharp.Messages.Outgoing;

public class ActionsForce : OutgoingMessageHandler
{
    protected override string Command => "actions/force";

    public ActionsForce(string query, string? state, bool? ephemeralContext, IEnumerable<INeuroAction> actions)
    {
        _state = state;
        _query = query;
        _ephemeralContext = ephemeralContext;
        _actionNames = actions.Select(a => a.Name).ToArray();
    }

    public ActionsForce(string query, string? state, bool? ephemeralContext, params INeuroAction[] actions) : this(
        query, state, ephemeralContext, (IEnumerable<INeuroAction>)actions)
    {}

    [JsonProperty("state", Order = 0)]
    private readonly string? _state;

    [JsonProperty("query", Order = 10)]
    private readonly string _query;

    [JsonProperty("ephemeral_context", Order = 20)]
    private readonly bool? _ephemeralContext;

    [JsonProperty("action_names", Order = 30)]
    private readonly string[] _actionNames;
}