using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class UseTool
    {
        public enum Parameters
        {
            AGENT_NAME = 1,
            ABNORMALITY_NAME = 2
        }
        public static string Command(string agentName, string abnormalityName)
        {
            AgentModel agent = null;
            if (!Helpers.AgentExists(agentName, out agent)) return "failure|Specified agent does not exist.";
            CreatureModel abnormality = null;
            if (!Helpers.AbnormalityExists(abnormalityName, out abnormality)) return "failure|Specified Abnormality does not exist.";
            if (abnormality.metaInfo.creatureWorkType != CreatureWorkType.KIT) return "failure|Specified Abnormality is not a Tool. Use the 'AssignWork' command instead.";
            switch (Helpers.GetAgentWorkingState(agent))
            {
                case Helpers.AgentWorkingState.WORKING:
                    return "failure|Work could not be assigned because the specified agent is already working.";
                case Helpers.AgentWorkingState.DEAD:
                    return "failure|Work could not be assigned because the specified agent is dead.";
                case Helpers.AgentWorkingState.PANICKING:
                    return "failure|Work could not be assigned because the specified agent is panicking.";
                case Helpers.AgentWorkingState.UNCONTROLLABLE:
                    return "failure|Work could not be assigned because the specified agent is uncontrollable.";
            }
            switch (Helpers.GetAbnormalityWorkingState(abnormality))
            {
                case Helpers.AbnormalityWorkingState.BREACHING:
                    return "failure|Work could not be assigned because the specified Abnormality is currently breaching."; //thanks yang
                case Helpers.AbnormalityWorkingState.WORKING:
                    return "failure|Work could not be assigned because the specified Abnormality is already being used.";
                case Helpers.AbnormalityWorkingState.COOLDOWN:
                    return "failure|Work could not be assigned because the specified Abnormality is in cooldown. Try again in a bit."; //this shouldn't be possible with a tool I think but I'll leave it there anyways
            }
            string cancelHint = "";
            if(abnormality.metaInfo.creatureKitType == CreatureKitType.EQUIP)
            {
                if (agent.Equipment.kitCreature != null) return "failure|Work could not be assigned because the specified Abnormality is an equippable Tool, but the specified Agent already has a Tool equipped.";
                cancelHint = String.Format(" (use the CancelAction command while {0} is idle to return the Tool)", agentName);
            }else if(abnormality.metaInfo.creatureKitType == CreatureKitType.CHANNEL)
            {
                cancelHint = String.Format(" (use the CancelAction command on {0} to stop channeling the Tool)", agentName);
            }
            ThreadPool.QueueUserWorkItem(CommandExecute, new UseToolState(agentName, abnormalityName));
            return String.Format("success|{0} begins their usage of {1}{2}.", agentName, abnormalityName, cancelHint);
        }

        private class UseToolState
        {
            public string AgentName { get; private set;}
            public string AbnormalityName { get; private set; }

            public UseToolState(string agentName, string abnormalityName)
            {
                AgentName = agentName;
                AbnormalityName = abnormalityName;
            }
        }

        private static void CommandExecute(object state)
        {
            UseToolState toolState = (UseToolState)state;
            AgentModel agent = null;
            CreatureModel abnormality = null;
            if (!Helpers.AgentExists(toolState.AgentName, out agent)) return;
            if (!Helpers.AbnormalityExists(toolState.AbnormalityName, out abnormality)) return;
            agent.ManageKitCreature(abnormality);
            agent.counterAttackEnabled = false;
            abnormality.Unit.room.OnWorkAllocated(agent);
        }
    }
}
