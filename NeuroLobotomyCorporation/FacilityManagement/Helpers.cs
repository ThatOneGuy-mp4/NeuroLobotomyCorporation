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

        public static SefiraEnum GetSefiraByDepartment(string department)
        {
            switch (department)
            {
                case "Control":
                    return SefiraEnum.MALKUT;
                case "Information":
                    return SefiraEnum.YESOD;
                case "Training":
                    return SefiraEnum.HOD;
                case "Safety":
                    return SefiraEnum.NETZACH;
                case "Upper Central Command":
                    return SefiraEnum.TIPERERTH1;
                case "Lower Central Command":
                    return SefiraEnum.TIPERERTH2;
                case "Disciplinary":
                    return SefiraEnum.GEBURAH;
                case "Welfare":
                    return SefiraEnum.CHESED;
                case "Extraction":
                    return SefiraEnum.BINAH;
                case "Record":
                    return SefiraEnum.CHOKHMAH;
                case "Architecture":
                    return SefiraEnum.KETHER;
                case "Carmen's Chamber":
                    return SefiraEnum.DAAT;
            }
            return SefiraEnum.DUMMY;
        }

        public static List<Entry<SefiraEnum, List<AgentModel>>> SortAgentsByDepartment(AgentModel[] agents, bool sortByCurrentLocation)
        {
            List<Entry<SefiraEnum, List<AgentModel>>> sefiraSortingList = new List<Entry<SefiraEnum, List<AgentModel>>>();
            foreach (Sefira sefira in SefiraManager.instance.GetActivatedSefiras())
            {
                sefiraSortingList.Add(new Entry<SefiraEnum, List<AgentModel>>(sefira.sefiraEnum, new List<AgentModel>()));
            }
            if (!sortByCurrentLocation)
            {
                foreach (AgentModel agent in agents)
                {
                    sefiraSortingList.Find((Entry<SefiraEnum, List<AgentModel>> entry) => entry.k == agent.currentSefiraEnum).v.Add(agent);
                }
            }
            else
            {
                Entry<SefiraEnum, List<AgentModel>> unknownLocationAgents = new Entry<SefiraEnum, List<AgentModel>>(SefiraEnum.DUMMY, new List<AgentModel>());
                foreach (AgentModel agent in agents)
                {
                    SefiraEnum location = GetUnitModelLocationSefira(agent);
                    if (location == SefiraEnum.DUMMY) unknownLocationAgents.v.Add(agent);
                    else sefiraSortingList.Find((Entry<SefiraEnum, List<AgentModel>> entry) => entry.k == location).v.Add(agent);
                }
                if (unknownLocationAgents.v.Count > 0) sefiraSortingList.Add(unknownLocationAgents);
            }
            return sefiraSortingList;
        }

        public static List<Entry<SefiraEnum, List<CreatureModel>>> SortAbnormalitiesByDepartment(CreatureModel[] abnormalities, bool currentLocation)
        {
            List<Entry<SefiraEnum, List<CreatureModel>>> sefiraSortingList = new List<Entry<SefiraEnum, List<CreatureModel>>>();
            foreach (Sefira sefira in SefiraManager.instance.GetActivatedSefiras())
            {
                sefiraSortingList.Add(new Entry<SefiraEnum, List<CreatureModel>>(sefira.sefiraEnum, new List<CreatureModel>()));
            }
            if (!currentLocation)
            {
                foreach (CreatureModel abnormality in abnormalities)
                {
                    sefiraSortingList.Find((Entry<SefiraEnum, List<CreatureModel>> entry) => entry.k == abnormality.sefira.sefiraEnum).v.Add(abnormality);
                }
            }
            else
            {
                Entry<SefiraEnum, List<CreatureModel>> unknownLocationAbnormalities = new Entry<SefiraEnum, List<CreatureModel>>(SefiraEnum.DUMMY, new List<CreatureModel>());
                foreach (CreatureModel abnormality in abnormalities)
                {
                    SefiraEnum location = GetUnitModelLocationSefira(abnormality);
                    if (location == SefiraEnum.DUMMY) unknownLocationAbnormalities.v.Add(abnormality);
                    else sefiraSortingList.Find((Entry<SefiraEnum, List<CreatureModel>> entry) => entry.k == location).v.Add(abnormality);
                }
                if (unknownLocationAbnormalities.v.Count > 0) sefiraSortingList.Add(unknownLocationAbnormalities);
            }
            return sefiraSortingList;
        }

        public class Entry<K, V>
        {
            public K k { get; private set; }

            public V v { get; private set; }

            public Entry(K key, V value)
            {
                k = key;
                v = value;
            }
        }

        public static string GetUnitModelLocationText(UnitModel unit)
        {
            string location = "";
            PassageObjectModel currentPassage = unit.GetMovableNode().GetPassage();
            if (currentPassage == null)
            {
                location = "Location Unknown";
            }
            else location = String.Format("Currently in the {0} Department", Helpers.GetDepartmentBySefira(currentPassage.GetSefiraEnum()));
            return location;
        }

        public static SefiraEnum GetUnitModelLocationSefira(UnitModel unit)
        {
            PassageObjectModel currentPassage = unit.GetMovableNode().GetPassage();
            if (currentPassage != null)
            {
                return currentPassage.GetSefiraEnum();
            }
            return SefiraEnum.DUMMY;
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

        public static List<string> GetResistanceTypeValues(UnitModel unit)
        {
            DefenseInfo defenseInfo = unit.defense;
            List<string> defenseTypeInfo = new List<string>();
            defenseTypeInfo.Add(UIUtil.GetDefenseText(defenseInfo.GetDefenseType(defenseInfo.R)));
            defenseTypeInfo.Add(UIUtil.GetDefenseText(defenseInfo.GetDefenseType(defenseInfo.W)));
            defenseTypeInfo.Add(UIUtil.GetDefenseText(defenseInfo.GetDefenseType(defenseInfo.B)));
            defenseTypeInfo.Add(UIUtil.GetDefenseText(defenseInfo.GetDefenseType(defenseInfo.P)));
            return defenseTypeInfo;
        }

        public static string GetResistancesText(List<string> defenseInfo)
        {
            return String.Format("{0} RED Res, {1} WHITE Res, {2} BLACK Res, {3} PALE Res.",
                defenseInfo[(int)ResistanceTypes.RED], defenseInfo[(int)ResistanceTypes.WHITE], defenseInfo[(int)ResistanceTypes.BLACK], defenseInfo[(int)ResistanceTypes.PALE]);
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

        public static UnitModel TryFindAnySuppressibleTarget(string targetName, SefiraEnum targetDepartment)
        {
            UnitModel target = TryFindPanickedTarget(targetName);
            if (target == null) target = TryFindAbnormalityTarget(targetName, targetDepartment);
            if (target == null) target = TryFindOrdealTarget(targetName, targetDepartment);
            if (target == null) target = TryFindEventCreatureTarget(targetName);
            if (target == null) target = TryFindSefiraCoreTarget(targetName);
            return target;
        }

        public static UnitModel TryFindPanickedTarget(string targetName)
        {
            AgentModel panickedTarget = null;
            if (Helpers.AgentExists(targetName, out panickedTarget) && panickedTarget.IsPanic()) return panickedTarget;
            return null;
        }

        public static UnitModel TryFindAbnormalityTarget(string targetName, SefiraEnum targetDepartment)
        {
            List<UnitModel> targetsWithName = new List<UnitModel>();
            foreach (CreatureModel abnormality in SefiraManager.instance.GetEscapedCreatures())
            {
                if (abnormality.Unit.gameObject.activeSelf && targetName.Equals(abnormality.script.GetName())) targetsWithName.Add(abnormality);
            }
            if (targetsWithName.Count == 0) return null;
            if (targetsWithName.Count == 1) return targetsWithName[0];
            Random rand = new Random();
            if (targetDepartment == SefiraEnum.DUMMY) return targetsWithName[rand.Next(0, targetsWithName.Count)];
            List<UnitModel> targetsInDepartment = new List<UnitModel>();
            foreach (CreatureModel abnormality in targetsWithName)
            {
                SefiraEnum targetLocation = Helpers.GetUnitModelLocationSefira(abnormality);
                if (targetLocation == targetDepartment) targetsInDepartment.Add(abnormality);
            }
            if (targetsInDepartment.Count > 0) return targetsInDepartment[rand.Next(0, targetsInDepartment.Count)];
            return targetsWithName[rand.Next(0, targetsWithName.Count)];
        }

        public static UnitModel TryFindOrdealTarget(string targetName, SefiraEnum targetDepartment)
        {
            if (OrdealManager.instance.GetActivatedOrdeals().Count == 0) return null;
            List<UnitModel> targetsWithName = new List<UnitModel>();
            foreach (OrdealCreatureModel ordealCreature in OrdealManager.instance.GetOrdealCreatureList())
            {
                if (ordealCreature.OrdealBase.OrdealNameText(ordealCreature).Equals(targetName)) targetsWithName.Add(ordealCreature);
            }
            if (targetsWithName.Count == 0) return null;
            if (targetsWithName.Count == 1) return targetsWithName[0];
            Random rand = new Random();
            if (targetDepartment == SefiraEnum.DUMMY) return targetsWithName[rand.Next(0, targetsWithName.Count)];
            List<UnitModel> targetsInDepartment = new List<UnitModel>();
            foreach (OrdealCreatureModel ordealCreature in targetsWithName)
            {
                SefiraEnum targetLocation = Helpers.GetUnitModelLocationSefira(ordealCreature);
                if (targetLocation == targetDepartment) targetsInDepartment.Add(ordealCreature);
            }
            if (targetsInDepartment.Count > 0) return targetsInDepartment[rand.Next(0, targetsInDepartment.Count)];
            return targetsWithName[rand.Next(0, targetsWithName.Count)];
        }

        public static UnitModel TryFindEventCreatureTarget(string targetName)
        {
            foreach (EventCreatureModel eventCreature in SpecialEventManager.instance.GetEventCreatureList())
            {
                if (eventCreature.GetUnitName().Equals(targetName)) return eventCreature;
            }
            return null;
        }

        public static UnitModel TryFindSefiraCoreTarget(string targetName)
        {
            if (!SefiraBossManager.Instance.IsAnyBossSessionActivated()) return null;
            foreach (SefiraBossCreatureModel bossSefira in SefiraBossManager.Instance.CurrentBossBase.modelList)
            {
                if (bossSefira.script.GetName().Equals(targetName)) return bossSefira;
            }
            return null;
        }
    }
}
