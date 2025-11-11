using KetherBoss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.AdamSuppression
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
            string nextMeltdownDesc = ClawSuppression.GetDayStatus.GetNextMeltdownInformation();
            string trumpetDesc = FacilityManagement.GetDayStatus.GetTrumpetInformation();
            if (qliphothMeltdownLevel < 5)
            {
                return String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
                "\nThe current Qliphoth Meltdown Level is {3}, and the next meltdown will trigger after {4} gauges have been filled." +
                "\n{5}" +
                "{6}" +
                "\nCore Suppression progress cannot be determined until An Arbiter appears. Continue raising the Qliphoth Meltdown Level until then." +
                "\nAn anomaly with the flow of time itself is detected.", energyCollected, energyRequired, energyProgress, qliphothMeltdownLevel, gaugesBeforeNextMeltdown, nextMeltdownDesc, trumpetDesc);
            }
            int qliphothMeltdownLevelProgress = FacilityManagement.GetDayStatus.GetCoreSuppressionMeltdownProgress(7, (qliphothMeltdownLevel - 4), qliphothMeltdownMax, qliphothMeltdownGauge);
            if (SefiraBossManager.Instance.CurrentBossBase.QliphothOverloadLevel >= 10) qliphothMeltdownLevelProgress = 100;
            int coreSuppressionViaMeltdownProgress = (energyProgress + qliphothMeltdownLevelProgress) / 2;
            //check if qliphoth meltdown is >= 8 because arbiter explodes afterwards
            if (qliphothMeltdownLevel > 8)
            {
                return String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
                "\nThe current Qliphoth Meltdown Level is {3}/(Completed 10), and the next meltdown will trigger after {4} gauges have been filled (Meltdown Level Progress is {5}%)." +
                "\n{6}" +
                "{7}" +
                "\nCore Suppression Progress:" +
                "\n-Through P.E. Box generation and Qliphoth Meltdown Level; {8}% complete." +
                "\n-Through suppression of An Arbiter; INCOMPLETABLE" +
                "\nAn anomaly with the flow of time itself is detected.", energyCollected, energyRequired, energyProgress, qliphothMeltdownLevel, gaugesBeforeNextMeltdown, qliphothMeltdownLevelProgress, nextMeltdownDesc, trumpetDesc, coreSuppressionViaMeltdownProgress);

            }
            SefiraBossCreatureModel anArbiterModel = null;
            if ((SefiraBossManager.Instance.CurrentBossBase as KetherLowerBossBase).bossBaseList[1].modelList.Count > 0)
            {
                foreach (SefiraBossCreatureModel bossSefira in (SefiraBossManager.Instance.CurrentBossBase as KetherLowerBossBase).bossBaseList[1].modelList)
                {
                    if (bossSefira.script.GetName().Equals("An Arbiter")) { anArbiterModel = bossSefira; break; }
                }
            }
            if (anArbiterModel == null) return "An Arbiter's model could not be found. Complain to the mod developer about it.";
            int healthPercentRemaining, coreSuppressionViaAnArbiterProgress;
            healthPercentRemaining = (int)((float)(anArbiterModel.hp / anArbiterModel.baseMaxHp) * 100);
            coreSuppressionViaAnArbiterProgress = (100 - healthPercentRemaining) / 3;
            switch (((BinahCoreScript)anArbiterModel.script).Phase)
            {
                case BinahBoss.BinahPhase.P1:
                    break;
                case BinahBoss.BinahPhase.P2:
                    coreSuppressionViaAnArbiterProgress += 34;
                    break;
                case BinahBoss.BinahPhase.P3:
                    coreSuppressionViaAnArbiterProgress += 67;
                    break;
            }
            coreSuppressionViaAnArbiterProgress = (int)(coreSuppressionViaAnArbiterProgress * 0.6 + 40);
            //try doing this manually???? idfk man
            //BinahSuppression.GetDayStatus.CalculateAnArbiterSuppression(anArbiterModel, true, out healthPercentRemaining, out coreSuppressionViaAnArbiterProgress, out location, out defenseInfo);
            status = String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
                "\nThe current Qliphoth Meltdown Level is {3}/(Completed 10), and the next meltdown will trigger after {4} gauges have been filled (Meltdown Level Progress is {5}%)." +
                "\n{6}" +
                "{7}" +
                "\nCore Suppression Progress:" +
                "\n-Through P.E. Box generation and Qliphoth Meltdown Level; {8}% complete." +
                "\n-Through suppression of An Arbiter; {9}% complete (use GetSuppressibleTargets to get more info)." +
                "\nAn anomaly with the flow of time itself is detected.", energyCollected, energyRequired, energyProgress, qliphothMeltdownLevel, gaugesBeforeNextMeltdown, qliphothMeltdownLevelProgress, nextMeltdownDesc, trumpetDesc, coreSuppressionViaMeltdownProgress, coreSuppressionViaAnArbiterProgress);
            return status;
        }
    }
}
