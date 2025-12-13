using Assets.Scripts.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class GetDetailedAgentInfo
    {
        private class GetDetailedAgentInfoState
        {
            public AgentModel Agent{ get; set; }
            public GetDetailedAgentInfoState(AgentModel agent)
            {
                Agent = agent;
            }
        }

        public static string Command(string agentName)
        {
            AgentModel agent;
            if (!Helpers.AgentExists(agentName, out agent)) return "failure|Action failed. The specified agent does not exist.";
            ThreadPool.QueueUserWorkItem(ExecuteCommand, new GetDetailedAgentInfoState(agent));
            return String.Format("success|Getting the information of {0} to send as context...", agentName);
        }

        public const int DETAILED_AGENT_NAME = 1;
        public static void ExecuteCommand(object state)
        {
            AgentModel agent = ((GetDetailedAgentInfoState)state).Agent;
            string agentDepartment = Helpers.GetDepartmentBySefira(agent.GetCurrentSefira().sefiraEnum);
            int hp = (int)agent.hp;
            int maxHp = agent.maxHp;
            int sp = (int)agent.mental;
            int maxSp = agent.maxMental;
            Helpers.AgentWorkingState workingState = Helpers.GetAgentWorkingState(agent);
            string agentStateDesc = "";
            switch (workingState)
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
                    agentStateDesc = "A Heretic";
                    break;
            }
            string agentLevel = AgentModel.GetLevelGradeText(agent);
            string[] statLevels = new string[4];
            statLevels[(int)Helpers.StatType.FORTITUDE] = AgentModel.GetLevelGradeText(agent.Rstat);
            statLevels[(int)Helpers.StatType.PRUDENCE] = AgentModel.GetLevelGradeText(agent.Wstat);
            statLevels[(int)Helpers.StatType.TEMPERANCE] = AgentModel.GetLevelGradeText(agent.Bstat);
            statLevels[(int)Helpers.StatType.JUSTICE] = AgentModel.GetLevelGradeText(agent.Pstat);
            string egoWeaponName = agent.Equipment.weapon.metaInfo.Name;
            string egoWeaponRiskLevel = agent.Equipment.weapon.metaInfo.Grade.ToString();
            string egoWeaponDamageType = Helpers.GetDamageColorByRwbpType(agent.Equipment.weapon.metaInfo.damageInfo.type);
            int egoWeaponMinDamage = (int)agent.Equipment.weapon.metaInfo.damageInfo.min;
            int egoWeaponMaxDamage = (int)agent.Equipment.weapon.metaInfo.damageInfo.max;
            string egoWeaponSpeed, egoWeaponRange;
            Inventory.InventoryItemDescGetter.GetWeaponDesc(agent.Equipment.weapon.metaInfo, out egoWeaponSpeed, out egoWeaponRange);
            string egoSuitName = agent.Equipment.armor.metaInfo.Name;
            string egoSuitRiskLevel = agent.Equipment.armor.metaInfo.Grade.ToString();
            string[] egoSuitDamageResistances = new string[4];
            egoSuitDamageResistances[(int)Helpers.ResistanceTypes.RED] = EnumTextConverter.GetDefenseType(agent.Equipment.armor.metaInfo.defenseInfo.GetDefenseType(RwbpType.R));
            egoSuitDamageResistances[(int)Helpers.ResistanceTypes.WHITE] = EnumTextConverter.GetDefenseType(agent.Equipment.armor.metaInfo.defenseInfo.GetDefenseType(RwbpType.W));
            egoSuitDamageResistances[(int)Helpers.ResistanceTypes.BLACK] = EnumTextConverter.GetDefenseType(agent.Equipment.armor.metaInfo.defenseInfo.GetDefenseType(RwbpType.B));
            egoSuitDamageResistances[(int)Helpers.ResistanceTypes.PALE] = EnumTextConverter.GetDefenseType(agent.Equipment.armor.metaInfo.defenseInfo.GetDefenseType(RwbpType.P));
            List<string> egoGifts = new List<string>();
            foreach (EGOgiftModel gift in agent.Equipment.gifts.addedGifts)
            {
                egoGifts.Add(gift.metaInfo.Name);
            }
            foreach (EGOgiftModel gift in agent.Equipment.gifts.replacedGifts)
            {
                egoGifts.Add(gift.metaInfo.Name);
            }
            string egoGiftsCombine = "";
            if (egoGifts.Count == 0) egoGiftsCombine += "None";
            for (int i = 0; i < egoGifts.Count; i++)
            {
                egoGiftsCombine += egoGifts[i];
                if (i != egoGifts.Count - 1) egoGiftsCombine += " & ";
            }
            NeuroSDKHandler.SendContext(String.Format("{0}, {1} Team Agent" +
                "\n{2}/{3} HP, {4}/{5} SP" +
                "\nAgent is Currently {6}" +
                "\nOverall Level: {7} (Fortitude:{8}, Prudence:{9}, Temperance:{10}, Justice:{11})" +
                "\nE.G.O Weapon: {12} ({13} Level, {14} Type Damage, {15}-{16} Damage Range, {17} Speed, {18} Range)" +
                "\nE.G.O Suit: {19} ({20} Level, {21} Red Res., {22} White Res., {23} Black Res., {24} Pale Res.)" +
                "\nE.G.O Gifts: {25}", agent.GetUnitName(), agentDepartment, hp, maxHp, sp, maxSp, agentStateDesc, agentLevel,
                statLevels[(int)Helpers.StatType.FORTITUDE], statLevels[(int)Helpers.StatType.PRUDENCE], statLevels[(int)Helpers.StatType.TEMPERANCE], statLevels[(int)Helpers.StatType.JUSTICE],
                egoWeaponName, egoWeaponRiskLevel, egoWeaponDamageType, egoWeaponMinDamage, egoWeaponMaxDamage, egoWeaponSpeed, egoWeaponRange,
                egoSuitName, egoSuitRiskLevel,
                egoSuitDamageResistances[(int)Helpers.ResistanceTypes.RED], egoSuitDamageResistances[(int)Helpers.ResistanceTypes.WHITE], egoSuitDamageResistances[(int)Helpers.ResistanceTypes.BLACK], egoSuitDamageResistances[(int)Helpers.ResistanceTypes.PALE],
                egoGiftsCombine), true);
        }
    }
}
