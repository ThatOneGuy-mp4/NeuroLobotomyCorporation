using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public static class Helpers
    {
        public static string GetDepartmentBySefira(SefiraEnum sefira)
        {
            switch (sefira)
            {
                case SefiraEnum.MALKUT:
                    return "Control";
                case SefiraEnum.YESOD:
                    return "Information";
                case SefiraEnum.HOD:
                    return "Training";
                case SefiraEnum.NETZACH:
                    return "Safety";
                case SefiraEnum.TIPERERTH1:
                    return "Upper Central Command";
                case SefiraEnum.TIPERERTH2:
                    return "Lower Central Command";
                case SefiraEnum.GEBURAH:
                    return "Disciplinary";
                case SefiraEnum.CHESED:
                    return "Welfare";
                case SefiraEnum.BINAH:
                    return "Extraction";
                case SefiraEnum.CHOKHMAH:
                    return "Record";
                case SefiraEnum.KETHER:
                    return "Architecture";
                case SefiraEnum.DAAT:
                    return "Carmen's Chamber";
                case SefiraEnum.DUMMY:
                    return "Dummy";
            }
            return "Department could not be gotten from the Sephira. Complain to the mod developer about it.";
        }

        public enum AgentWorkingState
        {
            UNKNOWN,
            IDLE,
            WORKING,
            SUPPRESSING,
            DEAD,
            PANICKING,
            UNCONTROLLABLE
        }

        public static AgentWorkingState GetAgentWorkingState(AgentModel agent)
        {
            if (agent.IsDead())
            {
                return AgentWorkingState.DEAD;
            }
            else if (agent.IsPanic())
            {
                return AgentWorkingState.PANICKING;
            }
            else if (agent.CannotControll())
            {
                return AgentWorkingState.UNCONTROLLABLE;
            }
            else if (agent.IsSuppressing())
            {
                return AgentWorkingState.SUPPRESSING;
            }
            else if (agent.GetState() == AgentAIState.MANAGE || agent.GetState() == AgentAIState.OBSERVE) //idk what tf the difference between these are. keep an eye for whatever it is.
            {
                return AgentWorkingState.WORKING;
            }
            else if (agent.IsIdle())
            {
                return AgentWorkingState.IDLE;
            }
            return AgentWorkingState.UNKNOWN;
        }

        public static AgentModel GetAgentByName(string name)
        {
            foreach (AgentModel agent in AgentManager.instance.GetAgentList())
            {
                if (agent.GetUnitName().Equals(name)) return agent;
            }
            return null;
        }

        public static bool AgentExists(string agentName, out AgentModel agent)
        {
            agent = GetAgentByName(agentName);
            return (agent != null);
        }

        public static CreatureModel GetAbnormalityByName(string name)
        {
            foreach (CreatureModel abnormality in CreatureManager.instance.GetCreatureList())
            {
                if (abnormality.GetUnitName().Equals(name)) return abnormality;
            }
            return null;
        }

        public static bool AbnormalityExists(string abnormalityName, out CreatureModel abnormality)
        {
            abnormality = GetAbnormalityByName(abnormalityName);
            return (abnormality != null);
        }

        public enum StatType
        {
            FORTITUDE = 0,
            PRUDENCE = 1,
            TEMPERANCE = 2,
            JUSTICE = 3
        }

        public enum ResistanceTypes
        {
            RED = 0,
            WHITE = 1,
            BLACK = 2,
            PALE = 3
        }

        public static string GetDamageColorByRwbpType(RwbpType type)
        {
            switch (type)
            {
                case RwbpType.R:
                    return "Red";
                case RwbpType.W:
                    return "White";
                case RwbpType.B:
                    return "Black";
                case RwbpType.P:
                    return "Pale";
            }
            return "Damage type could not be found. Complain to the mod developer about it.";
        }

        public enum AbnormalityWorkingState
        {
            UNKNOWN,
            IDLE,
            WORKING,
            BREACHING,
            COOLDOWN
        }

        public static AbnormalityWorkingState GetAbnormalityWorkingState(CreatureModel abnormality)
        {
            if (abnormality.IsEscaped())
            {
                return AbnormalityWorkingState.BREACHING;
            }
            if (abnormality.IsWorkingState())
            {
                return AbnormalityWorkingState.WORKING;
            }
            if (abnormality.state.Equals(CreatureState.WAIT_COOLDOWN) || abnormality.GetFeelingState() != CreatureFeelingState.NONE)
            {
                return AbnormalityWorkingState.COOLDOWN;
            }
            return AbnormalityWorkingState.IDLE;
        }

        public enum ObserveInfoIndexes
        {
            BasicInfo = 0,
            EscapeInfo = 1,
            InstinctSuccess = 2,
            InsightSuccess = 3,
            AttachmentSuccess = 4,
            RepressionSuccess = 5
        }
    }
}
