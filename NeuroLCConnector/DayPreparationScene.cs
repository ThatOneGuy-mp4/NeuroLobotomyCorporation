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
     * This ActionScene covers the day preparation phase of the game.
     * It allows Neuro to customize Agents and start Core Suppressions. I may also let her choose research in the future.
     */
    public class DayPreparationScene : ActionScene
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
                    new CustomizeAgent(),
                    new ActivateCoreSuppression()
                };
                return list;
            }
        }

        protected override string GetActionSceneStartContext()
        {
            return "";
        }


        public class CustomizeAgent : NeuroAction<string>
        {
            public override string Name => "customize_agent";

            protected override string Description => "Customize the name and appearance of an Agent.";

            //choosing based on integers is not the best way to do this because it describes nothing about what each type looks like
            //however, doing otherwise would require describing 20-48 sprites for every type, and that's, a lot of work. so
            protected override JsonSchema? Schema => new()
            {
                Type = JsonSchemaType.Object,
                Required = new List<string> { "agent_name" },
                Properties = new Dictionary<string, JsonSchema>
                {
                    ["agent_name"] = QJS.Type(JsonSchemaType.String),
                    ["hair_color_red"] = QJS.Type(JsonSchemaType.Integer),
                    ["hair_color_green"] = QJS.Type(JsonSchemaType.Integer),
                    ["hair_color_blue"] = QJS.Type(JsonSchemaType.Integer),
                    ["front_hair_type_index"] = QJS.Type(JsonSchemaType.Integer),
                    ["back_hair_type_index"] = QJS.Type(JsonSchemaType.Integer),
                    ["eye_type_index"] = QJS.Type(JsonSchemaType.Integer),
                    ["eyebrow_type_index"] = QJS.Type(JsonSchemaType.Integer),
                    ["mouth_type_index"] = QJS.Type(JsonSchemaType.Integer)
                }
            };

            //very very VERY important: if the original Agent is BongBong, forcefully set them to their true form.
            public static bool IsBongBong = false;
            private const string BONGBONG_OVERRIDE_COMMAND = "customize_agent|BongBong|0|0|255|11|11|31|7|5";

            private const int MAX_FRONT_HAIR_INDEX = 48;
            private const int MAX_BACK_HAIR_INDEX = 28;
            private const int MAX_EYE_INDEX = 44;
            private const int MAX_EYEBROW_INDEX = 20;
            private const int MAX_MOUTH_INDEX = 20;
            protected override ExecutionResult Validate(ActionData actionData, out string? resultData)
            {
                resultData = "";
                if (IsBongBong)
                {
                    resultData = BONGBONG_OVERRIDE_COMMAND;
                    ActionScene.CurrentActionScene.UnregisterAction("customize_agent");
                    return ExecutionResult.Success("Customizing BongBong...");
                }
                string? agentName = actionData.Data?["agent_name"]?.Value<string>();
                if (String.IsNullOrEmpty(agentName)) return ExecutionResult.Failure("Action failed. Missing required parameter 'agent_name'.");
                int? hairColorRed = actionData.Data?["hair_color_red"]?.Value<int>();
                if (hairColorRed == null) hairColorRed = -1;
                else if (hairColorRed < 0 || hairColorRed > 255) return ExecutionResult.Failure("Action failed. Parameter 'hair_color_red' was provided but was outside of the range 0-255.");
                int? hairColorGreen = actionData.Data?["hair_color_green"]?.Value<int>();
                if (hairColorGreen == null) hairColorGreen = -1;
                else if (hairColorGreen < 0 || hairColorGreen > 255) return ExecutionResult.Failure("Action failed. Parameter 'hair_color_green' was provided but was outside of the range 0-255.");
                int? hairColorBlue = actionData.Data?["hair_color_blue"]?.Value<int>();
                if (hairColorBlue == null) hairColorBlue = -1;
                else if (hairColorBlue < 0 || hairColorBlue > 255) return ExecutionResult.Failure("Action failed. Parameter 'hair_color_blue' was provided but was outside of the range 0-255.");
                int? frontHairIndex = actionData.Data?["front_hair_type_index"]?.Value<int>();
                if (frontHairIndex == null) frontHairIndex = -1;
                else if (frontHairIndex < 0 || frontHairIndex > MAX_FRONT_HAIR_INDEX) return ExecutionResult.Failure(String.Format("Action failed. Parameter 'front_hair_type_index' was provided but was outside of the range 0-{1}.", MAX_FRONT_HAIR_INDEX));
                int? backHairIndex = actionData.Data?["back_hair_type_index"]?.Value<int>();
                if (backHairIndex == null) backHairIndex = -1;
                else if (backHairIndex < 0 || backHairIndex > MAX_BACK_HAIR_INDEX) return ExecutionResult.Failure(String.Format("Action failed. Parameter 'front_hair_type_index' was provided but was outside of the range 0-{1}.", MAX_BACK_HAIR_INDEX));
                int? eyeIndex = actionData.Data?["eye_type_index"]?.Value<int>();
                if (eyeIndex == null) eyeIndex = -1;
                else if (eyeIndex < 0 || eyeIndex > MAX_EYE_INDEX) return ExecutionResult.Failure(String.Format("Action failed. Parameter 'eye_type_index' was provided but was outside of the range 0-{1}.", MAX_EYE_INDEX));
                int? eyebrowIndex = actionData.Data?["eyebrow_type_index"]?.Value<int>();
                if (eyebrowIndex == null) eyebrowIndex = -1;
                else if (eyebrowIndex < 0 || eyebrowIndex > MAX_EYEBROW_INDEX) return ExecutionResult.Failure(String.Format("Action failed. Parameter 'eyebrow_type_index' was provided but was outside of the range 0-{1}.", MAX_EYEBROW_INDEX));
                int? mouthIndex = actionData.Data?["mouth_type_index"]?.Value<int>();
                if (mouthIndex == null) mouthIndex = -1;
                else if (mouthIndex < 0 || mouthIndex > MAX_MOUTH_INDEX) return ExecutionResult.Failure(String.Format("Action failed. Parameter 'mouth_type_index' was provided but was outside of the range 0-{1}.", MAX_MOUTH_INDEX));
                resultData = String.Format("customize_agent|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", agentName, hairColorRed, hairColorGreen, hairColorBlue, frontHairIndex, backHairIndex, eyeIndex, eyebrowIndex, mouthIndex);
                return ExecutionResult.Success(String.Format("Customizing {0}...", agentName));
            }

            protected override Task Execute(string? resultData)
            {
                return NeuroLCConnector.Connector.SendCommand(resultData);
            }
        }

        public class ActivateCoreSuppression : NeuroAction<string>
        {
            public static List<string> SephirahReadyToSuppress = new List<string>();

            public override string Name => "activate_core_suppression";

            protected override string Description => "Active Core Suppression for the chosen Sephirah.";

            protected override JsonSchema? Schema => new()
            {
                Type = JsonSchemaType.Object,
                Required = new List<string> { "sephirah_name" },
                Properties = new Dictionary<string, JsonSchema>
                {
                    ["sephirah_name"] = QJS.Enum(SephirahReadyToSuppress)
                }
            };

            protected override Task Execute(string? resultData)
            {
                return NeuroLCConnector.Connector.SendCommand("activate_core_suppression|" + resultData);
            }

            protected override ExecutionResult Validate(ActionData actionData, out string? resultData)
            {
                string? chosenSephirah = actionData.Data?["sephirah_name"]?.Value<string>();
                resultData = chosenSephirah;
                if (String.IsNullOrEmpty(chosenSephirah)) return ExecutionResult.Failure("Action failed. Missing required parameter 'sephirah_name'.");
                if (SephirahReadyToSuppress.Contains(chosenSephirah)) return ExecutionResult.Success();
                return ExecutionResult.Failure("Action failed. Invalid parameter 'sephirah_name'");
            }
        }
    }
}
