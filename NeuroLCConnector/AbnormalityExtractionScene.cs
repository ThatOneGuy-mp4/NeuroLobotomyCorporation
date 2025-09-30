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
     * TODO: 
     * -Add Action to reextract once unlocked
     */
    public class AbnormalityExtractionScene : ActionScene
    {
        protected override List<INeuroAction> InitActions
        {
            get
            {
                List<INeuroAction> actions = new List<INeuroAction>
                {
                    new ExtractAbnormality()
                };
                return actions;
            }
        }

        protected override List<INeuroAction> AllPossibleActions
        {
            get
            {
                if(allPossibleActions == null)
                {
                    allPossibleActions = new List<INeuroAction>
                    {
                        new ExtractAbnormality()
                    };
                }
                return allPossibleActions;
            }
        }

        public static List<string> AbnoNames
        {
            get
            {
                return abnoNames;
            }
            set
            {
                abnoNames = value;
            }
        }
        private static List<string> abnoNames;

        //TODO: add the ability to reextract
        private bool canReextract;

        //TODO: add the taglines for the abnormalities to the context
        protected override string GetActionSceneStartContext()
        {
            string context = "It is time to extract a new Abnormality to add to the facility. Your choices are: ";
            foreach(string name in AbnoNames)
            {
                context += "\n" + name;
            }
            return context;
        }
    }

    public class ExtractAbnormality : NeuroAction<string>
    {
        public override string Name => "extract_abnormality";
        protected override string Description => "Choose the Abnormality that will be added to the facility.";
        protected override JsonSchema? Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = new List<string> { "abnormality_name" },
            Properties = new Dictionary<string, JsonSchema>
            {
                ["abnormality_name"] = QJS.Enum(AbnormalityExtractionScene.AbnoNames)
            }
        };

        protected override ExecutionResult Validate(ActionData actionData, out string? resultData)
        {
            string? chosenAbno = actionData.Data?["abnormality_name"]?.Value<string>();
            resultData = chosenAbno;
            if (String.IsNullOrEmpty(chosenAbno)) return ExecutionResult.Failure("Action failed. Missing required parameter 'abnormality_name'.");
            if (AbnormalityExtractionScene.AbnoNames.Contains(chosenAbno)) return ExecutionResult.Success();
            return ExecutionResult.Failure("Action failed. Invalid parameter 'abnormality_name'");
        }

        protected override Task Execute(string? resultData)
        {
            return NeuroLCConnector.Connector.SendCommand("extract_abnormality|" + resultData);
        }
    }
}
