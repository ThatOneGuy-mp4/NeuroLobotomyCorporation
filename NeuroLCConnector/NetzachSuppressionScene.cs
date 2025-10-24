using NeuroSDKCsharp.Messages.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class NetzachSuppressionScene : CoreSuppressionBaseScene
    {
        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "UNUSED",
            "\"You weren’t the person I put my trust in. ...This place will never be safe, you know.\"" +
            "\n\nNetzach's Qlipha has been agitated. Something within it stirs...all Agents have had their HP and SP completely healed.",
            "\"Abandon your sense of guilt, everyone. They couldn’t be saved anyways. ...‘Cause this place is always horrible, the only thing I can hope for is a blissful end.\"" +
            "\n\nNetzach's Qlipha has been agitated. Something within it stirs...all Agents have had their HP and SP completely healed.",
            "\"In the end, the hope that anyone will live on thanks to me is gone. ...I never wanted to be in this position anyways. I never asked for this. Not once.\"" +
            "\n\nNetzach's Qlipha has been agitated. Something within it stirs...all Agents have had their HP and SP completely healed." +
            "\nAgainst his will, Netzach is beginning to wake up...please be fearlessness in the last stretch of this suppression.",
            "\"Just leave me alone, it’s what you’re best at. ...All I want is to close my eyes and enjoy a good nap, just once.\"" +
            "\n\nNetzach's Qlipha has been agitated. Something within it stirs...all Agents have had their HP and SP completely healed."
        };

        protected override string SuppressionCompleteDialogue => "\"I stopped breathing, but if life calls out to me yet again...\"" +
            "\nNetzach's Qlipha has vanished from the Safety Department... Core Suppression successfully completed.";
    }
}
