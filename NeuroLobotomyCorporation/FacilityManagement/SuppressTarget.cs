using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class SuppressTarget
    {
        public enum Parameters
        {
            AGENT_NAME = 1,
            TARGET_NAME = 2,
            LOCATION = 3
        }
        public static string Command(string agentName, string targetName, string location)
        {
            AgentModel agent = null;
            if (!Helpers.AgentExists(agentName, out agent)) return "failure|Specified agent does not exist.";
            switch (Helpers.GetAgentWorkingState(agent))
            {
                case Helpers.AgentWorkingState.WORKING:
                    return "failure|Suppression could not be ordered because the specified agent is busy working.";
                case Helpers.AgentWorkingState.DEAD:
                    return "failure|Suppression could not be ordered because the specified agent is dead.";
                case Helpers.AgentWorkingState.PANICKING:
                    return "failure|Suppression could not be ordered because the specified agent is panicking.";
                case Helpers.AgentWorkingState.UNCONTROLLABLE:
                    return "failure|Suppression could not be ordered because the specified agent is uncontrollable.";
            }
            SefiraEnum sefiraDepartment = Helpers.GetSefiraByDepartment(location);
            if (sefiraDepartment == SefiraEnum.DUMMY && !location.Equals("DUMMY")) return "failure|Target's department was specified but was not valid.";
            else if (sefiraDepartment == SefiraEnum.DUMMY)
            {
                PassageObjectModel currentPassage = agent.GetMovableNode().GetPassage();
                if (currentPassage != null)
                {
                    sefiraDepartment = currentPassage.GetSefiraEnum();
                }
            }
            UnitModel target = Helpers.TryFindAnySuppressableTarget(targetName, sefiraDepartment);
            if (target == null) return "failure|There are no valid targets of the specified name.";
            ThreadPool.QueueUserWorkItem(CommandExecute, new SuppressTargetState(agent, target, sefiraDepartment));
            return String.Format("success|{0} has begun suppressing {1}.", agentName, targetName);
        }

        

        private class SuppressTargetState
        {
            public AgentModel Agent { get; private set; }
            public UnitModel Target { get; private set; }
            public SefiraEnum Location { get; private set; }

            public SuppressTargetState(AgentModel agent, UnitModel target, SefiraEnum location)
            {
                Agent = agent;
                Target = target;
                Location = location;
            }
        }

        private static void CommandExecute(object state)
        {
            SuppressTargetState suppressTargetState = (SuppressTargetState)state;
            AgentModel agent = suppressTargetState.Agent;
            UnitModel target = suppressTargetState.Target;
            if (agent.IsSuppressing()) agent.ForcelyCancelSuppress();
            agent.Suppress(target);
        }
    }
}
