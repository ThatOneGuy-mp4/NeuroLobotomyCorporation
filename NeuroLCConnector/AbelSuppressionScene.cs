using NeuroSDKCsharp.Messages.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class AbelSuppressionScene : CoreSuppressionBaseScene
    {
        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "UNUSED",
            "\"Remember Elijah’s last moments… how she clawed and scraped at the floor, writhing in pain. ...How do we expect to move forward when we cannot even stand up straight?\"" +
            "\n\nAbel has been agitated. The error with the work assignment system has mutated.",
            CreateScrambledPhaseTransition(2),
            CreateScrambledPhaseTransition(3),
            CreateScrambledPhaseTransition(4),
            CreateScrambledPhaseTransition(5)
        };

        private List<string> PhaseTransitionAbelDialogue => new List<string>
        {
            "UNUSED",
            "UNUSED",
            "\"Our hearts overflowed with despair as we prepared for tomorrow, acting as if nothing had happened, even after the deaths of so many of our loved ones. ...The suppressed feelings were so severe, it caused phantom pains and hallucinations of bugs crawling on our skin...\"" +
            "\n\"Obsession with the rules, not letting ourselves feel an inch of sadness...We had no other way to cope but that. You would agree with this. ...Yet we had to do it, we had come too far. However, one accident led to a complete breakdown, like the collapse of a tower.\"",
            "\"Michelle was the youngest among us. She was a timid and innocent employee. She chose to be a whistleblower out of fear for the world. If we had just paid a bit more attention to her, she may not have run away so scared.\"" +
            "\n\"Everything we thought to be hope came back to us in the shape of despair. ...How will we improve ourselves out of this? It will only get worse and worse.\"",
            "\"You told Giovanni that you could revive Carmen. But think about it, did you really lead him to eternal slumber because you sincerely believed so? ...Going further than this is futile. Please, you have to just accept that it is over.\"" +
            "\n\"I survived alone. I’ve lived day by day, fighting an uphill battle. I fought because I thought my survival would somehow change the situation. How foolish I was. ...Perhaps our souls have already died, a long, long time ago. It may be just as Carmen feared.\"",
            "\"The world has abandoned us! We tried not to let it go, but it threw us away like a crumpled up newspaper in the end. ...We were too naive. I could only realize what wrongdoings we have done after transiently passing all these years.\"" +
            "\n\"You will overcome this despair if you just wake up and smell the roses. You still have a chance. Please, listen to what I have to say.\""
        };

        private List<string> PhaseTransitionFacilityStatusDialogue = new List<string>
        {
            "UNUSED",
            "UNUSED",
            "Abel has been disturbed. Errors with the information and camera systems are detected. Additionally, the error with the work assignment system has mutated.",
            "Abel has been disturbed. An error with our employees' statuses is detected. All Agents have reported feeling comatose. Addtionally, the error with the work assignment system has mutated.",
            "Abel has been disturbed. An error with the healing and recovery systems, and an error with the work cancellation system, is detected. Additionally, the error with the work assignment system has mutated." +
            "\nOne who does not move cannot block the way for long... Curtain Call approaches.",
            "Abel has been agitated. The error with the work assignment system has mutated. Something within Abel stirs...all Agents have had their HP and SP completely healed."
        };

        protected override string SuppressionCompleteDialogue => "\"A brief struggle has become an eternity. I just wish that you do not repeat the mistake I made and fall into despair and anger yet again.\"" +
            "\nAbel, and the Qlipha of Asiyah, have been permanently suppressed... Curtain Call for Act II of Keter's Core Suppression.";

        private string SuppressionFailureDialogue => "...perhaps he was not ready after all." +
            "\nAct II of Keter's Core Suppression has been cut short. I am sorry.";

        public void FailCoreSuppression()
        {
            CleanUpActionScene();
            Context.Send(SuppressionFailureDialogue, false);
        }

        public string GetScrambledMessage(string context)
        {
            if (phase < 2) return context;
            return YesodSuppressionScene.ScrambleContext(context, phase / 2);
        }

        private string CreateScrambledPhaseTransition(int targetPhase)
        {
            string abelDialogue = PhaseTransitionAbelDialogue[targetPhase];
            string facilityStatusDialogue = PhaseTransitionFacilityStatusDialogue[targetPhase];
            facilityStatusDialogue = YesodSuppressionScene.ScrambleContext(facilityStatusDialogue, targetPhase / 2);
            return abelDialogue + "\n\n" + facilityStatusDialogue;
        }

    }
}
