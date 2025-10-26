using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class TipherethSuppressionScene : CoreSuppressionBaseScene
    {
        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "UNUSED",
            "\"The Central Department is quite big. Everyone who works there has their hands full.\"" +
            "\n\nTiphereth's Qlipha has been agitated.",
            "\"Is it time for yet another replacement? Time to head to storage, then...\"" +
            "\n\nTiphereth's Qlipha has been agitated.",
            "\"Did Tiphereth get what he wanted at the end of all this? No. Just what were you even looking for in the first place?\"" +
            "\n\nTiphereth's Qlipha has been disturbed. Continue to deal with the progressively intensifying Qliphoth Meltdowns.",
            "\"Don’t stand there looking at me like that, with your forlorn smile. I’ve already abandoned you...\"" +
            "\n\nTiphereth's Qlipha has been agitated.",
            "\"I wish to walk with you again Tiphereth, with the sound of the waves behind us.\"" +
            "\n\nTiphereth's Qlipha has been agitated.",
            "\"If this song of mine that you can hear right now is a dirge to you...\"" +
            "\n\nTiphereth's Qlipha has been disturbed. Continue to deal with the progressively intensifying Qliphoth Meltdowns." +
            "\nLisa's core cannot sustain this level of corruption without Enoch for much longer...expect a successful resolution as you move forth with the last stretch of this suppression.",
            "\"When he returns, could you show Tiphereth how I’ve grown? Will you show him how strong I am now…?\"" +
            "\n\nTiphereth's Qlipha has been agitated.",
            "\"You said that something good would happen next time...\"" +
            "\n\nTiphereth's Qlipha has been agitated.",
        };

        protected override string SuppressionCompleteDialogue => "\"Was all of this really worth it?\"" +
            "\nTiphereth's Qlipha has vanished from the Central Command Department... Tiphereth's broken cores have powered down... Core Suppression successfully completed.";
    }
}
