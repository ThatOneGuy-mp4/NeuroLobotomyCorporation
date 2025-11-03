using GameStatusUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class GetDayStatus
    {
        private static EnergyController _energyController = null;
        public static string Command()
        {
            string status = "";
            int energyCollected, energyRequired, energyProgress;
            GetEnergyInformation(out energyCollected, out energyRequired, out energyProgress);
            int qliphothMeltdownLevel, qliphothMeltdownGauge, qliphothMeltdownMax, gaugesBeforeNextMeltdown;
            GetQliphothMeltdownInformation(out qliphothMeltdownLevel, out qliphothMeltdownGauge, out qliphothMeltdownMax, out gaugesBeforeNextMeltdown);
            string nextMeltdownDesc = GetNextMeltdownInformation();
            string trumpetDesc = GetTrumpetInformation();
            status = String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
                "\nThe current Qliphoth Meltdown Level is {3}, and the next meltdown will trigger after {4} gauges have been filled." +
                "\n{5}" +
                "{6}", energyCollected, energyRequired, energyProgress, qliphothMeltdownLevel, gaugesBeforeNextMeltdown, nextMeltdownDesc, trumpetDesc);
            return status;
        }

        public static void GetEnergyInformation(out int energyCollected, out int energyRequired, out int energyProgress)
        {
            energyCollected = (int)EnergyModel.instance.GetEnergy();
            energyRequired = (int)StageTypeInfo.instnace.GetEnergyNeed(PlayerModel.instance.GetDay());
            energyProgress = (int)((float)energyCollected / energyRequired * 100);
        }

        public static void GetQliphothMeltdownInformation(out int qliphothMeltdownLevel, out int qliphothMeltdownGauge, out int qliphothMeltdownMax, out int gaugesBeforeNextMeltdown)
        {
            qliphothMeltdownLevel = CreatureOverloadManager.instance.GetQliphothOverloadLevel();
            qliphothMeltdownGauge = -1;
            qliphothMeltdownMax = CreatureOverloadManager.instance.qliphothOverloadMax;
            //i think potentially the FieldInfo gotten by AccessTools.Field only needs to be gotten once? and then it auto-updates along with the value? test that later.
            FieldInfo gaugeInfo = typeof(CreatureOverloadManager).GetField("qliphothOverloadGauge", BindingFlags.Instance | BindingFlags.NonPublic);
            qliphothMeltdownGauge = (int)gaugeInfo.GetValue(CreatureOverloadManager.instance);
            gaugesBeforeNextMeltdown = qliphothMeltdownMax - qliphothMeltdownGauge;
        }

        public static string GetNextMeltdownInformation()
        {
            FieldInfo ordealInfo = typeof(CreatureOverloadManager).GetField("_nextOrdeal", BindingFlags.Instance | BindingFlags.NonPublic);
            OrdealBase ordeal = (OrdealBase)ordealInfo.GetValue(CreatureOverloadManager.instance);
            if (ordeal != null)
            {
                string typeOfOrdeal = ordeal.OrdealTypeText;
                //string getTypeOfOrdeal = ordeal.GetType().Name;
                //string prefix = "";
                //string suffix = "";
                //if (getTypeOfOrdeal.Contains("Dawn")) prefix = "Dawn";
                //else if (getTypeOfOrdeal.Contains("Noon")) prefix = "Noon";
                //else if (getTypeOfOrdeal.Contains("Dusk")) prefix = "Dusk";
                //else prefix = "Midnight";

                //if (getTypeOfOrdeal.Contains("Machine")) suffix = "Green";
                //else if (getTypeOfOrdeal.Contains("OutterGod")) suffix = "Violet";
                //else if (getTypeOfOrdeal.Contains("Circus")) suffix = "Crimson";
                //else if (getTypeOfOrdeal.Contains("Bug")) suffix = "Amber"; //Ellie reference?
                //else suffix = "Indigo";
                return String.Format("The next meltdown will spawn {0}.", typeOfOrdeal);
            }
            if (_energyController == null) _energyController = GameObject.FindObjectOfType<EnergyController>();
            int overloadIsolateNum = Int32.Parse(_energyController.OverLoadIsolateNumText.text);
            string unitOrUnits = "";
            if (overloadIsolateNum == 1) unitOrUnits = "unit";
            else unitOrUnits = "units";
            return String.Format("The next meltdown will cause {0} containment {1} to become overloaded.", overloadIsolateNum, unitOrUnits);
        }

        public static string GetTrumpetInformation()
        {
            EmergencyLevel trumpetLevel = PlayerModel.emergencyController.currentLevel;
            switch (trumpetLevel)
            {
                case EmergencyLevel.NORMAL:
                    return "";
                case EmergencyLevel.LEVEL1:
                    return "\nThe First Trumpet is playing. Deal with the situation before it becomes a threat.";
                case EmergencyLevel.LEVEL2:
                    return "\nThe Second Trumpet is playing. Deal with the situation before more casualties arise.";
                case EmergencyLevel.LEVEL3:
                    return "\nThe Third Trumpet is playing. Deal with the situation before nothing is left to save.";
                case EmergencyLevel.CHAOS:
                    return "\nThe emergency level is in a state of chaos.";
            }
            return "\nThe trumpet level is broken. Complain to the mod developer about it.";
        }

        public static int GetCoreSuppressionMeltdownProgress(int requiredLevel, int meltdownLevel, int meltdownMax, int meltdownGauge)
        {
            float progressPerLevel = 100 / (requiredLevel - 1);
            int qliphothMeltdownLevelProgress = (int)((float)(meltdownLevel - 1) * progressPerLevel + ((float)progressPerLevel / meltdownMax * meltdownGauge));
            if (qliphothMeltdownLevelProgress > 100) return 100;
            return qliphothMeltdownLevelProgress;
        }
    }
}
