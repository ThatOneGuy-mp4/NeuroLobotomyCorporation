using Newtonsoft.Json.Linq;

namespace NeuroSDKCsharp.Messages.API;

public readonly struct IncomingData
{
    public readonly JToken? Data;

    public IncomingData(JToken? data)
    {
        Data = data;
    }
}