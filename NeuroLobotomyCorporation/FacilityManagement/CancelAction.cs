using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class CancelAction
    {
        public enum Parameters
        {
            AGENT_NAME = 1
        }

        public enum CancelType
        {
            SUPPRESSION,
            EQUIPPED_TOOL,
            CHANNELED_TOOL,
            EN_ROUTE_ABNORMALITY
        }
        public static string Command(string agentName)
        {
            AgentModel agent = null;
            if (!Helpers.AgentExists(agentName, out agent)) return "failure|The specified Agent does not exist.";
            Helpers.AgentWorkingState agentState = Helpers.GetAgentWorkingState(agent);
            switch (agentState)
            {
                case Helpers.AgentWorkingState.DEAD:
                    return "failure|The specified Agent's actions were already cancelled long ago; they are dead.";
                case Helpers.AgentWorkingState.PANICKING:
                    return "failure|The specified Agent's action cannot be cancelled as they are currently panicking.";
                case Helpers.AgentWorkingState.UNCONTROLLABLE:
                    return "failure|The specified Agent's action cannot be cancelled as they are currently uncontrollable.";
                case Helpers.AgentWorkingState.SUPPRESSING:
                    ThreadPool.QueueUserWorkItem(CommandExecute, new CancelActionState(agent, CancelType.SUPPRESSION));
                    return String.Format("success|{0} has stopped suppressing their target.", agentName);
                case Helpers.AgentWorkingState.IDLE:
                    if (agent.Equipment.kitCreature == null) return "failure|The specified Agent is idle and has no action to cancel.";
                    ThreadPool.QueueUserWorkItem(CommandExecute, new CancelActionState(agent, CancelType.EQUIPPED_TOOL));
                    return String.Format("success|{0} has been ordered to return their equipped Tool.", agentName);
                case Helpers.AgentWorkingState.UNKNOWN:
                    return "failure|The Agent's status is unknown. Complain to the mod developer about it.";
            }
            if (agent.canCancelCurrentWork)
            {
                if (!SefiraBossManager.Instance.IsWorkCancelable) return "failure|Cancellation failed due to an error with the work cancellation system." +
                        "\n\"This is what you call a truly uncontrollable situation, manager.\"";
                ThreadPool.QueueUserWorkItem(CommandExecute, new CancelActionState(agent, CancelType.EN_ROUTE_ABNORMALITY));
                return String.Format("success|{0} is no longer en route to their target.", agentName);
            }
            if(agent.currentSkill.skillTypeInfo.id == 5L) //Tool Abnormality - at this point channelled Tools in use should be the only Tools that trigger this
            {
                if (!agent.target.script.PermitCancelCurrentWork()) return "failure|The specified Agent cannot escape channelling their Tool.";
                    ThreadPool.QueueUserWorkItem(CommandExecute, new CancelActionState(agent, CancelType.CHANNELED_TOOL));
                return String.Format("success|{0} has stopped channelling their Tool.", agentName);
            }
            return "failure|The specified Agent is already inside their target's containment unit.";
        }

        private class CancelActionState
        {
            public AgentModel Agent { get; private set; }

            public CancelType Type { get; private set; }

            public CancelActionState(AgentModel agent, CancelType type)
            {
                Agent = agent;
                Type = type;
            }
        }

        private static void CommandExecute(object state)
        {
            CancelActionState cancelActionState = (CancelActionState)state;
            AgentModel agent = cancelActionState.Agent;
            CancelType type = cancelActionState.Type;
            switch (type)
            {
                case CancelType.SUPPRESSION:
                    agent.ForcelyCancelSuppress();
                    break;
                case CancelType.EQUIPPED_TOOL:
                    agent.ReturnKitCreature();
                    break;
                case CancelType.CHANNELED_TOOL:
                    agentToCancel = agent;
                    break;
                case CancelType.EN_ROUTE_ABNORMALITY:
                    agent.ForcelyCancelWork();
                    break;
            }
        }

        /*
         * Trying to cancel an agent's action while they're channelling a Tool crashes the game. 
         * Instead, if the channelled action is to be cancelled, store the agent who is channelling it,
         * then this harmony patch on IsolateRoom's Update cancels it instead.
         */
        private static AgentModel agentToCancel = null;
        public static void CancelChannelledTool(IsolateRoom __instance)
        {
            if(agentToCancel != null && __instance.TargetUnit.model.currentSkill != null && __instance.TargetUnit.model.currentSkill.agent.Equals(agentToCancel))
            {
                __instance.TargetUnit.model.currentSkill.CancelWork();
                agentToCancel = null;
            }
        }
    }
}
