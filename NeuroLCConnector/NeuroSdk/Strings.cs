namespace NeuroSDKCsharp;

internal static class Strings
{
    public const string VedalFaultSuffix = " (This is probably not your fault, blame Vedal.)";
    public const string ModFaultSuffix = " (This is probably not your fault, blame the game integration.)";

    public const string ActionFailedNoData = "Action failed. Missing command data.";
    public const string ActionFailedNoId = "Action failed. Missing command field 'id'.";
    public const string ActionFailedNoName = "Action failed. Missing command field 'name'.";
    public const string ActionFailedInvalidJson = "Action failed. Could not parse action parameters from JSON.";

    public const string ActionFailedUnregistered =
        "This action has been recently unregistered and can no longer be used.";
}