using System.Security.Cryptography;
using NeuroSDKCsharp.Websocket;
using NeuroSDKCsharp.Json;
using Newtonsoft.Json;

namespace NeuroSDKCsharp.Actions;

public readonly struct WSAction
{
    public WSAction(string name, string description, JsonSchema? schema)
    {
        Name = name;
        _description = description;
        _schema = schema;
    }

    [JsonProperty("name", Order = 0)]
    public readonly string Name;

    [JsonProperty("description", Order = 10)]
    private readonly string _description;

    [JsonProperty("schema", Order = 20)]
    private readonly JsonSchema? _schema;
}