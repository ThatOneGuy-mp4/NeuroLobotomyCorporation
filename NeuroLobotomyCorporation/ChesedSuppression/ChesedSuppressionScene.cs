using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.ChesedSuppression
{
    public class ChesedSuppressionScene : FacilityManagementScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_boosted_damage":
                    return GetDayStatus.GetBoostedDamageInformation();
                case "get_day_status":
                    return ChesedSuppression.GetDayStatus.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            int currentDay = PlayerModel.instance.GetDay() + 1;
            int energyRequired = (int)StageTypeInfo.instnace.GetEnergyNeed(currentDay - 1);
            string boostedDamage = ChesedSuppression.GetDayStatus.GetBoostedDamageInformation();
            return string.Format("\"Rain is falling; this rain, the tears of all the employees. The downpour will never stop. ...My my, I’m feeling very moody today~\"" +
                "\n\nCore Suppression has begun on Day {0}." +
                "\nWarning!! Chesed's Qlipha has manifested in the Welfare Department. Suppression will be complete once {1} P.E. Boxes have been generated and Qliphoth Meltdown Level 8 has been achieved." +
                "\nAn anomaly with the amount of damage employees receive is detected. {2}" +
                "\n\n\"Let my employees rest in peace, won’t you?\"" +
                "\n\"Feel this vast pain...\"", currentDay, energyRequired, boostedDamage);
        }
    }
}
