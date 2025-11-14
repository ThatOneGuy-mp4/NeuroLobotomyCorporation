using NeuroSDKCsharp.Actions;
using NeuroSDKCsharp.Json;
using NeuroSDKCsharp.Websocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    /*
     * This ActionScene covers the story watching section of the game.
     * Aside from allowing Neuro to choose dialogue, it does nothing, as all the dialogue formatting is handled game-side and sent as context.
     * Mostly just exists to clear Neuro's actions from the previous ActionScene.
     */
    public class WatchStoryScene : ActionScene
    {
        protected override List<INeuroAction> InitActions
        {
            get
            {
                List<INeuroAction> list = new List<INeuroAction>
                {
                };
                return list;
            }
        }

        protected override List<INeuroAction> AllPossibleActions
        {
            get
            {
                List<INeuroAction> list = new List<INeuroAction>
                {
                    new SelectDialogue()
                };
                return list;
            }
        }

        protected override string GetActionSceneStartContext()
        {
            return "";
        }

        public void SetDialogueOptions(string[] parameters)
        {
            List<string> options = new List<string>();
            for(int i = 1; i < parameters.Length; i++)
            {
                options.Add(parameters[i]);
            }
            SelectDialogue.DialogueOptions = options;
        }

        public class SelectDialogue : NeuroAction<string>
        {
            public override string Name => "select_dialogue";

            protected override string Description => "Select which dialogue you think the manager should respond with.";

            public static List<string> DialogueOptions = new List<string>();

            protected override JsonSchema? Schema => new()
            {
                Type = JsonSchemaType.Object,
                Required = new List<string> { "selected_dialogue" },
                Properties = new Dictionary<string, JsonSchema>
                {
                    ["selected_dialogue"] = QJS.Enum(DialogueOptions)
                }
            };

            protected override ExecutionResult Validate(ActionData actionData, out string? resultData)
            {
                string? selectedDialogue = actionData.Data?["selected_dialogue"]?.Value<string>();
                resultData = selectedDialogue;
                if (String.IsNullOrEmpty(selectedDialogue)) return ExecutionResult.Failure("Action failed. Missing required parameter 'selected_dialogue'.");
                if (SelectDialogue.DialogueOptions.Contains(selectedDialogue)) return ExecutionResult.Success();
                return ExecutionResult.Failure("Action failed. Invalid parameter 'selected_dialogue'");
            }

            protected override Task Execute(string? resultData)
            {
                return NeuroLCConnector.Connector.SendCommand("select_dialogue|" + resultData);
            }
        }
    }
}
