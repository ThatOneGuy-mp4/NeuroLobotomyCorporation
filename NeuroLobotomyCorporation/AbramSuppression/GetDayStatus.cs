using KetherBoss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.AbramSuppression
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
            string boostedDamageTypes = ChesedSuppression.GetDayStatus.GetBoostedDamageInformation();
            if (String.IsNullOrEmpty(boostedDamageTypes)) boostedDamageTypes = "Chesed's Qlipha is slumbering...no damage types have been boosted.";
            else boostedDamageTypes = String.Format("An anomaly with the amount of damage employees receive is detected. {0}", boostedDamageTypes);
            string nextMeltdownDesc = ClawSuppression.GetDayStatus.GetNextMeltdownInformation();
            string trumpetDesc = FacilityManagement.GetDayStatus.GetTrumpetInformation();
            if (qliphothMeltdownLevel < 5)
            {
                return String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
                "\nThe current Qliphoth Meltdown Level is {3}, and the next meltdown will trigger after {4} gauges have been filled." +
                "\n{5}" +
                "{6}" +
                "\n{7}" +
                "\nCore Suppression progress cannot be determined until The Red Mist appears. Continue raising the Qliphoth Meltdown Level until then.", energyCollected, energyRequired, energyProgress, qliphothMeltdownLevel, gaugesBeforeNextMeltdown, nextMeltdownDesc, trumpetDesc, boostedDamageTypes);
            }
            int qliphothMeltdownLevelProgress = FacilityManagement.GetDayStatus.GetCoreSuppressionMeltdownProgress(7, (qliphothMeltdownLevel - 4), qliphothMeltdownMax, qliphothMeltdownGauge);
            if (SefiraBossManager.Instance.CurrentBossBase.QliphothOverloadLevel >= 10) qliphothMeltdownLevelProgress = 100;
            int coreSuppressionViaMeltdownProgress = (energyProgress + qliphothMeltdownLevelProgress) / 2;
            SefiraBossCreatureModel redMistModel = null;
            
            if ((SefiraBossManager.Instance.CurrentBossBase as KetherMiddleBossBase).bossBaseList[2].modelList.Count > 0)
            {
                foreach (SefiraBossCreatureModel bossSefira in (SefiraBossManager.Instance.CurrentBossBase as KetherMiddleBossBase).bossBaseList[2].modelList)
                {
                    if (bossSefira.script.GetName().Equals("The Red Mist")) { redMistModel = bossSefira; break; }
                }
            }
            if (redMistModel == null) return "The Red Mist's model could not be found. Complain to the mod developer about it.";
            int healthPercentRemaining, coreSuppressionViaRedMistProgress;
            string location, equippedEGOGear, defenseInfo;
            GeburaSuppression.GetDayStatus.CalculateRedMistSuppression(redMistModel, true, out healthPercentRemaining, out coreSuppressionViaRedMistProgress, out location, out equippedEGOGear, out defenseInfo);
            status = String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
                "\nThe current Qliphoth Meltdown Level is {3}/10, and the next meltdown will trigger after {4} gauges have been filled (Meltdown Level Progress is {5}%)." +
                "\n{6}" +
                "{7}" +
                "\nCore Suppression Progress:" +
                "\n-Through P.E. Box generation and Qliphoth Meltdown Level; {8}% complete." +
                "\n-Through suppression of The Red Mist; {9}% complete (use GetSuppressibleTargets to get more info)." +
                "\n{10}", energyCollected, energyRequired, energyProgress, qliphothMeltdownLevel, gaugesBeforeNextMeltdown, qliphothMeltdownLevelProgress, nextMeltdownDesc, trumpetDesc, coreSuppressionViaMeltdownProgress, coreSuppressionViaRedMistProgress, boostedDamageTypes);
            return status;
        }
    }
}
