using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class HodSuppressionScene : CoreSuppressionBaseScene
    {
        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "UNUSED",
            "\"How come nobody understands my kindness? ...I’m the only one who cares about them… Why would they hate me...?\"" +
            "\n\nHod's Qlipha has been disturbed. The error with our employees' statuses has intensified. All Agents are now reporting feelings of exhaustion.",
            "\"Manager, you’re not mad at me, right? You can’t be mad at me, I swear I’m a good Sephirah... It’s all thanks to me that the employees could survive so long, but does anyone ever thank me? No, no one does...\"" +
            "\n\nHod's Qlipha has been agitated. The error with our employees' statuses remains stable.",
            "\"Our employees are suffering! See? Things just don’t work out without me here! ...Where’s Tiffany? She’s late for her counseling session. Why isn’t she here?\"" +
            "\n\nHod's Qlipha has been disturbed. The error with our employees' statuses has intensified. All Agents are now reporting feelings of fatigue.",
            "\"You all would be dead where you stand without me! Every single one of you! ...Forgive me, manager. If you can’t forgive, then please at least forget.\"" +
            "\n\nHod's Qlipha has been agitated. The error with our employees' statuses remains stable." +
            "\nHod's core cannot sustain this level of corruption for much longer...suppression is nearing completion."
        };

        protected override string SuppressionCompleteDialogue => "\"I guess I just never was a good person from the start...\"" +
            "\nHod's Qlipha has vanished from the Training Department... Core Suppression successfully completed.";
    }
}
