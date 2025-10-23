using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class MalkuthSuppressionScene : CoreSuppressionBaseScene
    {
        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "UNUSED",
            "\"I knew I didn’t have any right to participate in the experiment, that’s why I was so impatient. ...Make sure you feel this horrible sense of helplessness to your core, right to your heart.\"" +
            "\n\nMalkuth's Qlipha has been disturbed. The error with the work assignment system has mutated.",
            "\"Please watch me manager! I can take care of this situation, no problem! ...I still have so much to do, just look at my notebook... it’s all worn out...\"" +
            "\n\nMalkuth's Qlipha has been agitated. The error with the work assignment system remains stable.",
            "\"Can’t you hear it? The sound of it struggling, as if to say it can’t fall asleep like this. ...Will you finally praise me? I just want to feel proud.\"" +
            "\n\nMalkuth's Qlipha has been disturbed. The error with the work assignment system has mutated. Additionally, an error in the work cancellation system has been detected." +
            "\nMalkuth's core cannot sustain this level of corruption for much longer...please maintain your will in the last stretch of this suppression.",
            "\"I see now how much fun it can be to look down on someone, manager... Be honest, back then it wasn’t that hard to turn around and look back at me.\"" +
            "\n\nMalkuth's Qlipha has been agitated. The error with the work assignment system remains stable."
        };

        protected override string SuppressionCompleteDialogue => "\"Ah... So I couldn’t do it...\"" +
            "\nMalkuth's Qlipha has vanished from the Control Department... Core Suppression successfully completed.";
    }
}
