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
            public string AgentName
            {
                get
                {
                    return agentName;
                }
            }
            private string agentName;

            public GetDetailedAgentInfoState(string agentName)
            {
                this.agentName = agentName;
            }
        }

        public static string Command(string agentName)
        {
            AgentModel discard;
            if (!Helpers.AgentExists(agentName, out discard)) return "failure|Action failed. The specified agent does not exist.";
            ThreadPool.QueueUserWorkItem(ExecuteCommand, new GetDetailedAgentInfoState(agentName));
            return String.Format("success|Getting the information of {0} to send as context...", agentName);
        }

        //TODO: the context sending in here can happen before the returned message is processed. Which like, holy negative latency or whatever but that's kinda awkward, so. Find a way to fix that mayhaps.
        //TODO: make formatting nicer. remove all caps (probably through changing the enums to be titlecase), un-converting the stats to words or roman numerals, etc
        public const int DETAILED_AGENT_NAME = 1;
        public static void ExecuteCommand(object state)
        {
            string agentName = ((GetDetailedAgentInfoState)state).AgentName;
            AgentModel agent = Helpers.GetAgentByName(agentName);
            string agentDepartment = Helpers.GetDepartmentBySefira(agent.GetCurrentSefira().sefiraEnum);
            int hp = (int)agent.hp;
            int maxHp = agent.maxHp;
            int sp = (int)agent.mental;
            int maxSp = agent.maxMental;
            Helpers.AgentWorkingState workingState = Helpers.GetAgentWorkingState(agent);
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
            //string egoWeaponSpeed = /////  figure this out later. the speed can be gotten with agent.Equipment.weapon.metaInfo.attackSpeed but that means nothing since its a float
            //same for the range
            string egoSuitName = agent.Equipment.armor.metaInfo.Name;
            string egoSuitRiskLevel = agent.Equipment.armor.metaInfo.Grade.ToString();
            string[] egoSuitDamageResistances = new string[4];
            egoSuitDamageResistances[(int)Helpers.ResistanceTypes.RED] = agent.Equipment.armor.metaInfo.defenseInfo.GetDefenseType(RwbpType.R).ToString();
            egoSuitDamageResistances[(int)Helpers.ResistanceTypes.WHITE] = agent.Equipment.armor.metaInfo.defenseInfo.GetDefenseType(RwbpType.W).ToString();
            egoSuitDamageResistances[(int)Helpers.ResistanceTypes.BLACK] = agent.Equipment.armor.metaInfo.defenseInfo.GetDefenseType(RwbpType.B).ToString();
            egoSuitDamageResistances[(int)Helpers.ResistanceTypes.PALE] = agent.Equipment.armor.metaInfo.defenseInfo.GetDefenseType(RwbpType.P).ToString();
            for (int i = 0; i < egoSuitDamageResistances.Length; i++)
            {
                switch (egoSuitDamageResistances[i])
                {
                    case "NONE":
                        egoSuitDamageResistances[i] = "NORMAL";
                        break;
                    case "SUPER_WEAKNESS":
                        egoSuitDamageResistances[i] = "VULNERABLE";
                        break;
                        //i feel like resistance also needs to be updated. not sure.
                        //TODO: replace this with the actual damage text replacement method you found
                }
            }
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
            if (egoGifts.Count == 0) egoGiftsCombine += "NONE";
            for (int i = 0; i < egoGifts.Count; i++)
            {
                egoGiftsCombine += egoGifts[i];
                if (i != egoGifts.Count - 1) egoGiftsCombine += " & ";
            }
            NeuroSDKHandler.SendContext(String.Format("{0}, {1} Team Agent" +
                "\n{2}/{3} HP, {4}/{5} SP" +
                "\nAGENT IS {6}" +
                "\nAGENT'S OVERALL LEVEL IS {7} (FORTITUDE:{8}, PRUDENCE:{9}, TEMPERANCE:{10}, JUSTICE:{11})" +
                "\nE.G.O WEAPON: {12} ({13} LEVEL, {14} TYPE DAMAGE, {15}-{16} DAMAGE RANGE)" +
                "\nE.G.O SUIT: {17} ({18} LEVEL, {19} RED RES., {20} WHITE RES., {21} BLACK RES., {22} PALE RES.)" +
                "\nE.G.O GIFTS: {23}", agentName, agentDepartment, hp, maxHp, sp, maxSp, workingState, agentLevel,
                statLevels[(int)Helpers.StatType.FORTITUDE], statLevels[(int)Helpers.StatType.PRUDENCE], statLevels[(int)Helpers.StatType.TEMPERANCE], statLevels[(int)Helpers.StatType.JUSTICE],
                egoWeaponName, egoWeaponRiskLevel, egoWeaponDamageType, egoWeaponMinDamage, egoWeaponMaxDamage,
                egoSuitName, egoSuitRiskLevel,
                egoSuitDamageResistances[(int)Helpers.ResistanceTypes.RED], egoSuitDamageResistances[(int)Helpers.ResistanceTypes.WHITE], egoSuitDamageResistances[(int)Helpers.ResistanceTypes.BLACK], egoSuitDamageResistances[(int)Helpers.ResistanceTypes.PALE],
                egoGiftsCombine), true);
        }
    }
}
