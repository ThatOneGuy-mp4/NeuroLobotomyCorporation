using Newtonsoft.Json.Linq;

namespace NeuroSDKCsharp.Actions;

public class ActionData
{
    public JToken? Data { get; private set; }

    private ActionData()
    {
    }

    private void DeserializeFromJson(string? stringData)
    {
        if (string.IsNullOrEmpty(stringData))
        {
            stringData = null;
            return;
        }

        Data = JToken.Parse(stringData);
    }

    internal static bool TryParse(string? stringData, out ActionData? actionData)
    {
        try
        {
            actionData = new ActionData();
            actionData.DeserializeFromJson(stringData);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            actionData = null;
            return false;
        }
    }
}