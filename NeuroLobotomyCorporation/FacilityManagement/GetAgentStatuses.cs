using System;
using NeuroLobotomyCorporation.FacilityManagement;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class GetAgentStatuses
    {
        public static string Command()
        {
            string status = "";
            List<Helpers.Entry<SefiraEnum, List<AgentModel>>> sortedAgents = Helpers.SortAgentsByDepartment(AgentManager.instance.GetAgentList().ToArray(), false);
            foreach(Helpers.Entry<SefiraEnum, List<AgentModel>> sortingEntry in sortedAgents)
            {
                status += Helpers.GetDepartmentBySefira(sortingEntry.k) + " Team Agents:\n";
                foreach(AgentModel agent in sortingEntry.v)
                {
                    string name = agent.GetUnitName();
                    int hp = (int)agent.hp;
                    int maxHp = agent.maxHp;
                    int sp = (int)agent.mental;
                    int maxSp = agent.maxMental;
                    Helpers.AgentWorkingState agentCurrentState = Helpers.GetAgentWorkingState(agent);
                    if (agentCurrentState == Helpers.AgentWorkingState.DEAD)
                    {
                        status += String.Format("-{0}: DEAD\n", name);
                        continue;
                    }
                    string agentStateDesc = "";
                    switch (agentCurrentState)
                    {
                        case Helpers.AgentWorkingState.UNKNOWN:
                            agentStateDesc = "In an Unknown State";
                            break;
                        case Helpers.AgentWorkingState.IDLE:
                            agentStateDesc = "Idle";
                            break;
                        case Helpers.AgentWorkingState.WORKING:
                            agentStateDesc = "In Work";
                            break;
                        case Helpers.AgentWorkingState.SUPPRESSING:
                            agentStateDesc = "Suppressing";
                            break;
                        case Helpers.AgentWorkingState.PANICKING:
                            agentStateDesc = "Panicking";
                            break;
                        case Helpers.AgentWorkingState.UNCONTROLLABLE:
                            agentStateDesc = "Uncontrollable";
                            break;
                        case Helpers.AgentWorkingState.HERETIC:
                            agentStateDesc = "Heretic";
                            break;
                    }
                    status += String.Format("-{0}: {1}/{2} HP, {3}/{4} SP, Currently {5}\n", name, hp, maxHp, sp, maxSp, agentStateDesc);
                }
                status += "\n";
            }
            return status;
        }
    }
}
