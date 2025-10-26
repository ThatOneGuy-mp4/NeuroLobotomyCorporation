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
    /*
     * This ActionScene covers the main gameplay phase of Lobotomy Corporation, the management of Abnormalities.
     * This scene contains a lot more actions overall than most scenes, and a lot of actions which only get data in particular.
     */

    /*
     * TODO: 
     * -Add a config option to allow execution bullets.
     * -Add an Action to order Rabbit Team Deployment...? (perhaps not?)
     * -Could be interesting to give her the ability to read an Abnormality's lore (not needed though)
     * -Could also give her the ability to unlock an Abnormality's observation info / craft E.G.O, but I think that might be better for just ved to be able to do (so she doesn't waste it)
     * 
     * TODO (CORE SUPPRESSIONS):
     * [The Core Suppressions will extend this scene, with modified context and actions to help direct Neuro while also making it a proper boss even for her.]
     * -MALKUTH: AssignWork will assign the randomized work instead of the version she requests (as per the original game), but since Neuro can't see the work icons to see what was assigned, 
     *      she will instead be told what was actually assigned without telling her it changed (e.g., attempts to assign Instinct, gets a message saying Repression was begun)
     * -YESOD: SendContext will randomly scramble some of the characters before sending the message back, with the intensity increasing as the suppression continues
     * -HOD: No change except for context (GetDayStatus informs her of the progressively weakened agents). 
     * -NETZACH: No change except for context (informs her every Qliphoth Meltdown of the heal).
     * -TIPHERETH: No change except for context (informs her energy generation is not required and to focus on meltdowns).
     * -GEBURA: I don't even know how to translate this one to make Neuro able to do things. She might just have to be moral support for Ved.
     * -CHESED: No change except for context (GetDayStatus informs her of the currently boosted damage types).
     * -BINAH: Same problem as Gebura, but she can still help deal with Qliphoth Overloads at least. The Qliphoth Overload info Action will also inform her of the type.
     * -HOKMA: So I can't exactly speed up Neuro's response to things, so, to truly emulate the boss' time-warping experience, I'd need to like...do...something else...
     *      (for legal reasons, I am not going to artifically increase Neuro's latency on her Actions. I would never.)
     * -KETER (A): No change except for context...? Might add something to track the Claw's containment unit targetting, not sure.
     * -KETER (ABEL): All of Asiyah's gimmicks again, with new context.
     * -KETER (ABRAM): All of Briah's gimmicks again, with new context. 
     * -KETER (ADAM): All of Atziluth's gimmicks again, with new context. But maybe disable Hokma's (LEGALLY NONEXISTENT) latency gimmick because day 49 sucks enough as is.
     * -DA'AT: ;)
     */
    public class FacilityManagementScene : ActionScene
    {
        protected override List<INeuroAction> InitActions
        {
            get
            {
                List<INeuroAction> list = new List<INeuroAction>
                {
                    new DEBUGFuckingKillsYou(),
                    new GetDayStatus(),
                    new GetAgentStatuses(),
                    new GetDetailedAgentInfo(),
                    new GetAbnormalityStatuses(),
                    new GetDetailedAbnormalityInfo(),
                    new GetOverloadedUnits(),
                    new AssignWork(),
                    new UseTool(),
                    new GetSuppressibleTargets(),
                    new SuppressTarget(),
                    new CancelAction()
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
                    new DEBUGFuckingKillsYou(),
                    new GetDayStatus(),
                    new GetAgentStatuses(),
                    new GetDetailedAgentInfo(),
                    new GetAbnormalityStatuses(),
                    new GetDetailedAbnormalityInfo(),
                    new GetOverloadedUnits(),
                    new AssignWork(),
                    new UseTool(),
                    new GetSuppressibleTargets(),
                    new SuppressTarget(),
                    new CancelAction(),
                    new ShootManagerialBullet()
                };
                return list;
            }
        }

        //TODO: add the actual day and energy requirements in here (will require a game request)
        protected override string GetActionSceneStartContext()
        {
            return NeuroLCConnector.Connector.SendCommand("get_day_start_context").Result;
        }

        public override void InitializeActionScene()
        {
            base.InitializeActionScene();
            string isBulletUnlocked = NeuroLCConnector.Connector.SendCommand("is_bullet_unlocked").Result;
            if (isBulletUnlocked.Equals("true")) RegisterAction("shoot_managerial_bullet");
        }
    }

    public class GetDayStatus : NeuroActionNoValidation
    {
        public override string Name => "get_day_status";

        protected override string SuccessMessage => "Getting the day's status to send as context...";

        protected override string Description => "Get the day's overall status. ";

    }

    public class DEBUGFuckingKillsYou : NeuroActionExternalExecute
    {
        public override string Name => "DEBUG_fucking_kills_you";

        protected override string Description => "Fucking kill that guy. Kill 'em dead.";

        protected override JsonSchema? Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = new List<string> { "target_name" },
            Properties = new Dictionary<string, JsonSchema>
            {
                ["target_name"] = QJS.Type(JsonSchemaType.String)
            }
        };

        protected override ExecutionResult Validate(ActionData actionData)
        {
            return ValidateGameSide(actionData.Data?["target_name"]?.Value<string>());
        }
    }
    public class GetAgentStatuses : NeuroActionNoValidation
    {
        public override string Name => "get_agent_statuses";

        protected override string SuccessMessage => "Getting all agents' statuses to send as context...";

        protected override string Description => "Get a brief overview of the status of all agents.";
    }

    public class GetDetailedAgentInfo : NeuroActionExternalExecute
    {
        public override string Name => "get_detailed_agent_info";

        protected override string Description => "Get the detailed information for a specified agent.";

        protected override JsonSchema? Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = new List<string> { "agent_name" },
            Properties = new Dictionary<string, JsonSchema>
            {
                ["agent_name"] = QJS.Type(JsonSchemaType.String)
            }
        };

        protected override ExecutionResult Validate(ActionData actionData)
        {
            string? agentName = actionData.Data?["agent_name"]?.Value<string>();
            if (String.IsNullOrEmpty(agentName)) return ExecutionResult.Failure("Action failed. Missing required parameter 'agent_name'.");
            return ValidateGameSide(agentName);
        }
    }

    public class GetAbnormalityStatuses : NeuroActionNoValidation
    {
        public override string Name => "get_abnormality_statuses";

        protected override string SuccessMessage => "Getting all Abnormalities' statuses to send as context...";

        protected override string Description => "Get a brief overview of the status of Abnormalities.";
    }

    public class GetDetailedAbnormalityInfo : NeuroActionExternalExecute
    {
        public override string Name => "get_detailed_abnormality_info";

        protected override string Description => "Get detailed information for a specified Abnormality.";

        protected override JsonSchema? Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = new List<string> { "abnormality_name" },
            Properties = new Dictionary<string, JsonSchema>
            {
                ["abnormality_name"] = QJS.Type(JsonSchemaType.String),
                ["include_basic_info"] = QJS.Enum(new List<string> { "true", "false"}),
                ["include_managerial_guidelines"] = QJS.Enum(new List<string> { "true", "false" }),
                ["include_work_success_rates"] = QJS.Enum(new List<string> { "true", "false" }),
                ["include_escape_information"] = QJS.Enum(new List<string> { "true", "false" })
            }
        };

        protected override ExecutionResult Validate(ActionData actionData)
        {
            string? abnormalityName = actionData.Data?["abnormality_name"]?.Value<string>();
            if (String.IsNullOrEmpty(abnormalityName)) return ExecutionResult.Failure("Action failed. Missing required parameter 'abnormality_name'.");
            string? includeBasicInfo = actionData.Data?["include_basic_info"]?.Value<string>();
            if (String.IsNullOrEmpty(includeBasicInfo)) includeBasicInfo = "true";
            else if (!includeBasicInfo.Equals("false") && !includeBasicInfo.Equals("true")) return ExecutionResult.Failure("Action failed. Parameter 'include_basic_info' must be 'true', 'false', or left empty.");
            string? includeManagerialGuidelines = actionData.Data?["include_managerial_guidelines"]?.Value<string>();
            if (String.IsNullOrEmpty(includeManagerialGuidelines)) includeManagerialGuidelines = "true";
            else if (!includeManagerialGuidelines.Equals("false") && !includeManagerialGuidelines.Equals("true")) return ExecutionResult.Failure("Action failed. Parameter 'include_managerial_guidelines' must be 'true', 'false', or left empty.");
            string? includeWorkSuccessRates = actionData.Data?["include_work_success_rates"]?.Value<string>();
            if (String.IsNullOrEmpty(includeWorkSuccessRates)) includeWorkSuccessRates = "true";
            else if (!includeWorkSuccessRates.Equals("false") && !includeWorkSuccessRates.Equals("true")) return ExecutionResult.Failure("Action failed. Parameter 'include_work_success_rates' must be 'true', 'false', or left empty.");
            string? includeEscapeInformation = actionData.Data?["include_escape_information"]?.Value<string>();
            if (String.IsNullOrEmpty(includeEscapeInformation)) includeEscapeInformation = "true";
            else if (!includeEscapeInformation.Equals("false") && !includeEscapeInformation.Equals("true")) return ExecutionResult.Failure("Action failed. Parameter 'include_escape_information' must be 'true', 'false', or left empty.");
            return ValidateGameSide(abnormalityName, includeBasicInfo, includeManagerialGuidelines, includeWorkSuccessRates, includeEscapeInformation);
        }
    }

    public class GetOverloadedUnits : NeuroActionNoValidation
    {
        public override string Name => "get_overloaded_units";

        protected override string SuccessMessage => "Getting all overloaded Containment Units to send as context...";

        protected override string Description => "Get the names of all Abnormalities with overloaded Containment Units.";
    }

    public class AssignWork : NeuroActionExternalExecute
    {
        public override string Name => "assign_work";

        protected override string Description => "Assign an agent to do work on an Abnormality.";

        protected override JsonSchema? Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = new List<string>() { "agent_name", "abnormality_name", "work_type" },
            Properties = new Dictionary<string, JsonSchema>
            {
                ["agent_name"] = QJS.Type(JsonSchemaType.String),
                ["abnormality_name"] = QJS.Type(JsonSchemaType.String),
                ["work_type"] = QJS.Enum(new List<string>() { "Instinct", "Insight", "Attachment", "Repression" })
            }
        };

        protected override ExecutionResult Validate(ActionData actionData)
        {
            string? agentName = actionData.Data?["agent_name"]?.Value<string>();
            if (String.IsNullOrEmpty(agentName)) return ExecutionResult.Failure("Action failed. Missing required parameter 'agent_name'.");
            string? abnormalityName = actionData.Data?["abnormality_name"]?.Value<string>();
            if (String.IsNullOrEmpty(abnormalityName)) return ExecutionResult.Failure("Action failed. Missing required parameter 'abnormality_name'.");
            string? workType = actionData.Data?["work_type"]?.Value<string>();
            if (String.IsNullOrEmpty(workType)) return ExecutionResult.Failure("Action failed. Missing required parameter 'work_type'.");
            return ValidateGameSide(agentName, abnormalityName, workType);
        }
    }

    public class UseTool : NeuroActionExternalExecute
    {
        public override string Name => "use_tool";

        protected override string Description => "Assign an agent to use a Tool Abnormality.";

        protected override JsonSchema? Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = new List<string>() { "agent_name", "abnormality_name"},
            Properties = new Dictionary<string, JsonSchema>
            {
                ["agent_name"] = QJS.Type(JsonSchemaType.String),
                ["abnormality_name"] = QJS.Type(JsonSchemaType.String)
            }
        };

        protected override ExecutionResult Validate(ActionData actionData)
        {
            string? agentName = actionData.Data?["agent_name"]?.Value<string>();
            if (String.IsNullOrEmpty(agentName)) return ExecutionResult.Failure("Action failed. Missing required parameter 'agent_name'.");
            string? abnormalityName = actionData.Data?["abnormality_name"]?.Value<string>();
            if (String.IsNullOrEmpty(abnormalityName)) return ExecutionResult.Failure("Action failed. Missing required parameter 'abnormality_name'.");
            return ValidateGameSide(agentName, abnormalityName);
        }
    }

    public class GetSuppressibleTargets : NeuroActionNoValidation
    {
        public override string Name => "get_suppressible_targets";

        protected override string SuccessMessage => "Getting all suppressible targets to send as context...";

        protected override string Description => "Get all entities which can currently be suppressed.";
    }

    public class SuppressTarget : NeuroActionExternalExecute
    {
        public override string Name => "suppress_target";

        protected override string Description => "Order an agent to suppress a specified entity.";

        protected override JsonSchema? Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = new List<string>() { "agent_name", "target_name"},
            Properties = new Dictionary<string, JsonSchema>
            {
                ["agent_name"] = QJS.Type(JsonSchemaType.String),
                ["target_name"] = QJS.Type(JsonSchemaType.String),
                ["target_department"] = QJS.Type(JsonSchemaType.String) //could change this to be an enum but that would also require making a game request at some point to see which departments have been opened
            }
        };

        protected override ExecutionResult Validate(ActionData actionData)
        {
            string? agentName = actionData.Data?["agent_name"]?.Value<string>();
            if (String.IsNullOrEmpty(agentName)) return ExecutionResult.Failure("Action failed. Missing required parameter 'agent_name'.");
            string? targetName = actionData.Data?["target_name"]?.Value<string>();
            if (String.IsNullOrEmpty(targetName)) return ExecutionResult.Failure("Action failed. Missing required parameter 'target_name'.");
            string? targetDepartment = actionData.Data?["target_department"]?.Value<string>();
            if (!String.IsNullOrEmpty(targetDepartment))
            {
                if (targetDepartment.Contains("Department"))
                {
                    targetDepartment = targetDepartment.Remove(targetDepartment.IndexOf("Department"));
                }
                targetDepartment = targetDepartment.Trim();
            }
            else targetDepartment = "DUMMY";
            return ValidateGameSide(agentName, targetName, targetDepartment);
        }
    }

    public class CancelAction : NeuroActionExternalExecute
    {
        public override string Name => "cancel_action";

        protected override string Description => "Cancel an Agent's current action.";

        protected override JsonSchema? Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = new List<string>() { "agent_name" },
            Properties = new Dictionary<string, JsonSchema>
            {
                ["agent_name"] = QJS.Type(JsonSchemaType.String)
            }
        };

        protected override ExecutionResult Validate(ActionData actionData)
        {
            string? agentName = actionData.Data?["agent_name"]?.Value<string>();
            if (String.IsNullOrEmpty(agentName)) return ExecutionResult.Failure("Action failed. Missing required parameter 'agent_name'.");
            return ValidateGameSide(agentName);
        }
    }

    public class ShootManagerialBullet : NeuroActionExternalExecute
    {
        public override string Name => "shoot_managerial_bullet";

        protected override string Description => "Shoot a specified type of managerial bullet at the specified target.";

        protected override JsonSchema? Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = new List<string>() { "bullet_type", "target_name" },
            Properties = new Dictionary<string, JsonSchema>
            {
                ["bullet_type"] = QJS.Type(JsonSchemaType.String),
                ["target_name"] = QJS.Type(JsonSchemaType.String),
                ["target_department"] = QJS.Type(JsonSchemaType.String) 
            }
        };

        protected override ExecutionResult Validate(ActionData actionData)
        {
            string? bulletType = actionData.Data?["bullet_type"]?.Value<string>();
            if (String.IsNullOrEmpty(bulletType)) return ExecutionResult.Failure("Action failed. Missing required parameter 'bullet_type'.");
            if (bulletType.Contains("Bullet"))
            {
                bulletType = bulletType.Remove(bulletType.IndexOf("Bullet"));
                bulletType = bulletType.Trim();
            }
            string? targetName = actionData.Data?["target_name"]?.Value<string>();
            if (String.IsNullOrEmpty(targetName)) return ExecutionResult.Failure("Action failed. Missing required parameter 'target_name'.");
            string? targetDepartment = actionData.Data?["target_department"]?.Value<string>();
            if (String.IsNullOrEmpty(targetDepartment)) targetDepartment = "DUMMY";
            else
            {
                if (targetDepartment.Contains("Department"))
                {
                    targetDepartment = targetDepartment.Remove(targetDepartment.IndexOf("Department"));
                    targetDepartment = targetDepartment.Trim();
                }
            }
            return ValidateGameSide(bulletType, targetName, targetDepartment);
        }
    }
}
