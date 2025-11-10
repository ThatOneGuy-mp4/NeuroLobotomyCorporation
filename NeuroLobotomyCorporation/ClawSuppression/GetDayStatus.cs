using GameStatusUI;
using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NeuroLobotomyCorporation.ClawSuppression
{
    public class GetDayStatus
    {
        public static string Command()
        {
            string status = "";
            int energyCollected, energyRequired, energyProgress;
            FacilityManagement.GetDayStatus.GetEnergyInformation(out energyCollected, out energyRequired, out energyProgress);
            int qliphothMeltdownLevel, qliphothMeltdownGauge, qliphothMeltdownMax, gaugesBeforeNextMeltdown;
            FacilityManagement.GetDayStatus.GetQliphothMeltdownInformation(out qliphothMeltdownLevel, out qliphothMeltdownGauge, out qliphothMeltdownMax, out gaugesBeforeNextMeltdown);
            string qliphothMeltdownLevelText = qliphothMeltdownLevel.ToString();
            int healthPercentRemaining;
            string clawStatus = CalculateClawSuppression(out healthPercentRemaining);
            int coreSuppressionProgress = (int)(energyProgress * 0.75);
            if(healthPercentRemaining != -1) coreSuppressionProgress += (int)((100 - healthPercentRemaining) * 0.25);
            string nextMeltdownDesc = ClawSuppression.GetDayStatus.GetNextMeltdownInformation();
            string trumpetDesc = FacilityManagement.GetDayStatus.GetTrumpetInformation();
            status = String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
                "\nThe current Qliphoth Meltdown Level is {3}, and the next meltdown will trigger after {4} gauges have been filled." +
                "\n{5}" +
                "{6}" +
                "\n\n{7}\n" +
                "\nCore Suppression is {8}% complete. Continue generating P.E. Boxes and suppressing Ordeals.", energyCollected, energyRequired, energyProgress, qliphothMeltdownLevelText, gaugesBeforeNextMeltdown, nextMeltdownDesc, trumpetDesc, clawStatus, coreSuppressionProgress);
            return status;
        }

        public static string GetNextMeltdownInformation()
        {
            FieldInfo ordealInfo = typeof(CreatureOverloadManager).GetField("_nextOrdeal", BindingFlags.Instance | BindingFlags.NonPublic);
            OrdealBase ordeal = (OrdealBase)ordealInfo.GetValue(CreatureOverloadManager.instance);
            if (ordeal != null)
            {
                string prefix = "";
                switch (ordeal.level)
                {
                    case OrdealLevel.DAWN:
                        prefix = "Dawn";
                        break;
                    case OrdealLevel.NOON:
                        prefix = "Noon";
                        break;
                    case OrdealLevel.DUSK:
                        prefix = "Dusk";
                        break;
                    case OrdealLevel.MIDNIGHT:
                        prefix = "Midnight";
                        break;
                }
                return String.Format("The next meltdown will spawn the {0} of White.", prefix);
            }
            EnergyController _energyController = GameObject.FindObjectOfType<EnergyController>();
            int overloadIsolateNum = Int32.Parse(_energyController.OverLoadIsolateNumText.text);
            return String.Format("The next meltdown will cause {0} containment unit(s) to become overloaded.", overloadIsolateNum);
        }

        public static string CalculateClawSuppression(out int healthPercentRemaining)
        {
            OrdealCreatureModel clawModel = null;
            foreach (OrdealCreatureModel ordealCreature in OrdealManager.instance.GetOrdealCreatureList())
            {
                if (ordealCreature.OrdealBase.OrdealNameText(ordealCreature).Equals("The Claw")) { clawModel = ordealCreature; break; }
            }
            if (clawModel == null) 
            {
                healthPercentRemaining = -1;
                return "The Midnight of White has not arrived yet. Continue raising the Qliphoth Meltdown Level until it does.";
            }
            healthPercentRemaining = (int)((float)(clawModel.hp / clawModel.metaInfo.maxHp) * 100);
            string location = Helpers.GetUnitModelLocationText(clawModel);
            string defenseInfo = Helpers.GetResistancesText(Helpers.GetResistanceTypeValues(clawModel));
            return String.Format("Head, Eye, and... The Claw." +
                "\n{0}" +
                "\n{1}" +
                "\nIt has {2}% HP remaining.", location, defenseInfo, healthPercentRemaining);
        }
    }
}
