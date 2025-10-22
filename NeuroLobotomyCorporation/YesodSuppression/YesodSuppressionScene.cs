using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.YesodSuppression
{
    public class YesodSuppressionScene : FacilityManagementScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return YesodSuppression.GetDayStatus.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            int currentDay = PlayerModel.instance.GetDay() + 1;
            int energyRequired = (int)StageTypeInfo.instnace.GetEnergyNeed(currentDay - 1);
            return string.Format("Core Suppression has begun on Day {0}." +
                "\nWarning! Yesod's Qlipha has manifested in the Information Department. Suppression will be complete once {1} P.E. Boxes have been generated and Qliphoth Meltdown Level 6 has been achieved." +
                "\nErrors have been detected in the visual and information retrieval systems.", currentDay, energyRequired);
        }
    }
}
