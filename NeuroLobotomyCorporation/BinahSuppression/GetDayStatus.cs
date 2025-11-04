using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.BinahSuppression
{
    public class GetDayStatus
    {
        public static string Command()
        {
            SefiraBossCreatureModel anArbiterModel = null;
            if (SefiraBossManager.Instance.IsAnyBossSessionActivated() && SefiraBossManager.Instance.CurrentBossBase.modelList.Count > 0)
            {
                foreach (SefiraBossCreatureModel bossSefira in SefiraBossManager.Instance.CurrentBossBase.modelList)
                {
                    if (bossSefira.script.GetName().Equals("An Arbiter")) { anArbiterModel = bossSefira; break; }
                }
            }
            if (anArbiterModel == null) return "An Arbiter's model could not be found. Complain to the mod developer about it.";
            int healthPercentRemaining, coreSuppressionProgress;
            string location, defenseInfo;
            CalculateAnArbiterSuppression(anArbiterModel, false, out healthPercentRemaining, out coreSuppressionProgress, out location, out defenseInfo);
            string trumpetDesc = FacilityManagement.GetDayStatus.GetTrumpetInformation();
            return String.Format("An Arbiter awakens." +
                "\n{0}, but she slowly marches through the facility, overloading containment units as she does so." +
                "\nHer current defenses are: {1}." +
                "\nShe currently has {2}% HP Remaining. Core Suppression is {3}% complete." +
                "{4}", location, defenseInfo, healthPercentRemaining.ToString(), coreSuppressionProgress.ToString(), trumpetDesc);
        }

        public static void CalculateAnArbiterSuppression(SefiraBossCreatureModel anArbiterModel, bool isKeter, out int healthPercentRemaining, out int coreSuppressionProgress, out string location, out string defenseInfo)
        {
            healthPercentRemaining = (int)((float)(anArbiterModel.hp / anArbiterModel.metaInfo.maxHp) * 100);
            coreSuppressionProgress = (100 - healthPercentRemaining) / 3;
            location = Helpers.GetUnitModelLocationText(anArbiterModel);
            defenseInfo = Helpers.GetResistancesText(Helpers.GetResistanceTypeValues(anArbiterModel));
            switch (((BinahCoreScript)anArbiterModel.script).Phase)
            {
                case BinahBoss.BinahPhase.P1:
                    break;
                case BinahBoss.BinahPhase.P2:
                    coreSuppressionProgress += 34;
                    break;
                case BinahBoss.BinahPhase.P3:
                    coreSuppressionProgress += 67;
                    break;
            }
            if (isKeter) coreSuppressionProgress = (int)(coreSuppressionProgress * 0.6) + 40; //starts 40% weakened in the Keter version; also makes the An Arbiter suppression seem closer to completion compared to the energy generation
        }
    }
}
