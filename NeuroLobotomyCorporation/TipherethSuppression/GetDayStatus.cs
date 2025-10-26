using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.TipherethSuppression
{
    public class GetDayStatus
    {
        public static string Command()
        {
            string status = "";
            int qliphothMeltdownLevel, qliphothMeltdownGauge, qliphothMeltdownMax, gaugesBeforeNextMeltdown;
            FacilityManagement.GetDayStatus.GetQliphothMeltdownInformation(out qliphothMeltdownLevel, out qliphothMeltdownGauge, out qliphothMeltdownMax, out gaugesBeforeNextMeltdown);
            int qliphothMeltdownLevelProgress = FacilityManagement.GetDayStatus.GetCoreSuppressionMeltdownProgress(10, qliphothMeltdownLevel, qliphothMeltdownMax, qliphothMeltdownGauge);
            string nextMeltdownDesc = FacilityManagement.GetDayStatus.GetNextMeltdownInformation();
            string trumpetDesc = FacilityManagement.GetDayStatus.GetTrumpetInformation();
            status = String.Format("The current Qliphoth Meltdown Level is {0}/10, and the next meltdown will trigger after {1} gauges have been filled." +
                "\n{2}" +
                "{3}" +
                "\nCore Suppression is {4}% complete. Ignore P.E. Box generation; continue raising the Qliphoth Meltdown Level." +
                "\nMass Qliphoth Meltdowns are detected. Department immunity has been bypassed", qliphothMeltdownLevel, gaugesBeforeNextMeltdown, nextMeltdownDesc, trumpetDesc, qliphothMeltdownLevelProgress);
            return status;
        }
    }
}
