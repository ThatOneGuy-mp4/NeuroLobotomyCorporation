using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.ChesedSuppression
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
            int qliphothMeltdownLevelProgress = FacilityManagement.GetDayStatus.GetCoreSuppressionMeltdownProgress(8, qliphothMeltdownLevel, qliphothMeltdownMax, qliphothMeltdownGauge);
            int coreSuppressionProgress = (energyProgress + qliphothMeltdownLevelProgress) / 2;
            string nextMeltdownDesc = FacilityManagement.GetDayStatus.GetNextMeltdownInformation();
            string trumpetDesc = FacilityManagement.GetDayStatus.GetTrumpetInformation();
            string boostedDamageTypes = GetBoostedDamageInformation();
            status = String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
                "\nThe current Qliphoth Meltdown Level is {3}/8, and the next meltdown will trigger after {4} gauges have been filled (Meltdown Level Progress is {5}%)." +
                "\n{6}" +
                "{7}" +
                "\nCore Suppression is {8}% complete. Continue generating P.E. Boxes and raising the Qliphoth Meltdown Level." +
                "\nAn error with the amount of damage employees receive is detected. {9}", energyCollected, energyRequired, energyProgress, qliphothMeltdownLevel, gaugesBeforeNextMeltdown, qliphothMeltdownLevelProgress, nextMeltdownDesc, trumpetDesc, coreSuppressionProgress, boostedDamageTypes);
            return status;
        }

        public static string GetBoostedDamageInformation()
        {
            List<ChesedBossUI.RWBPUnit> activeBoostedColours = new List<ChesedBossUI.RWBPUnit>();
            foreach(ChesedBossUI.RWBPUnit colorSegment in SefiraBossUI.Instance.chesedBossUI.units)
            {
                if (colorSegment.renderer.sprite == colorSegment.enabled) activeBoostedColours.Add(colorSegment);
            }
            if(activeBoostedColours.Count == 1)
            {
                return String.Format("{0} type damage will be massively increased.", Helpers.GetDamageColorByRwbpType(activeBoostedColours[0].type));
            }
            if(activeBoostedColours.Count == 2)
            {
                return String.Format("{0} and {1} type damage will be massively increased.", Helpers.GetDamageColorByRwbpType(activeBoostedColours[0].type), Helpers.GetDamageColorByRwbpType(activeBoostedColours[1].type));
            }
            if(activeBoostedColours.Count == 3)
            {
                return String.Format("{0}, {1}, and {2} type damage will be massively increased.", Helpers.GetDamageColorByRwbpType(activeBoostedColours[0].type), Helpers.GetDamageColorByRwbpType(activeBoostedColours[1].type), Helpers.GetDamageColorByRwbpType(activeBoostedColours[2].type));
            }
            return "The boosted damage types could not be found. Complain to the mod developer about it.";
        }
    }
}
