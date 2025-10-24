using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.HodSuppression 
{
    public class HodSuppressionScene : FacilityManagementScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return HodSuppression.GetDayStatus.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            int currentDay = PlayerModel.instance.GetDay() + 1;
            int energyRequired = (int)StageTypeInfo.instnace.GetEnergyNeed(currentDay - 1);
            return string.Format("\"I wish everyone would rely on me... I want to help as much as I can here.\"" +
                "\n\nCore Suppression has begun on Day {0}." +
                "\nWarning! Hod's Qlipha has manifested in the Training Department. Suppression will be complete once {1} P.E. Boxes have been generated and Qliphoth Meltdown Level 6 has been achieved." +
                "\nAn error with our employees' statuses is detected. All Agents have reported feeling weakness." +
                "\n\n\"Well, shall we film the corporate educational video together? ...Alright everyone, look here! Smile, say cheese!\"" +
                "\n\"Please make sure to do it carefully, and don’t mess up! Every employee will watch this educational video and refer back to it!\"", currentDay, energyRequired);
        }
    }
}
