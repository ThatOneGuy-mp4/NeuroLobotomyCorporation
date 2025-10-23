using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class YesodSuppressionScene : CoreSuppressionBaseScene
    {
        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "UNUSED",
            CreateScrambledPhaseTransition(1),
            CreateScrambledPhaseTransition(2),
            CreateScrambledPhaseTransition(3),
            CreateScrambledPhaseTransition(4)
        };

        private string YesodStartSuppressionDialogue_1 => "\"What did we do wrong? ...Are we not even allowed to fall into despair?\"";
        private string YesodStartSuppressionDialogue_2 => "\"This impenetrable darkness is inexplicably lonely. ...What was I really looking at all this time?\"" +
            "\n\"You will not be able to discern anything properly.\"";

        protected override string GetActionSceneStartContext()
        {
            string unscrambledFacilityStatusStartContext = base.GetActionSceneStartContext();
            string facilityStatusStartContext = ScrambleContext(unscrambledFacilityStatusStartContext, 0);
            return YesodStartSuppressionDialogue_1 + "\n\n" + facilityStatusStartContext + "\n\n" + YesodStartSuppressionDialogue_2;
        }

        private List<string> PhaseTransitionYesodDialogue => new List<string>
        {
            "UNUSED",
            "\"To tell you the truth, it’s just that I did not want to witness any more deaths. ...We’ve created monsters that should never have been in this world. This sin can never be forgotten.\"",
            "\"I am the head of the Information Team. I take great pride in my department and team for obtaining information and expunging it exactly as ordered. ...You should know that all the information you just mindlessly skimmed through was from somebody’s dire sacrifice.\"",
            "\"You are unfit to be the manager if you cannot even remember the simplest rules. ...Did you not see my marred, rotting flesh, covered in boils and oozing with pus?\"",
            "\"Now I see; I have been wallowing in despair, for such a long time. ...I tried to pretend that everything was fine, but in reality, it never was. I was dying on the inside...\""
        };

        private List<string> PhaseTransitionFacilityStatusDialogue => new List<string>
        {
            "UNUSED",
            "Yesod's Qlipha has been disturbed. The errors with the information and camera systems have greatly intensified.",
            "Yesod's Qlipha has been agitated. The error with the information system has intensified.",
            "Yesod's Qlipha has been disturbed. The errors with the information and camera systems have greatly intensified." +
            "\nYesod's core cannot sustain this level of corruption for much longer...please maintain rationality in the last stretch of this suppression.",
            "Yesod's Qlipha has been agitated. The error with the information system has reached critical levels." 
        };

        protected override string SuppressionCompleteDialogue => "\"So I was the one who couldn’t see a single step ahead...\"" +
            "\nYesod's Qlipha has vanished from the Information Department... Core Suppression successfully completed.";

        private static Random rand = new Random();
        private string CreateScrambledPhaseTransition(int targetPhase)
        {
            string yesodDialogue = PhaseTransitionYesodDialogue[targetPhase];
            string facilityStatusDialogue = PhaseTransitionFacilityStatusDialogue[targetPhase];
            facilityStatusDialogue = ScrambleContext(facilityStatusDialogue, targetPhase);
            return yesodDialogue + "\n\n" + facilityStatusDialogue;
        }

        public string GetScrambledMessage(string context)
        {
            return ScrambleContext(context, phase);
        }

        private static readonly int MIN_SCRAMBLE_CHANCE = 1;
        private static readonly int MAX_SCRAMBLE_CHANCE = 8;
        public static string ScrambleContext(string context, int intensity)
        {
            int scrambleChance = MIN_SCRAMBLE_CHANCE + intensity;
            List<char> charactersToScramble = new List<char>();
            string scrambledMessageTemplate = "";
            foreach (char c in context.ToCharArray())
            {
                if (c.Equals('\n') || c.Equals(' ') ||  !(rand.Next(0, MAX_SCRAMBLE_CHANCE) < scrambleChance))
                {
                    scrambledMessageTemplate += c;
                }
                else
                {
                    charactersToScramble.Add(c);
                    scrambledMessageTemplate += "|";
                }
            }
            string scrambledMessage = "";
            foreach (char c in scrambledMessageTemplate.ToCharArray())
            {
                if (c.Equals('|'))
                {
                    char randomChar = charactersToScramble[rand.Next(charactersToScramble.Count)];
                    charactersToScramble.Remove(randomChar);
                    scrambledMessage += randomChar;
                }
                else
                {
                    scrambledMessage += c;
                }
            }
            return scrambledMessage;
        }
    }
}
