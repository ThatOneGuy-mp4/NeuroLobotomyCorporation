using NeuroSDKCsharp.Messages.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class ChesedSuppressionScene : CoreSuppressionBaseScene
    {
        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "UNUSED",
            "\"Pour that lukewarm coffee down the drain, I’ll need another cup.\"" +
            "\n\nChesed's Qlipha has been agitated. The anomaly with the amount of damage employees receive has mutated.",
            "\"I wonder... do I still have a conscience left inside me?\"" +
            "\n\nChesed's Qlipha has been disturbed. The anomaly with the amount of damage employees receive has mutated and intensified.",
            "\"I envy you manager, you can conveniently forget your moments most filled with shame.\"" +
            "\n\nChesed's Qlipha has been agitated. The anomaly with the amount of damage employees receive has mutated.",
            "\"Each time Angela shows up, I shall commit sin according to her orders… And I see no fig leaf here to cover my shame.\"" +
            "\n\nChesed's Qlipha has been agitated. The anomaly with the amount of damage employees receive has mutated.",
            "\"It’s much too late for me to repent.\"" +
            "\n\nChesed's Qlipha has been disturbed. The anomaly with the amount of damage employees receive has mutated and intensified.",
            "\"I didn’t want to open my eyes again. I just yearned to fall into the infernal pit of hell, bearing my sins on my back. ...My friends were all gone, and I waited alone for dawn to rise out of the darkness. \"" +
            "\n\nChesed's Qlipha has been agitated. The anomaly with the amount of damage employees receive has mutated.",
        };

        protected string SuppressionAlmostDoneDialogue => "\nChesed's core cannot sustain this level of corruption for much longer...please have faith in yourself and your allies in the last stretch of this suppression.";

        protected override string SuppressionCompleteDialogue => "\"Is my world... finally crumbling?\"" +
            "\nChesed's Qlipha has vanished from the Welfare Department... Core Suppression successfully completed.";

        public override async Task ChangePhase()
        {
            phase++;
            if(phase < PhaseTransitionDialogue.Count)
            {
                string dialogue = String.Format("{0} {1}", PhaseTransitionDialogue[phase], NeuroLCConnector.Connector.SendCommand("get_boosted_damage").Result);
                if (phase == 5) dialogue += SuppressionAlmostDoneDialogue;
                Context.Send(dialogue, true);
            }
        }
    }
}
