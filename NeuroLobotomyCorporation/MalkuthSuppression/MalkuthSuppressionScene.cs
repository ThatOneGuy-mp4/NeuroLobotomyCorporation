using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.MalkuthSupression
{
    public class MalkuthSuppressionScene : FacilityManagementScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return MalkuthSuppression.GetDayStatus.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            int currentDay = PlayerModel.instance.GetDay() + 1;
            int energyRequired = (int)StageTypeInfo.instnace.GetEnergyNeed(currentDay - 1);
            return string.Format("\"I could have done it. ...I just wanted to help out.\"" +
                "\n\nCore Suppression has begun on Day {0}." +
                "\nWarning! Malkuth's Qlipha has manifested in the Control Department. Suppression will be complete once {1} P.E. Boxes have been generated and Qliphoth Meltdown Level 6 has been achieved." +
                "\nAn error with the work assignment system is detected." +
                "\n\n\"This is what you call a truly uncontrollable situation, manager. ...It's unpredictable, isn't it?\"" +
                "\n\"Everything’s all jumbled up, messed up, flustered... hahaha...", currentDay, energyRequired);
        }
    }
}
