using Newtonsoft.Json;

namespace NeuroSDKCsharp.Websocket;

public record WsMessage
{
    public WsMessage(string command, object? data, string game)
    {
        Command = command;
        Data = data;
        Game = game;
    }

    [JsonProperty("command", Order=0)]
    public readonly string Command;
    
    [JsonProperty("game", Order=10)]
    public readonly string Game;
    
    [JsonProperty("data", Order=20)]
    public readonly object? Data;
}