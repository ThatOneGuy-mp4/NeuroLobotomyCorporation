using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class GetSuppressableTargets
    {
        public static string Command()
        {
            string result = "";
            List<AgentModel> panickingAgents = new List<AgentModel>();
            foreach (AgentModel agent in AgentManager.instance.GetAgentList())
            {
                if (agent.IsPanic()) panickingAgents.Add(agent);
            }
            if (panickingAgents.Count > 0)
            {
                result += "Panicking Agents (suppress with WHITE or BLACK damage to recover sanity)\n-------------------------\n";
                foreach (AgentModel agent in panickingAgents)
                {
                    string name = agent.GetUnitName();
                    string location = Helpers.GetUnitModelLocationText(agent);
                    string panicAction = "";
                    if (agent.CurrentPanicAction is PanicViolence)
                    {
                        panicAction = "Attempting to Commit Murder";
                    }
                    else if (agent.CurrentPanicAction is PanicSuicideExecutor)
                    {
                        panicAction = "Preparing to Commit Suicide";
                    }
                    else if (agent.CurrentPanicAction is PanicRoaming)
                    {
                        panicAction = "Running Around Aimlessly";
                    }
                    else if (agent.CurrentPanicAction is PanicOpenIsolate)
                    {
                        panicAction = "Attempting to Free Abnormalities";
                    }
                    else
                    {
                        panicAction = "Panicking"; //i don't know if AiB's panic will register differently so this is just a backup
                    }

                    result += String.Format("{0}, {1}, {2}/{3} HP, {4}/{5} SP, Currently {6}\n", name, location, (int)agent.hp, agent.maxHp, (int)agent.mental, agent.maxMental, panicAction);
                }
                result += "\n";
            }
            //TODO: add special enemies which always get placed at the top of the list 
            //  (Red Mist, An Arbiter, Apoc Bird + Eggs, Whitenight + Angels, Claw (during Keter 1))
            //TODO: make sure this works correctly with identically named child entities
            List<CreatureModel> escapedAbnormalities = SefiraManager.instance.GetEscapedCreatures();
            if (escapedAbnormalities.Count > 0)
            {
                result += "Breaching Abnormalities\n-------------------------\n";
                foreach (CreatureModel abnormality in escapedAbnormalities)
                {
                    string name = abnormality.script.GetName();
                    string riskLevel = abnormality.metaInfo.riskLevelForce;
                    string location = Helpers.GetUnitModelLocationText(abnormality);
                    string healthPercentRemaining = ((int)((float)(abnormality.hp / abnormality.metaInfo.maxHp) * 100)).ToString();
                    List<string> defenseInfo = Helpers.GetResistanceTypeValues(abnormality);
                    result += String.Format("{0}, {1} Level Threat, {2}, {3}% HP Remaining\n" +
                        "{4} RED Res, {5} WHITE Res, {6} BLACK Res, {7} PALE Res.", name, riskLevel, location, healthPercentRemaining,
                        defenseInfo[(int)Helpers.ResistanceTypes.RED], defenseInfo[(int)Helpers.ResistanceTypes.WHITE], defenseInfo[(int)Helpers.ResistanceTypes.BLACK], defenseInfo[(int)Helpers.ResistanceTypes.PALE]);
                }
            }

            List<OrdealBase> activatedOrdeals = OrdealManager.instance.GetActivatedOrdeals();
            if (activatedOrdeals.Count > 0)
            {
                foreach (OrdealBase ordeal in activatedOrdeals)
                {
                    List<OrdealCreatureModel> ordealCreaturesInOrdeal = new List<OrdealCreatureModel>();
                    foreach (OrdealCreatureModel ordealCreature in OrdealManager.instance.GetOrdealCreatureList())
                    {
                        if (ordealCreature.OrdealBase.OrdealTypeText.Equals(ordeal.OrdealTypeText)) ordealCreaturesInOrdeal.Add(ordealCreature);
                    }
                    string ordealTargets = String.Format("{0}\n-------------------------\n", ordeal.OrdealTypeText);
                    foreach (OrdealCreatureModel ordealCreature in ordealCreaturesInOrdeal)
                    {
                        string name = ordealCreature.OrdealBase.OrdealNameText(ordealCreature);
                        string riskLevel = ordealCreature.OrdealBase.GetRiskLevel(ordealCreature).ToString();
                        string location = Helpers.GetUnitModelLocationText(ordealCreature);
                        string healthPercentRemaining = ((int)((float)(ordealCreature.hp / ordealCreature.metaInfo.maxHp) * 100)).ToString();
                        ordealTargets += String.Format("{0}, {1} Level Threat, {2}, {3}% HP Remaining\n", name, riskLevel, location, healthPercentRemaining);
                    }
                    result += ordealTargets;
                }
            }
            if (String.IsNullOrEmpty(result)) return "There are no suppressable targets at the moment.";
            return result;
        }
    }
}
