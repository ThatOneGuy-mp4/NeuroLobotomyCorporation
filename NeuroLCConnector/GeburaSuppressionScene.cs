using NeuroSDKCsharp.Actions;
using NeuroSDKCsharp.Json;
using NeuroSDKCsharp.Messages.Outgoing;
using NeuroSDKCsharp.Websocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class GeburaSuppressionScene : CoreSuppressionBaseScene
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

        public override void InitializeActionScene()
        {
            base.InitializeActionScene();
            if (angelaConversationSaidBefore)
            {
                RegisterAction("poke");
                Context.Send("The 'poke' command has been reauthorized. May the suppression proceed smoothly.", true);
            }
        }

        private static bool angelaConversationSaidBefore = false;
        public async Task GivePokeAction()
        {
            if (!angelaConversationSaidBefore)
            {
                await Task.Delay(15000);
                Context.Send("Hm...this suppression requires high levels of micro-management. It would be no problem for an AI such as myself to handle, but I fear your response time may be too slow in this scenario..." +
                    "\nAllow me a moment to find you something to do.", false);
                await Task.Delay(60000); //exactly one minute has been allowed LULE
                if (RegisterAction("poke"))
                {
                    Context.Send("You are authorized to use the 'poke' command. It may not seem like much, but it is sometimes said that slow and steady wins the race.", false);
                }
                else
                {
                    Context.Send("You are authorized to use the 'poke' command. It may not seem like much, but it is sometimes said that slow and steady wins the race." +
                        "\n...ah, but it would seem the registering of the action has failed...well, it shall be waiting when the next attempt has started.", false);
                }
                angelaConversationSaidBefore = true;
            }
        }

        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "UNUSED",
            "\"Some things simply couldn’t be forgotten, no matter how much time has passed. ...Hatred is a poison that eviscerates me, and yet it makes me open my eyes once more.\"" +
            "\n\nThe Red Mist has received substantial damage, but she is still moving. She is preparing to discard Red Eyes & Penitence, and equip Mimicry & De Capo.",
            "\"Some things just wouldn’t cool down, no matter how long they were left in the cold. ...Those monsters always kill people, there is no end to this sin; I have descended to bring about their reckoning.\"" +
            "\n\nThe Red Mist's core has cracked, but she is still moving. Be cautious of her throwing away Mimicry & De Capo as she equips Smile & Justitia." +
            "\n\n\"This isn't enough. ...I wasn’t slacking off all this time.\"",
            "\"The right path is too far away, and I have too far to go. My heart is pulsing with anger, and I must keep pushing. ...Let me show you how to actually wield E.G.O.\"" +
            "\n\nThe Red Mist's body has almost completely been destroyed, but she refuses to fall. Face her with courage as she destroys Smile, and awakens both Twilight and her own Effloresced E.G.O." +
            "\n\n\"Let’s put an end to this... The apocalypse is here...\""
        };

        protected override string SuppressionCompleteDialogue => "\"Ah... I'm breaking... I won't drop my sword, even if I turn to dust...\"" +
            "\nGebura's Qlipha has been driven out... Core Suppression successfully completed.";
    }

    public class Poke : NeuroActionExternalExecute
    {
        public override string Name => "poke";

        protected override string Description => "Distract the Red Mist by dealing 1 damage to her.";

        protected sealed override JsonSchema? Schema => new()
        {
            Type = JsonSchemaType.Null
        };

        protected override ExecutionResult Validate(ActionData actionData)
        {
            return ValidateGameSide();
        }
    }
}
