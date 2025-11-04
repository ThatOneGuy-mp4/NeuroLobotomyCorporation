using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.HokmaSuppression
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
            int qliphothMeltdownLevelProgress;
            //Qliphoth Meltdown Level MAX doesn't actually exist in game so a little check needs to be done to ensure output is correct
            FieldInfo qliphothMaxInfo = typeof(ChokhmahBossBase).GetField("_qliphothClear", BindingFlags.Instance | BindingFlags.NonPublic);
            bool qliphothLevelIsMaxed = (bool) qliphothMaxInfo.GetValue(SefiraBossManager.Instance.CurrentBossBase as ChokhmahBossBase);
            if (qliphothLevelIsMaxed)
            {
                qliphothMeltdownLevelProgress = 100;
                qliphothMeltdownLevelText = "MAX";
            }
            else
            {
                qliphothMeltdownLevelProgress = FacilityManagement.GetDayStatus.GetCoreSuppressionMeltdownProgress(11, qliphothMeltdownLevel, qliphothMeltdownMax, qliphothMeltdownGauge);
            }
            int coreSuppressionProgress = (energyProgress + qliphothMeltdownLevelProgress) / 2;
            string nextMeltdownDesc = FacilityManagement.GetDayStatus.GetNextMeltdownInformation();
            string trumpetDesc = FacilityManagement.GetDayStatus.GetTrumpetInformation();
            int agentsToPayPriceOfSilence = (SefiraBossManager.Instance.CurrentBossBase as ChokhmahBossBase).GetTargetAgentCount() + 1;
            status = String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
                "\nThe current Qliphoth Meltdown Level is {3}/MAX [Completed Level 10], and the next meltdown will trigger after {4} gauges have been filled (Meltdown Level Progress is {5}%)." +
                "\n{6}" +
                "{7}" +
                "\nCore Suppression is {8}% complete. Continue generating P.E. Boxes and raising the Qliphoth Meltdown Level." +
                "\nAn anomaly with the flow of time itself has been detected. {9} Agent(s) will pay the price of silence upon pausing, the manager's time has quickened, and yours has slowed.", energyCollected, energyRequired, energyProgress, qliphothMeltdownLevelText, gaugesBeforeNextMeltdown, qliphothMeltdownLevelProgress, nextMeltdownDesc, trumpetDesc, coreSuppressionProgress, agentsToPayPriceOfSilence);
            return status;
        }


    }
}
