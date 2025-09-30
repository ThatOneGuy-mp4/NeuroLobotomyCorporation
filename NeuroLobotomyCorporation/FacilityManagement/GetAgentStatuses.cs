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
        //TODO: these are not sorted by department, and that annoys me. Fix it.
        public static string Command()
        {
            string allAgentStatuses = "";
            foreach (AgentModel agent in AgentManager.instance.GetAgentList())
            {
                string name = agent.GetUnitName();
                string department = Helpers.GetDepartmentBySefira(agent.GetCurrentSefira().sefiraEnum);
                int hp = (int)agent.hp;
                int maxHp = agent.maxHp;
                int sp = (int)agent.mental;
                int maxSp = agent.maxMental;
                Helpers.AgentWorkingState agentCurrentState = Helpers.GetAgentWorkingState(agent);
                allAgentStatuses += String.Format("{0}, {1} Team Agent: {2}/{3} HP, {4}/{5} SP, {6}\n", name, department, hp, maxHp, sp, maxSp, agentCurrentState.ToString());
            }
            return allAgentStatuses;
        }
    }
}
