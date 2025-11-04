using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class BinahSuppressionScene : CoreSuppressionBaseScene
    {
        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "UNUSED",
            "\"You’re being reckless yet again, deceiving the Head and planning such bold actions.\"" +
            "\n\"This body simply has too many limits... If only I knew what power this place held, we could have acquired it for us to use.\"" +
            "\n\nAn Arbiter has been brought to her knees, but she is not satisfied. She has gotten back up.",
            "\"What do you think you will accomplish on your own, even after breaking out of this prison? ...Even if you were to break this cycle, it would not last long.\"" +
            "\n\"If you cannot defeat me, you shall be crushed by the Head and its ruthless Claws yet again. ...You cannot escape the Head.\"" +
            "\n\nAn Arbiter has been brought to her knees again, but is still not yet satisfied...please face your fear as the last stretch of this suppression begins."
        };

        protected override string SuppressionCompleteDialogue => "\"You've proven yourself. I shall witness you with my eyes.\"" +
            "\nBinah's Qlipha has been driven out... Core Suppression successfully completed.";
    }
}
