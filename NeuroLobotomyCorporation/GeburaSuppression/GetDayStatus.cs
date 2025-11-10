using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.GeburaSuppression
{
    public class GetDayStatus
    {
        public static string Command()
        {
            SefiraBossCreatureModel redMistModel = null;
            if (SefiraBossManager.Instance.IsAnyBossSessionActivated() && SefiraBossManager.Instance.CurrentBossBase.modelList.Count > 0)
            {
                foreach (SefiraBossCreatureModel bossSefira in SefiraBossManager.Instance.CurrentBossBase.modelList)
                {
                    if (bossSefira.script.GetName().Equals("The Red Mist")) { redMistModel = bossSefira; break; }
                }
            }
            if (redMistModel == null) return "The Red Mist's model could not be found. Complain to the mod developer about it.";
            int healthPercentRemaining, coreSuppressionProgress;
            string location, equippedEGOGear, defenseInfo;
            CalculateRedMistSuppression(redMistModel, false, out healthPercentRemaining, out coreSuppressionProgress, out location, out equippedEGOGear, out defenseInfo);
            string trumpetDesc = FacilityManagement.GetDayStatus.GetTrumpetInformation();
            return String.Format("The Red Mist makes her return." +
                "\n{0}, however, be wary of her movement at certain HP thresholds." +
                "\nHer currently equipped E.G.O Gear is {1}. She may unveil other weapons at certain HP thresholds, or during special attacks." +
                "\nHer current defenses are: {2}." +
                "\nShe currently has {3}% HP Remaining. Core Suppression is {4}% complete." +
                "{5}", location, equippedEGOGear, defenseInfo, healthPercentRemaining.ToString(), coreSuppressionProgress.ToString(), trumpetDesc);
        }

        public static void CalculateRedMistSuppression(SefiraBossCreatureModel redMistModel, bool isKeter, out int healthPercentRemaining, out int coreSuppressionProgress, out string location, out string equippedEGOGear, out string defenseInfo)
        {
            healthPercentRemaining = (int)((float)(redMistModel.hp / redMistModel.baseMaxHp) * 100);
            coreSuppressionProgress = (100 - healthPercentRemaining) / 4;
            location = Helpers.GetUnitModelLocationText(redMistModel);
            equippedEGOGear = "";
            defenseInfo = Helpers.GetResistancesText(Helpers.GetResistanceTypeValues(redMistModel));
            switch (((GeburahCoreScript)redMistModel.script).Phase)
            {
                case GeburahBoss.GeburahPhase.P1:
                    equippedEGOGear = "Red Eyes & Penitence";
                    break;
                case GeburahBoss.GeburahPhase.P2:
                    equippedEGOGear = "Mimicry & De Capo";
                    coreSuppressionProgress += 25;
                    break;
                case GeburahBoss.GeburahPhase.P3:
                    equippedEGOGear = "Smile & Justitia";
                    coreSuppressionProgress += 50;
                    break;
                case GeburahBoss.GeburahPhase.P4:
                    equippedEGOGear = "Twilight & an Effloresced E.G.O";
                    coreSuppressionProgress += 75;
                    break;
                default:
                    equippedEGOGear = "Nothing";
                    break;
            }
            if (isKeter) coreSuppressionProgress = (int)(coreSuppressionProgress * 0.6) + 40; //starts 40% weakened in the Keter version; also makes the Red Mist suppression seem closer to completion compared to the energy generation
        }
    }
}
