using GameStatusUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NeuroLobotomyCorporation.YesodSuppression
{
    public class GetDayStatus
    {
        private static EnergyController _energyController = null;
        public static string Command()
        {
            string status = "";
            int energyCollected = (int)EnergyModel.instance.GetEnergy();
            int energyRequired = (int)StageTypeInfo.instnace.GetEnergyNeed(PlayerModel.instance.GetDay());
            int energyProgress = (int)((float)energyCollected / energyRequired * 100);
            int qliphothMeltdownLevel = CreatureOverloadManager.instance.GetQliphothOverloadLevel();
            int qliphothMeltdownGauge = -1;
            int qliphothMeltdownMax = CreatureOverloadManager.instance.qliphothOverloadMax;
            try
            {
                //i think potentially the FieldInfo gotten by AccessTools.Field only needs to be gotten once? and then it auto-updates along with the value? test that later.
                FieldInfo gaugeInfo = typeof(CreatureOverloadManager).GetField("qliphothOverloadGauge", BindingFlags.Instance | BindingFlags.NonPublic);
                qliphothMeltdownGauge = (int)gaugeInfo.GetValue(CreatureOverloadManager.instance);
            }
            catch (Exception e)
            {

            }
            int gaugesBeforeNextMeltdown = qliphothMeltdownMax - qliphothMeltdownGauge;
            int qliphothMeltdownLevelProgress = (int)((float)(qliphothMeltdownLevel - 1) * 20 + ((float)20 / qliphothMeltdownMax * qliphothMeltdownGauge));
            if (qliphothMeltdownLevelProgress > 100) qliphothMeltdownLevelProgress = 100;
            int coreSuppressionProgress = (energyProgress + qliphothMeltdownLevelProgress) / 2;
            string ordealType = "";
            int overloadIsolateNum = -1;
            try
            {
                FieldInfo ordealInfo = typeof(CreatureOverloadManager).GetField("_nextOrdeal", BindingFlags.Instance | BindingFlags.NonPublic);
                OrdealBase ordeal = (OrdealBase)ordealInfo.GetValue(CreatureOverloadManager.instance);
                if (ordeal != null)
                {
                    string getTypeOfOrdeal = ordeal.GetType().Name;
                    switch (getTypeOfOrdeal) //probably a better way to do this that's still fast. also this doesn't account for white ordeals since those all trigger in one class.
                    {
                        case "MachineDawnOrdeal":
                            ordealType = "Dawn of Green";
                            break;
                        case "MachineNoonOrdeal":
                            ordealType = "Noon of Green";
                            break;
                        case "MachineDuskOrdeal":
                            ordealType = "Dusk of Green";
                            break;
                        case "MachineMidnightOrdeal":
                            ordealType = "Midnight of Green";
                            break;
                        case "OutterGodDawnOrdeal":
                            ordealType = "Dawn of Violet";
                            break;
                        case "OutterGodNoonOrdeal":
                            ordealType = "Noon of Violet";
                            break;
                        case "OutterGodMidnightOrdeal":
                            ordealType = "Midnight of Violet";
                            break;
                        case "CircusDawnOrdeal":
                            ordealType = "Dawn of Crimson";
                            break;
                        case "CircusNoonOrdeal":
                            ordealType = "Noon of Crimson";
                            break;
                        case "CircusDuskOrdeal":
                            ordealType = "Dusk of Crimson";
                            break;
                        case "BugDawnOrdeal":
                            ordealType = "Dawn of Amber";
                            break;
                        case "BugDuskOrdeal":
                            ordealType = "Dusk of Amber";
                            break;
                        case "BugMidnightOrdeal":
                            ordealType = "Midnight of Amber";
                            break;
                        case "ScavengerNoonOrdeal":
                            ordealType = "Noon of Indigo";
                            break;
                    }
                }
                else
                {
                    if (_energyController == null) _energyController = GameObject.FindObjectOfType<EnergyController>();
                    overloadIsolateNum = Int32.Parse(_energyController.OverLoadIsolateNumText.text);
                }
            }
            catch (Exception e)
            {

            }
            EmergencyLevel trumpetLevel = PlayerModel.emergencyController.currentLevel;
            string nextMeltdownDesc = "";
            if (overloadIsolateNum == -1)
            {
                nextMeltdownDesc = String.Format("The next meltdown will spawn the {0}.", ordealType);
            }
            else
            {
                string unitOrUnits = "";
                if (overloadIsolateNum == 1) unitOrUnits = "unit";
                else unitOrUnits = "units";
                nextMeltdownDesc = String.Format("The next meltdown will cause {0} containment {1} to become overloaded.", overloadIsolateNum, unitOrUnits);
            }
            string trumpetDesc = "";
            switch (trumpetLevel)
            {
                case EmergencyLevel.NORMAL:
                    break;
                case EmergencyLevel.LEVEL1:
                    trumpetDesc = "\nThe First Trumpet is playing. Deal with the situation before it becomes a threat.";
                    break;
                case EmergencyLevel.LEVEL2:
                    trumpetDesc = "\nThe Second Trumpet is playing. Deal with the situation before more casualties arise.";
                    break;
                case EmergencyLevel.LEVEL3:
                    trumpetDesc = "\nThe Third Trumpet is playing. Deal with the situation before nothing is left to save.";
                    break;
                case EmergencyLevel.CHAOS:
                    trumpetDesc = "\nThe emergency level is in a state of chaos.";
                    break;
                default:
                    trumpetDesc = "\nThe trumpet level is broken. Complain to the mod developer about it.";
                    break;
            }
            status = String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
                "\nThe current Qliphoth Meltdown Level is {3}/6, and the next meltdown will trigger after {4} gauges have been filled (Meltdown Level Progress is {5})." +
                "\n{6}" +
                "{7}" +
                "\nCore Suppression is {8}% complete. Continue generating P.E. Boxes and raising the Qliphoth Meltdown Level." +
                "\nAn error has been detected in the visual and information retrieval systems.", energyCollected, energyRequired, energyProgress, qliphothMeltdownLevel, gaugesBeforeNextMeltdown, qliphothMeltdownLevelProgress, nextMeltdownDesc, trumpetDesc, coreSuppressionProgress);
            return status;
        }
    }
}
