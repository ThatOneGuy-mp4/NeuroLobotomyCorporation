using NeuroSDKCsharp.Actions;
using NeuroSDKCsharp.Messages.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class AbramSuppressionScene : CoreSuppressionBaseScene
    {

        protected override List<INeuroAction> AllPossibleActions
        {
            get
            {
                List<INeuroAction> actions = new List<INeuroAction>();
                actions.AddRange(base.AllPossibleActions);
                actions.Add(new Poke());
                return actions;
            }
        }

        private int altPhase;

        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "UNUSED",
            "\"We tried not to put trust in anyone, but we shamelessly woke up our colleagues who had fallen into eternal slumber when we needed them... We destroyed everything while holding her warmth in our hands.\"" +
            "\n\nAbram has been agitated. The anomaly with the amount of damage employees receive has mutated.",
            "\"Don’t hold hope in anything. For the sake of our employees who died hopelessly. ...Tell me, what great purpose did we fulfill with all those actions?\"" +
            "\n\nAbram has been agitated. The anomaly with the amount of damage employees receive has mutated.",
            "\"If existence itself is an ailment to us, there is only one way to cure it. Just shut your eyes, and never open them again.\"" +
            "\n\nAbram has been agitated. The anomaly with the amount of damage employees receive has mutated.",
            "\"You have to let it all go, you don’t even have the strength to grip onto anything... The things I’ve let go have drifted too far for me to take up into my hands again.\"" +
            "\n\nWARNING!! Abram's hopelessness has reawoken another Qlipha... The Red Mist makes her return once again in the Disciplinary Department." +
            "\nHer presence has overwritten Chesed's Qlipha...the anomaly in the amount of damage employees receive has been disabled. However, if you continue to perform works rather than suppressing The Red Mist, it will reemerge.",
            "\"You will never escape your guilt if you stay that way. You will curse your existence, and every single day will be painful.\"" +
            "\n\"Carmen left without a smile, never to come back to nap under the warm sun. ...Why must I be abandoned by the world, alone like this?\"",
            "\"...It was my duty to protect my coworkers, my friends... but I couldn’t save even a single one of them.\"" +
            "\n\"Yes, there was only a path of despair in the sky she often looked up to. ...Desire is void and meaningless. Can’t you hear it? The doors are closing, and they will not open again.\"",
            "\"The scene, the air, the pain of that day... It all comes back no matter how many times I try to void it from my mind. Tell me, how do I escape from this?\"" +
            "\n\"Do we really deserve to persist in existing? Meanwhile as we step on all those sacrifices? ...Let us sleep peacefully... Let us sink and flow to the bottom of the river with Carmen. She must have been lonely for such a long time...\"",
            "..."
        };

        private List<string> QliphothPhaseTransitionDialogueSuffix => new List<string>
        {
            "UNUSED",
            "SHARED",
            "SHARED",
            "SHARED",
            "SHARED",
            "Abram has been agitated. Chesed's Qlipha stirs, but nothing happened.",
            "Abram has been agitated. Chesed's Qlipha stirs...it is soon to awaken again.",
            "Abram has been disturbed. Chesed's Qlipha has reawoken; the anomaly in the damage employees receive has returned, intensified, and mutated." +
            "\nOne without hope can only struggle so long... Curtain Call approaches.",
            "Abram has been agitated. The anomaly in the damage employees receive has mutated."
        };

        private List<string> RedMistPhaseTransitionDialogueSuffix => new List<string>
        {
            "UNUSED",
            "SHARED",
            "SHARED",
            "SHARED",
            "SHARED",
            "The Red Mist has received substantial damage, but she is still moving. She is preparing to discard Red Eyes & Penitence, and equip Mimicry & De Capo.",
            "The Red Mist's core has cracked, but she is still moving. Be cautious of her throwing away Mimicry & De Capo as she equips Smile & Justitia.",
            "The Red Mist's body has almost completely been destroyed, but she refuses to fall. She destroys Smile, and awakens both Twilight and her own Effloresced E.G.O." +
            "\nOne without hope can only struggle so long... Curtain Call approaches.",
            "\"...\""
        };

        protected override string SuppressionCompleteDialogue => "\"...I’m perfectly fine with my life expiring here and now. Only stigma awaits those who cannot overcome the trial.\"" +
            "\nAbram, and the Qlipha of Briah, have been permanently suppressed... Curtain Call for Act III of Keter's Core Suppression.";

        private string SuppressionFailureDialogue => "...Bloodbath calls to him." +
            "\nAct III of Keter's Core Suppression has been cut short. I am sorry.";

        public override async Task ChangePhase()
        {
            if (phase < 4)
            {
                await base.ChangePhase();
                if (phase == 4) 
                { 
                    altPhase = 4;
                    RegisterAction("poke");
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
                dialogue += RedMistPhaseTransitionDialogueSuffix[altPhase];
                Context.Send(dialogue, true);
            }
        }

        public void FailCoreSuppression()
        {
            CleanUpActionScene();
            Context.Send(SuppressionFailureDialogue, false);
        }
    }
}
