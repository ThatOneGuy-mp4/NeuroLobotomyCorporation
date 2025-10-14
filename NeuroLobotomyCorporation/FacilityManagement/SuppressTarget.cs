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
            UnitModel target = TryFindPanickedTarget(targetName);
            if (target == null) target = TryFindAbnormalityTarget(targetName, sefiraDepartment);
            if (target == null) target = TryFindOrdealTarget(targetName, sefiraDepartment);
            if (target == null) target = TryFindSefiraCoreTarget(targetName);
            if (target == null) return "failure|There are no valid targets of the specified name.";
            ThreadPool.QueueUserWorkItem(CommandExecute, new SuppressTargetState(agent, target, sefiraDepartment));
            return String.Format("success|{0} has begun suppressing {1}.", agentName, targetName);
        }

        private static UnitModel TryFindPanickedTarget(string targetName)
        {
            AgentModel panickedTarget = null;
            if (Helpers.AgentExists(targetName, out panickedTarget) && panickedTarget.IsPanic()) return panickedTarget;
            return null;
        }

        private static UnitModel TryFindAbnormalityTarget(string targetName, SefiraEnum location)
        {
            List<UnitModel> targetsWithName = new List<UnitModel>();
            foreach(CreatureModel abnormality in SefiraManager.instance.GetEscapedCreatures())
            {
                if (targetName.Equals(abnormality.script.GetName())) targetsWithName.Add(abnormality);
            }
            if (targetsWithName.Count == 0) return null;
            if (targetsWithName.Count == 1) return targetsWithName[0];
            Random rand = new Random();
            if (location == SefiraEnum.DUMMY) return targetsWithName[rand.Next(0, targetsWithName.Count)]; 
            List<UnitModel> targetsInDepartment = new List<UnitModel>();
            foreach (CreatureModel abnormality in targetsWithName)
            {
                SefiraEnum targetLocation = Helpers.GetUnitModelLocationSefira(abnormality);
                if (targetLocation == location) targetsInDepartment.Add(abnormality);
            }
            if (targetsInDepartment.Count > 0) return targetsInDepartment[rand.Next(0, targetsInDepartment.Count)];
            return targetsWithName[rand.Next(0, targetsWithName.Count)];
        }

        private static UnitModel TryFindOrdealTarget(string targetName, SefiraEnum location)
        {
            if (OrdealManager.instance.GetActivatedOrdeals().Count == 0) return null;
            List<UnitModel> targetsWithName = new List<UnitModel>();
            foreach(OrdealCreatureModel ordealCreature in OrdealManager.instance.GetOrdealCreatureList())
            {
                if (ordealCreature.OrdealBase.OrdealNameText(ordealCreature).Equals(targetName)) targetsWithName.Add(ordealCreature);
            }
            if (targetsWithName.Count == 0) return null;
            if (targetsWithName.Count == 1) return targetsWithName[0];
            Random rand = new Random();
            if (location == SefiraEnum.DUMMY) return targetsWithName[rand.Next(0, targetsWithName.Count)];
            List<UnitModel> targetsInDepartment = new List<UnitModel>();
            foreach(OrdealCreatureModel ordealCreature in targetsWithName)
            {
                SefiraEnum targetLocation = Helpers.GetUnitModelLocationSefira(ordealCreature);
                if (targetLocation == location) targetsInDepartment.Add(ordealCreature);
            }
            if (targetsInDepartment.Count > 0) return targetsInDepartment[rand.Next(0, targetsInDepartment.Count)];
            return targetsWithName[rand.Next(0, targetsWithName.Count)];
        }

        private static UnitModel TryFindSefiraCoreTarget(string targetName)
        {
            if (!SefiraBossManager.Instance.IsAnyBossSessionActivated()) return null;
            foreach (SefiraBossCreatureModel bossSefira in SefiraBossManager.Instance.CurrentBossBase.modelList)
            {
                if (bossSefira.script.GetName().Equals(targetName)) return bossSefira;
            }
            return null;
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
