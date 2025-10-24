using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.NetzachSuppression
{
    public class NetzachSuppressionScene : FacilityManagementScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return NetzachSuppression.GetDayStatus.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            int currentDay = PlayerModel.instance.GetDay() + 1;
            int energyRequired = (int)StageTypeInfo.instnace.GetEnergyNeed(currentDay - 1);
            return string.Format("\"The moment I woke up again here, I met you. You, whom I never wanted to see ever again. ...Why must I wake up and do all the garbage I hate every single day?\"" +
                "\n\nCore Suppression has begun on Day {0}." +
                "\nWarning! Netzach's Qlipha has manifested in the Safety Department. Suppression will be complete once {1} P.E. Boxes have been generated and Qliphoth Meltdown Level 6 has been achieved." +
                "\nAn error with the healing and recovery systems is detected." +
                "\n\n\"No one is actually safe here. You know that the Safety Team is just for show, right?\"" +
                "\n\"Why do you want to continue prolonging these undesired lives? What’d you expect to see at the end of all this?\"", currentDay, energyRequired);
        }
    }
}
