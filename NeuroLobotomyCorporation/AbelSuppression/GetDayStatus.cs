using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.AbelSuppression
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
            int qliphothMeltdownLevelProgress = FacilityManagement.GetDayStatus.GetCoreSuppressionMeltdownProgress(7, qliphothMeltdownLevel, qliphothMeltdownMax, qliphothMeltdownGauge);
            int coreSuppressionProgress = (energyProgress + qliphothMeltdownLevelProgress) / 2;
            string nextMeltdownDesc = ClawSuppression.GetDayStatus.GetNextMeltdownInformation();
            string trumpetDesc = FacilityManagement.GetDayStatus.GetTrumpetInformation();
            string currentErrorDesc = "";
            switch (qliphothMeltdownLevel)
            {
                case 1:
                case 2:
                    currentErrorDesc = "An error with the work assignment system is detected.";
                    break;
                case 3:
                    currentErrorDesc = "Errors with the work assignment, information, and camera systems are detected.";
                    break;
                case 4: 
                    currentErrorDesc = "Errors with the work assignment system, information and camera systems, and our employees' statuses are detected.";
                    break;
                default:
                    currentErrorDesc = "Errors with the work assignment system, work cancellation system, information and camera systems, our employees' statuses, and the healing and recovery systems are detected.";
                    break;
            }
            status = String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
                "\nThe current Qliphoth Meltdown Level is {3}/(Completed 6), and the next meltdown will trigger after {4} gauges have been filled (Meltdown Level Progress is {5}%)." +
                "\n{6}" +
                "{7}" +
                "\nCore Suppression is {8}% complete. Continue generating P.E. Boxes and raising the Qliphoth Meltdown Level." +
                "\n{9}", energyCollected, energyRequired, energyProgress, qliphothMeltdownLevel, gaugesBeforeNextMeltdown, qliphothMeltdownLevelProgress, nextMeltdownDesc, trumpetDesc, coreSuppressionProgress, currentErrorDesc);
            return status;
        }
    }
}
