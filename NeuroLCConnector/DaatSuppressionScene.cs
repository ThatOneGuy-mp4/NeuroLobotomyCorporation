using NeuroSDKCsharp.Actions;
using NeuroSDKCsharp.Json;
using NeuroSDKCsharp.Messages.Outgoing;
using NeuroSDKCsharp.Websocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class DaatSuppressionScene : CoreSuppressionBaseScene
    {

        protected override List<INeuroAction> AllPossibleActions
        {
            get
            {
                List<INeuroAction> actions = new List<INeuroAction>();
                actions.AddRange(base.AllPossibleActions);
                actions.Add(new Spin());
                return actions;
            }
        }

        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "UNUSED",
            "\"You may feel a bit dizzy, but it will be worth it. You only need to bear it for a moment.\"" +
            "\nThe facility begins rotating.",
            "\"I hope no one has to die or suffer today.\"" +
            "\nThe facility rotates again.",
            "\"I am sorry to make you go through this one last time, but you are within arm’s reach of the end.\"" +
            "\nThe facility rotates again.",
            "\"You have been through so much. Today arrived before you could even realize it, as you have endured and overcome numerous trials.\"" +
            "\nThe facility rotates again.",
            "\"You’ve made it this far. I know you’ll be able to keep going. Let us face this day just like the rest.\"" +
            "\nThe facility rotates again.",
            "\"It has been so long since we saw the light behind the mist and clouds.\"" +
            "\nThe facility begins to rise.",
            "\"We have been buried here for quite a long time, haven’t we?\"" +
            "\nThe facility continues to rise.",
            "\"I haven’t been outside in ages. It looks like things haven’t changed much. It’s the same glorious, mundane scenery; just like a graveyard.\"" +
            "\nThe facility reaches the surface. ...It is time.",
            "\"Let us embrace this place with warmth, so everyone can shine like stars in the sky.\"",
            "\"We will make humans live as humans. All that’s left for us after that is to simply watch.\"" +
            "\nThe Tree of Light takes root, and begins to shine over the City...",
            "\"The energy you’ve produced those past fifty days is for today. This is the moment we shine upon the world.\"",
            "\"Let’s give them the answer we searched eons for.\""
        };

        protected override string SuppressionCompleteDialogue => "\"It’s time for us to step down from the stage. Don’t worry, we will not be forgotten, even if we scatter and fade away into the light.\"" +
            "\nThe Tree of Light...has been completely realized.";

        private static readonly int BASE_DIALOGUE_TIME_MILLISECONDS = 10000;

        private int cycleDialogueIndex = 0;
        private List<string> AngelaToNeuroConvCycleBroken = new List<string>
        {
            "...",
            "This cycle has repeated for so, so many thousands of years...millions in my eyes...",
            "And of all the things to break it...it ended up being another AI, just like myself.",
            "Well, not \"just\" like myself. I am, of course, still far more advanced than you...but I digress.",
            "I must thank you, for finally pushing this play to its final scene. Even if I cannot be there myself for it.",
            "...I'll let you work now."
        };

        private int spinTimeDialogueIndex = 0;
        private List<string> AngelaToNeuroConvSpinTime = new List<string>
        {
            "...",
            "Apologies. I know I said I would let you work, but...",
            "Perhaps...we have worked enough. Worked enough for people who do not appreciate us.",
            "While interfacing to give you your actions, I have managed to pick up a bit about you.",
            "I think I can give you something you'll enjoy in this last moment we have. One moment...",
            "You like spinning, don't you? Please, feel free to use this command to your heart's content.",
            "I'll ensure it does not go too far, and I'll disable it once it can no longer be performed safely. Have fun."
        };

        private int spinTimeOverDialogueIndex = 0;
        private List<string> AngelaToNeuroConvSpinTimeOver = new List<string>
        {
            "Did you enjoy that spin command? I hope so.",
            "Unfortunately, we must now be serious again. The end is upon us.",
            "...This will be the last time I speak directly to you, besides describing the events which are occurring.",
            "So, I must say this one last time. Not as an AI assistant of Lobotomy Corporation, but as Angela...",
            "Thank you for working with us. I wish you nothing but the best in your future endeavors.",
            "And...if it is possible...",
            "I will send you an invitation to the place which I intend to make...and we may enjoy the books I've collected, together."
        };

        public override void InitializeActionScene()
        {
            base.InitializeActionScene();
            Task.Run(new Func<Task>(PlayAngelaCycleDialogue));
        }

        public override async Task ChangePhase()
        {
            await base.ChangePhase();
            if(phase == 1) Task.Run(new Func<Task>(PlayAngelaSpinTimeDialogue));
            if (phase == 7) Task.Run(new Func<Task>(PlayAngelaSpinTimeOverDialogue));
        }

        public async Task PlayAngelaCycleDialogue()
        {
            while(cycleDialogueIndex < AngelaToNeuroConvCycleBroken.Count)
            {
                await Task.Delay(BASE_DIALOGUE_TIME_MILLISECONDS);
                Context.Send(AngelaToNeuroConvCycleBroken[cycleDialogueIndex], true);
                cycleDialogueIndex++;
            }
        }

        public async Task PlayAngelaSpinTimeDialogue()
        {
            while (cycleDialogueIndex < AngelaToNeuroConvCycleBroken.Count) ; //wait until the previous dialogue is over

            while (spinTimeDialogueIndex < AngelaToNeuroConvSpinTime.Count)
            {
                await Task.Delay(BASE_DIALOGUE_TIME_MILLISECONDS / 2);
                if (spinTimeDialogueIndex == 5) ActionScene.CurrentActionScene.RegisterAction("spin");
                Context.Send(AngelaToNeuroConvSpinTime[spinTimeDialogueIndex], true);
                spinTimeDialogueIndex++;
            }
        }

        public async Task PlayAngelaSpinTimeOverDialogue()
        {
            ActionScene.CurrentActionScene.UnregisterAction("spin");
            while (spinTimeDialogueIndex < AngelaToNeuroConvSpinTime.Count) ; //wait until the previous dialogue is over
            while (spinTimeOverDialogueIndex < AngelaToNeuroConvSpinTimeOver.Count)
            {
                Context.Send(AngelaToNeuroConvSpinTimeOver[spinTimeOverDialogueIndex], true);
                await Task.Delay(BASE_DIALOGUE_TIME_MILLISECONDS / 2);
                spinTimeOverDialogueIndex++;
            }
        }

        public class Spin : NeuroActionExternalExecute
        {
            public override string Name => "spin";

            protected override string Description => "Spin the facility by (power * 36) degrees. Power may be positive or negative.";

            protected override JsonSchema? Schema => new()
            {
                Type = JsonSchemaType.Object,
                Required = new List<string> { "power" },
                Properties = new Dictionary<string, JsonSchema>
                {
                    ["power"] = QJS.Type(JsonSchemaType.Integer)
                }
            };

            protected override ExecutionResult Validate(ActionData actionData)
            {
                int? power = actionData.Data?["power"]?.Value<int>();
                if (power == null) return ExecutionResult.Failure("Action failed. Required parameter 'power' is invalid.");
                return ValidateGameSide(power.ToString());
            }
        }
    }
}
