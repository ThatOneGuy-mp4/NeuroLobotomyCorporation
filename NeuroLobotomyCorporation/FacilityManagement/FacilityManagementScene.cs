using GameStatusUI;
using Harmony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class FacilityManagementScene : ActionScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_status":
                    return GetDayStatus.Command();
                case "get_agent_statuses":
                    return GetAgentStatuses.Command();
                case "get_detailed_agent_info":
                    return GetDetailedAgentInfo.Command(message[GetDetailedAgentInfo.DETAILED_AGENT_NAME]);
                case "get_abnormality_statuses":
                    return GetAbnormalityStatuses.Command();
                case "get_detailed_abnormality_info":
                    return GetDetailedAbnormalityInfo.Command(message[(int)GetDetailedAbnormalityInfo.Parameters.ABNORMALITY_NAME], bool.Parse(message[(int)GetDetailedAbnormalityInfo.Parameters.INCLUDE_BASIC_INFO]), bool.Parse(message[(int)GetDetailedAbnormalityInfo.Parameters.INCLUDE_MANAGERIAL_GUIDELINES]), bool.Parse(message[(int)GetDetailedAbnormalityInfo.Parameters.INCLUDE_WORK_SUCCESS_RATES]), bool.Parse(message[(int)GetDetailedAbnormalityInfo.Parameters.INCLUDE_ESCAPE_INFORMATION]));
                case "assign_work":
                    return AssignWork.Command(message[(int)AssignWork.Parameters.AGENT_NAME], message[(int)AssignWork.Parameters.ABNORMALITY_NAME], message[(int)AssignWork.Parameters.WORK_TYPE]);
                case "use_tool":
                    return UseTool.Command(message[(int)UseTool.Parameters.AGENT_NAME], message[(int)UseTool.Parameters.ABNORMALITY_NAME]);
            }
            return "Command " + message[COMMAND_INDEX] + " does not exist in scene FacilityManagementScene.";
        }

        //private EnergyController _energyController = null;
        //public string GetDayStatus()
        //{
        //    string status = "";
        //    int energyCollected = (int)EnergyModel.instance.GetEnergy();
        //    int energyRequired = (int)StageTypeInfo.instnace.GetEnergyNeed(PlayerModel.instance.GetDay());
        //    int energyProgress = (int)((float)energyCollected / energyRequired * 100);
        //    int qliphothMeltdownLevel = CreatureOverloadManager.instance.GetQliphothOverloadLevel();
        //    int qliphothMeltdownGauge = -1;
        //    int qliphothMeltdownMax = CreatureOverloadManager.instance.qliphothOverloadMax;
        //    try
        //    {
        //        //i think potentially the FieldInfo gotten by AccessTools.Field only needs to be gotten once? and then it auto-updates along with the value? test that later.
        //        FieldInfo gaugeInfo = typeof(CreatureOverloadManager).GetField("qliphothOverloadGauge", BindingFlags.Instance | BindingFlags.NonPublic);
        //        qliphothMeltdownGauge = (int)gaugeInfo.GetValue(CreatureOverloadManager.instance);
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //    int gaugesBeforeNextMeltdown = qliphothMeltdownMax - qliphothMeltdownGauge;
        //    string ordealType = "";
        //    int overloadIsolateNum = -1;
        //    try
        //    {
        //        FieldInfo ordealInfo = typeof(CreatureOverloadManager).GetField("_nextOrdeal", BindingFlags.Instance | BindingFlags.NonPublic);
        //        OrdealBase ordeal = (OrdealBase)ordealInfo.GetValue(CreatureOverloadManager.instance);
        //        if (ordeal != null)
        //        {
        //            string getTypeOfOrdeal = ordeal.GetType().Name;
        //            switch (getTypeOfOrdeal) //probably a better way to do this that's still fast. also this doesn't account for white ordeals since those all trigger in one class.
        //            {
        //                case "MachineDawnOrdeal":
        //                    ordealType = "Dawn of Green";
        //                    break;
        //                case "MachineNoonOrdeal":
        //                    ordealType = "Noon of Green";
        //                    break;
        //                case "MachineDuskOrdeal":
        //                    ordealType = "Dusk of Green";
        //                    break;
        //                case "MachineMidnightOrdeal":
        //                    ordealType = "Midnight of Green";
        //                    break;
        //                case "OutterGodDawnOrdeal":
        //                    ordealType = "Dawn of Violet";
        //                    break;
        //                case "OutterGodNoonOrdeal":
        //                    ordealType = "Noon of Violet";
        //                    break;
        //                case "OutterGodMidnightOrdeal":
        //                    ordealType = "Midnight of Violet";
        //                    break;
        //                case "CircusDawnOrdeal":
        //                    ordealType = "Dawn of Crimson";
        //                    break;
        //                case "CircusNoonOrdeal":
        //                    ordealType = "Noon of Crimson";
        //                    break;
        //                case "CircusDuskOrdeal":
        //                    ordealType = "Dusk of Crimson";
        //                    break;
        //                case "BugDawnOrdeal":
        //                    ordealType = "Dawn of Amber";
        //                    break;
        //                case "BugDuskOrdeal":
        //                    ordealType = "Dusk of Amber";
        //                    break;
        //                case "BugMidnightOrdeal":
        //                    ordealType = "Midnight of Amber";
        //                    break;
        //                case "ScavengerNoonOrdeal":
        //                    ordealType = "Noon of Indigo";
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            if (_energyController == null) _energyController = GameObject.FindObjectOfType<EnergyController>();
        //            overloadIsolateNum = Int32.Parse(_energyController.OverLoadIsolateNumText.text);
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //    EmergencyLevel trumpetLevel = PlayerModel.emergencyController.currentLevel;
        //    string nextMeltdownDesc = "";
        //    if (overloadIsolateNum == -1)
        //    {
        //        nextMeltdownDesc = String.Format("The next meltdown will spawn the {0}.", ordealType);
        //    }
        //    else
        //    {
        //        string unitOrUnits = "";
        //        if (overloadIsolateNum == 1) unitOrUnits = "unit";
        //        else unitOrUnits = "units";
        //        nextMeltdownDesc = String.Format("The next meltdown will cause {0} containment {1} to become overloaded.", overloadIsolateNum, unitOrUnits);
        //    }
        //    string trumpetDesc = "";
        //    switch (trumpetLevel)
        //    {
        //        case EmergencyLevel.NORMAL:
        //            break;
        //        case EmergencyLevel.LEVEL1:
        //            trumpetDesc = "\nThe First Trumpet is playing. Deal with the situation before it becomes a threat.";
        //            break;
        //        case EmergencyLevel.LEVEL2:
        //            trumpetDesc = "\nThe Second Trumpet is playing. Deal with the situation before more casualties arise.";
        //            break;
        //        case EmergencyLevel.LEVEL3:
        //            trumpetDesc = "\nThe Third Trumpet is playing. Deal with the situation before nothing is left to save.";
        //            break;
        //        case EmergencyLevel.CHAOS:
        //            trumpetDesc = "\nThe emergency level is in a state of chaos. This shouldn't trigger outside of core suppressions so I don't think you should be seeing this.";
        //            break;
        //        default:
        //            trumpetDesc = "\nThe trumpet level is broken. Complain to the mod developer about it.";
        //            break;
        //    }
        //    status = String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
        //        "\nThe current Qliphoth Meltdown Level is {3}, and the next meltdown will trigger after {4} gauges have been filled." +
        //        "\n{5}" +
        //        "{6}", energyCollected, energyRequired, energyProgress, qliphothMeltdownLevel, gaugesBeforeNextMeltdown, nextMeltdownDesc, trumpetDesc);
        //    return status;
        //}

        //public string GetAgentStatuses()
        //{
        //    string allAgentStatuses = "";
        //    foreach (AgentModel agent in AgentManager.instance.GetAgentList())
        //    {
        //        string name = agent.GetUnitName();
        //        string department = GetDepartmentBySefira(agent.GetCurrentSefira().sefiraEnum);
        //        int hp = (int)agent.hp;
        //        int maxHp = agent.maxHp;
        //        int sp = (int)agent.mental;
        //        int maxSp = agent.maxMental;
        //        AgentWorkingState agentCurrentState = GetAgentWorkingState(agent);
        //        allAgentStatuses += String.Format("{0}, {1} Team Agent: {2}/{3} HP, {4}/{5} SP, {6}\n", name, department, hp, maxHp, sp, maxSp, agentCurrentState.ToString());
        //    }
        //    return allAgentStatuses;
        //}

        //public static AgentModel GetAgentByName(string name)
        //{
        //    foreach(AgentModel agent in AgentManager.instance.GetAgentList())
        //    {
        //        if (agent.GetUnitName().Equals(name)) return agent;
        //    }
        //    return null;
        //}

        //public bool AgentExists(string agentName, out AgentModel agent)
        //{
        //    agent = GetAgentByName(agentName);
        //    return (agent != null);
        //}

        //public CreatureModel GetAbnormalityByName(string name)
        //{
        //    foreach(CreatureModel abnormality in CreatureManager.instance.GetCreatureList())
        //    {
        //        if (abnormality.GetUnitName().Equals(name)) return abnormality;
        //    }
        //    return null;
        //}

        //public bool AbnormalityExists(string abnormalityName, out CreatureModel abnormality)
        //{
        //    abnormality = GetAbnormalityByName(abnormalityName);
        //    return (abnormality != null);
        //}

        //private enum AgentWorkingState
        //{
        //    UNKNOWN,
        //    IDLE,
        //    WORKING,
        //    SUPPRESSING,
        //    DEAD,
        //    PANICKING,
        //    UNCONTROLLABLE
        //}
        //private AgentWorkingState GetAgentWorkingState(AgentModel agent)
        //{
        //    if (agent.IsDead())
        //    {
        //        return AgentWorkingState.DEAD;
        //    }
        //    else if (agent.IsPanic())
        //    {
        //        return AgentWorkingState.PANICKING;
        //    }
        //    else if (agent.CannotControll())
        //    {
        //        return AgentWorkingState.UNCONTROLLABLE;
        //    }
        //    else if (agent.IsSuppressing())
        //    {
        //        return AgentWorkingState.SUPPRESSING;
        //    }
        //    else if (agent.GetState() == AgentAIState.MANAGE || agent.GetState() == AgentAIState.OBSERVE) //idk what tf the difference between these are. keep an eye for whatever it is.
        //    {
        //        return AgentWorkingState.WORKING;
        //    }
        //    else if (agent.IsIdle())
        //    {
        //        return AgentWorkingState.IDLE;
        //    }
        //    return AgentWorkingState.UNKNOWN;
        //}


        //private enum StatType
        //{
        //    FORTITUDE = 0,
        //    PRUDENCE = 1,
        //    TEMPERANCE = 2,
        //    JUSTICE = 3
        //}
        //private enum ResistanceTypes
        //{
        //    RED = 0,
        //    WHITE = 1,
        //    BLACK = 2,
        //    PALE = 3
        //}

        //private class GetDetailedAgentInfoState
        //{
        //    public string AgentName
        //    {
        //        get
        //        {
        //            return agentName;
        //        }
        //    }
        //    private string agentName;
           
        //    public GetDetailedAgentInfoState(string agentName)
        //    {
        //        this.agentName = agentName;
        //    }
        //}

        //public string GetDetailedAgentInfo(string agentName)
        //{
        //    AgentModel discard;
        //    if (!AgentExists(agentName, out discard)) return "failure|Specified agent does not exist.";
        //    ThreadPool.QueueUserWorkItem(ExecuteGetDetailedAgentInfo, new GetDetailedAgentInfoState(agentName));
        //    return String.Format("success|Getting the information of {0} to send as context...", agentName);
        //}

        ////TODO: the context sending in here happens before the returned message is processed. Which like, holy negative latency or whatever but that's kinda awkward, so. Find a way to fix that mayhaps.
        ////TODO: make formatting nicer. remove all caps (probably through changing the enums to be titlecase), un-converting the stats to words or roman numerals, etc
        //private const int DETAILED_AGENT_NAME = 1;
        //public void ExecuteGetDetailedAgentInfo(object state)
        //{
        //    string agentName = ((GetDetailedAgentInfoState)state).AgentName;
        //    AgentModel agent = GetAgentByName(agentName);
        //    string agentDepartment = GetDepartmentBySefira(agent.GetCurrentSefira().sefiraEnum);
        //    int hp = (int)agent.hp;
        //    int maxHp = agent.maxHp;
        //    int sp = (int)agent.mental;
        //    int maxSp = agent.maxMental;
        //    AgentWorkingState workingState = GetAgentWorkingState(agent);
        //    string agentLevel = AgentModel.GetLevelGradeText(agent);
        //    string[] statLevels = new string[4];
        //    statLevels[(int)StatType.FORTITUDE] = AgentModel.GetLevelGradeText(agent.Rstat);
        //    statLevels[(int)StatType.PRUDENCE] = AgentModel.GetLevelGradeText(agent.Wstat);
        //    statLevels[(int)StatType.TEMPERANCE] = AgentModel.GetLevelGradeText(agent.Bstat);
        //    statLevels[(int)StatType.JUSTICE] = AgentModel.GetLevelGradeText(agent.Pstat);
        //    string egoWeaponName = agent.Equipment.weapon.metaInfo.Name;
        //    string egoWeaponRiskLevel = agent.Equipment.weapon.metaInfo.Grade.ToString();
        //    string egoWeaponDamageType = GetDamageColorByRwbpType(agent.Equipment.weapon.metaInfo.damageInfo.type);
        //    int egoWeaponMinDamage = (int)agent.Equipment.weapon.metaInfo.damageInfo.min;
        //    int egoWeaponMaxDamage = (int)agent.Equipment.weapon.metaInfo.damageInfo.max;
        //    //string egoWeaponSpeed = /////  figure this out later. the speed can be gotten with agent.Equipment.weapon.metaInfo.attackSpeed but that means nothing since its a float
        //    //same for the range
        //    string egoSuitName = agent.Equipment.armor.metaInfo.Name;
        //    string egoSuitRiskLevel = agent.Equipment.armor.metaInfo.Grade.ToString();
        //    string[] egoSuitDamageResistances = new string[4];
        //    egoSuitDamageResistances[(int)ResistanceTypes.RED] = agent.Equipment.armor.metaInfo.defenseInfo.GetDefenseType(RwbpType.R).ToString();
        //    egoSuitDamageResistances[(int)ResistanceTypes.WHITE] = agent.Equipment.armor.metaInfo.defenseInfo.GetDefenseType(RwbpType.W).ToString();
        //    egoSuitDamageResistances[(int)ResistanceTypes.BLACK] = agent.Equipment.armor.metaInfo.defenseInfo.GetDefenseType(RwbpType.B).ToString();
        //    egoSuitDamageResistances[(int)ResistanceTypes.PALE] = agent.Equipment.armor.metaInfo.defenseInfo.GetDefenseType(RwbpType.P).ToString();
        //    for(int i = 0; i < egoSuitDamageResistances.Length; i++)
        //    {
        //        switch (egoSuitDamageResistances[i])
        //        {
        //            case "NONE":
        //                egoSuitDamageResistances[i] = "NORMAL";
        //                break;
        //            case "SUPER_WEAKNESS":
        //                egoSuitDamageResistances[i] = "VULNERABLE";
        //                break;
        //            //i feel like resistance also needs to be updated. not sure.
        //        }
        //    }
        //    List<string> egoGifts = new List<string>();
        //    foreach(EGOgiftModel gift in agent.Equipment.gifts.addedGifts)
        //    {
        //        egoGifts.Add(gift.metaInfo.Name);
        //    }
        //    foreach (EGOgiftModel gift in agent.Equipment.gifts.replacedGifts)
        //    {
        //        egoGifts.Add(gift.metaInfo.Name);
        //    }
        //    string egoGiftsCombine = "";
        //    if (egoGifts.Count == 0) egoGiftsCombine += "NONE";
        //    for(int i = 0; i < egoGifts.Count; i++)
        //    {
        //        egoGiftsCombine += egoGifts[i];
        //        if (i != egoGifts.Count - 1) egoGiftsCombine += " & ";
        //    }
        //    Harmony_Patch.SendContext(String.Format("{0}, {1} Team Agent" +
        //        "\n{2}/{3} HP, {4}/{5} SP" +
        //        "\nAGENT IS {6}" +
        //        "\nAGENT'S OVERALL LEVEL IS {7} (FORTITUDE:{8}, PRUDENCE:{9}, TEMPERANCE:{10}, JUSTICE:{11})" +
        //        "\nE.G.O WEAPON: {12} ({13} LEVEL, {14} TYPE DAMAGE, {15}-{16} DAMAGE RANGE)" +
        //        "\nE.G.O SUIT: {17} ({18} LEVEL, {19} RED RES., {20} WHITE RES., {21} BLACK RES., {22} PALE RES.)" +
        //        "\nE.G.O GIFTS: {23}", agentName, agentDepartment, hp, maxHp, sp, maxSp, workingState, agentLevel,
        //        statLevels[(int)StatType.FORTITUDE], statLevels[(int)StatType.PRUDENCE], statLevels[(int)StatType.TEMPERANCE], statLevels[(int)StatType.JUSTICE],
        //        egoWeaponName, egoWeaponRiskLevel, egoWeaponDamageType, egoWeaponMinDamage, egoWeaponMaxDamage,
        //        egoSuitName, egoSuitRiskLevel,
        //        egoSuitDamageResistances[(int)ResistanceTypes.RED], egoSuitDamageResistances[(int)ResistanceTypes.WHITE], egoSuitDamageResistances[(int)ResistanceTypes.BLACK], egoSuitDamageResistances[(int)ResistanceTypes.PALE],
        //        egoGiftsCombine), true);
        //}

        //private string GetDamageColorByRwbpType(RwbpType type)
        //{
        //    switch (type)
        //    {
        //        case RwbpType.R:
        //            return "RED";
        //        case RwbpType.W:
        //            return "WHITE";
        //        case RwbpType.B:
        //            return "BLACK";
        //        case RwbpType.P:
        //            return "PALE";
        //    }
        //    return "Damage type could not be found. Complain to the mod developer about it.";
        //}

        //public static string GetDepartmentBySefira(SefiraEnum sefira)
        //{
        //    switch (sefira)
        //    {
        //        case SefiraEnum.MALKUT:
        //            return "Control";
        //        case SefiraEnum.YESOD:
        //            return "Information";
        //        case SefiraEnum.HOD:
        //            return "Training";
        //        case SefiraEnum.NETZACH:
        //            return "Safety";
        //        case SefiraEnum.TIPERERTH1:
        //            return "Upper Central Command";
        //        case SefiraEnum.TIPERERTH2:
        //            return "Lower Central Command";
        //        case SefiraEnum.GEBURAH:
        //            return "Disciplinary";
        //        case SefiraEnum.CHESED:
        //            return "Welfare";
        //        case SefiraEnum.BINAH:
        //            return "Extraction";
        //        case SefiraEnum.CHOKHMAH:
        //            return "Record";
        //        case SefiraEnum.KETHER:
        //            return "Architecture";
        //        case SefiraEnum.DAAT:
        //            return "Carmen's Chamber";
        //        case SefiraEnum.DUMMY:
        //            return "Dummy";
        //    }
        //    return "Department could not be gotten from the Sephira. Complain to the mod developer about it.";
        //}

        //private enum AbnormalityWorkingState
        //{
        //    UNKNOWN,
        //    IDLE,
        //    WORKING,
        //    BREACHING,
        //    COOLDOWN
        //}

        //private AbnormalityWorkingState GetAbnormalityWorkingState(CreatureModel abnormality)
        //{
        //    if (abnormality.IsEscaped())
        //    {
        //        return AbnormalityWorkingState.BREACHING;
        //    }
        //    if (abnormality.IsWorkingState())
        //    {
        //        return AbnormalityWorkingState.WORKING;
        //    }
        //    if (abnormality.state.Equals(CreatureState.WAIT_COOLDOWN) || abnormality.GetFeelingState() != CreatureFeelingState.NONE)
        //    {
        //        return AbnormalityWorkingState.COOLDOWN;
        //    }
        //    return AbnormalityWorkingState.IDLE;
        //}

        //public string GetAbnormalityStatuses()
        //{
        //    string status = "";
        //    foreach(CreatureModel abnormality in CreatureManager.instance.GetCreatureList())
        //    {
        //        string abnormalityName = abnormality.GetUnitName();
        //        //string riskLevel = abnormality.GetRiskLevel(); TODO: add the risk level
        //        string isTool = "";
        //        if (abnormality.metaInfo.creatureWorkType == CreatureWorkType.KIT) isTool = "Tool ";
        //        string department = GetDepartmentBySefira(abnormality.sefira.sefiraEnum);
        //        string qliphothCounter = "";
        //        if (!(abnormality.metaInfo.creatureWorkType == CreatureWorkType.KIT) && !abnormality.observeInfo.GetObserveState(CreatureModel.regionName[0])) //0 is Lob Corp's basic info observe info index
        //        {
        //            qliphothCounter = "?";
        //        }else if(abnormality.metaInfo.creatureWorkType == CreatureWorkType.KIT || (abnormality.observeInfo.GetObserveState(CreatureModel.regionName[0]) && !abnormality.script.HasRoomCounter()))
        //        {
        //            qliphothCounter = "X";
        //        }
        //        else
        //        {
        //            qliphothCounter = abnormality.qliphothCounter.ToString();
        //        }
        //        string abnormalityStatus = "";
        //        switch (GetAbnormalityWorkingState(abnormality))
        //        {
        //            case AbnormalityWorkingState.BREACHING:
        //                abnormalityStatus = "! BREACHING !";
        //                break;
        //            case AbnormalityWorkingState.WORKING:
        //                abnormalityStatus = "In Work";
        //                break;
        //            case AbnormalityWorkingState.IDLE:
        //                abnormalityStatus = "Idle";
        //                break;
        //            case AbnormalityWorkingState.COOLDOWN:
        //                abnormalityStatus = "On Cooldown";
        //                break;
        //        } //probably not gonna work right with tool abnos. So considering figuring out how those work perhaps.
        //        status += String.Format("\n{0}, {1} Department {2}Abnormality: {3} Qliphoth Counter, {4}", abnormalityName, department, isTool, qliphothCounter, abnormalityStatus);
        //    }
        //    return status;
        //}

        //private enum AssignWorkParameters
        //{
        //    AGENT_NAME = 1,
        //    ABNORMALITY_NAME = 2,
        //    WORK_TYPE = 3
        //}

        ////TODO: add all the weird exceptions for special work types (der freischutz, red riding hood, el piano, one sin + whitenight)
        //public string AssignWork(string agentName, string abnoName, string workType)
        //{
        //    AgentModel agent = null;
        //    CreatureModel abnormality = null;
        //    if (!AgentExists(agentName, out agent)) return "failure|Action failed. Specified agent does not exist.";
        //    if (!AbnormalityExists(abnoName, out abnormality)) return "failure|Action failed. Specified Abnormality does not exist.";
        //    int workId = 0;
        //    switch (workType)
        //    {
        //        case "Instinct":
        //            workId = 1;
        //            break;
        //        case "Insight":
        //            workId = 2;
        //            break;
        //        case "Attachment":
        //            workId = 3;
        //            break;
        //        case "Repression":
        //            workId = 4;
        //            break;
        //        default:
        //            return "failure|Action failed. Specified work type is not valid.";
        //    }
        //    switch (GetAgentWorkingState(agent))
        //    {
        //        case AgentWorkingState.WORKING:
        //            return "failure|Work could not be assigned because the specified agent is already working.";
        //        case AgentWorkingState.DEAD:
        //            return "failure|Work could not be assigned because the specified agent is dead.";
        //        case AgentWorkingState.PANICKING:
        //            return "failure|Work could not be assigned because the specified agent is panicking.";
        //        case AgentWorkingState.UNCONTROLLABLE:
        //            return "failure|Work could not be assigned because the specified agent is uncontrollable.";
        //    }
        //    switch (GetAbnormalityWorkingState(abnormality))
        //    {
        //        case AbnormalityWorkingState.BREACHING:
        //            return "failure|Work could not be assigned because the specified Abnormality is currently breaching.";
        //        case AbnormalityWorkingState.WORKING:
        //            return "failure|Work could not be assigned because the specified Abnormality is already being worked on.";
        //        case AbnormalityWorkingState.COOLDOWN:
        //            return "failure|Work could not be assigned because the specified Abnormality is in cooldown. Try again in a bit.";
        //    }
        //    workId = SefiraBossManager.Instance.GetWorkId(workId);
        //    string workTypeAfterCheckingForMalkuth = "";
        //    switch (workId) //I hate this shit as much as you do, not my fault project moon forgot what an enum was
        //    {
        //        case 1:
        //            workTypeAfterCheckingForMalkuth = "Instinct";
        //            break;
        //        case 2:
        //            workTypeAfterCheckingForMalkuth = "Insight";
        //            break;
        //        case 3:
        //            workTypeAfterCheckingForMalkuth = "Attachment";
        //            break;
        //        case 4:
        //            workTypeAfterCheckingForMalkuth = "Repression";
        //            break;
        //        default:
        //            workTypeAfterCheckingForMalkuth = "Unknown. Complain to the mod developer about it.";
        //            break;
        //    }
        //    ThreadPool.QueueUserWorkItem(ExecuteAssignWork, new AssignWorkState(agentName, abnoName, workType));
        //    return String.Format("success|{0} begins their {1} Work with {2}.", agentName, workTypeAfterCheckingForMalkuth, abnoName);
        //}

        //private class AssignWorkState
        //{
        //    public string AgentName
        //    {
        //        get
        //        {
        //            return agentName;
        //        }
        //    }
        //    private string agentName;
        //    public string AbnormalityName
        //    {
        //        get
        //        {
        //            return abnormalityName;
        //        }
        //    }
        //    private string abnormalityName;
        //    public string WorkType
        //    {
        //        get
        //        {
        //            return workType;
        //        }
        //    }
        //    private string workType;
        //    public AssignWorkState(string agentName, string abnormalityName, string workType)
        //    {
        //        this.agentName = agentName;
        //        this.abnormalityName = abnormalityName;
        //        this.workType = workType;
        //    }
        //}

        //private static List<SystemLogScript.CreatureSystemLog> neuroCreatedLogItems = new List<SystemLogScript.CreatureSystemLog>();
        //public void ExecuteAssignWork(object state)
        //{
        //    AssignWorkState parameters = (AssignWorkState)state;
        //    string agentName = parameters.AgentName;
        //    string abnormalityName = parameters.AbnormalityName;
        //    string workType = parameters.WorkType;
        //    int workId = 0;
        //    switch (workType)
        //    {
        //        case "Instinct":
        //            workId = 1;
        //            break;
        //        case "Insight":
        //            workId = 2;
        //            break;
        //        case "Attachment":
        //            workId = 3;
        //            break;
        //        case "Repression":
        //            workId = 4;
        //            break;
        //    }

        //    workId = SefiraBossManager.Instance.GetWorkId(workId);
        //    SkillTypeInfo data = SkillTypeList.instance.GetData(workId);
        //    AgentModel agent = GetAgentByName(agentName);
        //    CreatureModel abnormality = GetAbnormalityByName(abnormalityName);           
        //    Sprite workSprite = CommandWindow.CommandWindow.CurrentWindow.GetWorkSprite((RwbpType)workId); //does this work with no command instance open. we shall see.
        //    agent.ManageCreature(abnormality, data, workSprite);
        //    agent.counterAttackEnabled = false;
        //    abnormality.Unit.room.OnWorkAllocated(agent);
        //    abnormality.script.OnWorkAllocated(data, agent);

        //    SystemLogScript logScript = GameObject.FindObjectOfType<SystemLogScript>();
        //    string workTypeAfterCheckingForMalkuth = "";
        //    if (SefiraBossManager.Instance.CheckBossActivation(SefiraEnum.MALKUT) || SefiraBossManager.Instance.IsKetherBoss(KetherBossType.E1))
        //    {
        //        workTypeAfterCheckingForMalkuth = "Unknown";
        //    }
        //    else
        //    {
        //        workTypeAfterCheckingForMalkuth = data.calledName;
        //    }
        //    LogItemScript neuroLog = logScript.script.MakeText("");
        //    neuroLog.SetText(String.Format("▶  <color=#66bfcd>{0}</color> begins their <color=#84bd36>{1}</color> with <color=#ef9696>{2}</color>.", agentName, workTypeAfterCheckingForMalkuth, abnormalityName));
        //    SystemLogScript.CreatureSystemLog neuroCreatureLog = GetNeuroCreatureSystemLog(abnormality);
        //    if (neuroCreatureLog == null)
        //    {
        //        neuroCreatureLog = new SystemLogScript.CreatureSystemLog(abnormality.instanceId);
        //        neuroCreatedLogItems.Add(neuroCreatureLog);
        //    }
        //    neuroCreatureLog.AddLog(neuroLog);
        //}

        ////All of the normal ways to put something in the system log crash the fucking game and I have literally no idea why.
        ////I found a workaround, however, said workaround does not sort the logs correctly. And I can't sort them myself because that also crashes the game.
        ////So I'm using this harmony patch to sort them every time one is added. Because doing it with a harmony patch doesn't crash the game. Whatever man.
        //public static void SortSystemLogs(LoggingScript __instance)
        //{
        //    __instance.Sort();
        //}

        ////This one updates the logs when observation level increases. Because yes I have to do this manually too.
        ////Pretty much stolen directly from SystemLogScript.
        //public static void UpdateNeuroLogsObservationLevel(string notice, params object[] param)
        //{
        //    if (NoticeName.CreatureObserveLevelAdded == notice)
        //    {
        //        CreatureModel creatureModel = param[0] as CreatureModel;
        //        if (creatureModel.observeInfo.GetObserveState(CreatureModel.regionName[0]))
        //        {
        //            SystemLogScript.CreatureSystemLog creatureSystemLog = GetNeuroCreatureSystemLog(creatureModel);
        //            if (creatureSystemLog != null)
        //            {
        //                creatureSystemLog.OnObserveLevelUpdated(creatureModel);
        //            }
        //        }
        //    }
        //}

        //private static SystemLogScript.CreatureSystemLog GetNeuroCreatureSystemLog(CreatureModel abnormality)
        //{
        //    foreach (SystemLogScript.CreatureSystemLog creatureSystemLog in neuroCreatedLogItems)
        //    {
        //        if (creatureSystemLog.targetId == abnormality.instanceId)
        //        {
        //            return creatureSystemLog;
        //        }
        //    }
        //    return null;
        //}
    }
}
