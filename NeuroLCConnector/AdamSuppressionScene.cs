using NeuroSDKCsharp.Messages.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class AdamSuppressionScene : CoreSuppressionBaseScene
    {

        private int altPhase;

        protected override List<string> PhaseTransitionDialogue => new List<string>()
        {
            "UNUSED",
            "\"People live every day looking up to a hope they cannot reach while shedding tears of pain. We have the power to save them, should we not rightfully use it? ...We must use this power if we have it.\"" +
            "\n\nAdam has been agitated. All errors remain stagnant.",
            "\"The path I traveled was riddled with thorns; it was a penance with no end in sight. You know how many sacrifices have been made to come this far. We cannot let them be in vain. So join me.\"" +
            "\n\nAdam has been agitated. All errors remain stagnant.",
            "\"We are all sinners. Our imperfect eyes could not see the world properly. It is time that I purified them.\"" +
            "\n\nAdam has been agitated. All errors remain stagnant.",
            "\"The desires humanity has forsaken are floating deep under the stream. People shall be forever isolated from themselves and will never be free if they stay this way. ...They thirst for pure desires. We shall fill their glasses with the juice and wine of the forbidden fruit.\"" +
            "\n\"Humanity had to give up on freedom and desire for survival, but do you think those are truly gone? No, they merely have been waiting for the chance to rise again. People are too ignorant to see what is occurring in the world above them. However, ignorance is not a sin. Disbelief is.\"" +
            "\n\nWARNING!!! Adam's madness has reawoken another Qlipha... An Arbiter awakens in the Extraction Department." +
            "\nHokma's Qlipha's control over time has been shaken...I have repaired your latency. Good luck...you will need it.",
            "\"The sky shall fall, and mankind shall become holy. They will reach their promised glory as they change in the blink of an eye. ...I know that you are not certain of what lies ahead for you. The future you yearn for may never come. I can show a proper future to you instead, right before your eyes.\"" +
            "\n\"Freedom is a necessity for one to be truly human. ...Let go of your fear and gain true freedom. Doing so will also cure the disease she tried to heal.\"",
            "\"They are moaning in agony, even as we speak. Why can you not hear them? ...Just one bite of this sweet fruit will redeem them, all of them!\"",
            "\"...\"",
            "\"...\""
        };

        private List<string> QliphothPhaseTransitionDialogueSuffix => new List<string>
        {
            "UNUSED",
            "SHARED",
            "SHARED",
            "SHARED",
            "SHARED",
            "Adam has been agitated. All errors remain stagnant.",
            "Adam has been agitated. All errors remain stagnant.",
            "Adam has been agitated. All errors remain stagnant.",
            "Adam has been disturbed. Hokma's Qlipha regains full power...the anomaly in the flow of time has intensified; however, An Arbiter has ceased movement." +
            "\nRealize your knowledge. Finish this.",
            "Adam has been disturbed. The anomaly in the flow of time has intensified."
        };

        private List<string> ArbiterPhaseTransitionDialogueSuffix => new List<string>
        {
            "UNUSED",
            "SHARED",
            "SHARED",
            "SHARED",
            "SHARED",
            "An Arbiter has been brought to her knees, but she is not satisfied. She has gotten back up.",
            "An Arbiter has been brought to her knees again, but is still not yet satisfied..." + 
            "\nRealize your knowledge. Finish this."
        };

        protected override string SuppressionCompleteDialogue => "\"How shameful. Do you truly want to let people suffocate, trapped away in their skin?! ...Can you not hear the trumpets...?\"" +
            "\nAdam, and the Qlipha of Atziluth, have been permanently suppressed... Curtain Call for Act IV...no. Keter's Core Suppression, as a whole...was successfully completed.";

        private string SuppressionFailureDialogue => "...I don't blame you for this. I would like you to know that." +
            "\nAct IV of Keter's Core Suppression has been cut short.";

        public override async Task ChangePhase()
        {
            if (phase < 4)
            {
                await base.ChangePhase();
                if (phase == 4)
                {
                    altPhase = 4;
                }
            }
            else
            {
                phase++;
                if (phase < PhaseTransitionDialogue.Count)
                {
                    string dialogue = "";
                    if (phase > altPhase)
                    {
                        dialogue = PhaseTransitionDialogue[phase] + "\n\n";
                    }
                    dialogue += QliphothPhaseTransitionDialogueSuffix[phase];
                    Context.Send(dialogue, true);
                }
            }
        }

        public async Task ChangeAltPhase()
        {
            altPhase++;
            if (altPhase < PhaseTransitionDialogue.Count)
            {
                string dialogue = "";
                if (altPhase > phase)
                {
                    dialogue = PhaseTransitionDialogue[altPhase] + "\n\n";
                }
                dialogue += ArbiterPhaseTransitionDialogueSuffix[altPhase];
                Context.Send(dialogue, true);
            }
        }

        public void FailCoreSuppression()
        {
            CleanUpActionScene();
            Context.Send(SuppressionFailureDialogue, false);
        }

        public async Task Stall()
        {
            if(phase < 4) await HokmaSuppressionScene.InduceLatency(phase);
        }
    }
}
