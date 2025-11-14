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
    public class AbnormalityExtractionScene : ActionScene
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
                    new ExtractAbnormality(),
                    new ReextractAbnormalities()
                };
                return list;
            }
        }

        private static bool canExtract;
        private static bool canReextract;

        public static void ParseParameters(string[] parameters)
        {
            if (parameters[2].Equals("true")) canReextract = true;
            else canReextract = false;
            if (parameters[3].Equals("NO_EXTRACTION")) { canExtract = false; return; }//depending on whether extraction is possible, parameter[3] is either the first abnormality's name or "NO_EXTRACTION"
            else canExtract = true;
            List<string> names = new List<string>();
            List<string> taglines = new List<string>();
            for(int i = 3; i < parameters.Length; i += 2)
            {
                names.Add(parameters[i]);
                taglines.Add(parameters[i + 1]);
            }
            AbnormalityNames = names;
            AbnormalityTaglines = taglines;
        }

        public static List<string> AbnormalityNames
        {
            get
            {
                return abnormalityNames;
            }
            set
            {
                abnormalityNames = value;
            }
        }
        private static List<string> abnormalityNames;

        public static List<string> AbnormalityTaglines
        {
            get
            {
                return abnormalityTaglines;
            }
            set
            {
                abnormalityTaglines = value;
            }
        }
        private static List<string> abnormalityTaglines;

        public override void InitializeOptionalActions()
        {
            if (canExtract) RegisterAction("extract_abnormality");
            else return;
            if (canReextract) RegisterAction("reextract_abnormalities");
        }

        protected override string GetActionSceneStartContext()
        {
            if (!canExtract) return "";
            string context = "It is time to extract a new Abnormality to add to the facility. Your choices are: ";

            for(int i = 0; i < AbnormalityNames.Count; i++)
            {
                context += String.Format("\n{0}: {1}", AbnormalityNames[i], AbnormalityTaglines[i]);
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
                ["abnormality_name"] = QJS.Enum(AbnormalityExtractionScene.AbnormalityNames)
            }
        };

        protected override ExecutionResult Validate(ActionData actionData, out string? resultData)
        {
            string? chosenAbnormality = actionData.Data?["abnormality_name"]?.Value<string>();
            resultData = chosenAbnormality;
            if (String.IsNullOrEmpty(chosenAbnormality)) return ExecutionResult.Failure("Action failed. Missing required parameter 'abnormality_name'.");
            if (AbnormalityExtractionScene.AbnormalityNames.Contains(chosenAbnormality)) return ExecutionResult.Success();
            return ExecutionResult.Failure("Action failed. Invalid parameter 'abnormality_name'");
        }

        protected override Task Execute(string? resultData)
        {
            return NeuroLCConnector.Connector.SendCommand("extract_abnormality|" + resultData);
        }
    }

    public class ReextractAbnormalities : NeuroActionNoValidation
    {
        public override string Name => "reextract_abnormalities";

        protected override string SuccessMessage => "Refreshing...";

        protected override string Description => "Gain a new set of Abnormalities which may be extracted.";
    }
}
