using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.TipherethSuppression
{
    public class TipherethSuppressionScene : FacilityManagementScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return TipherethSuppression.GetDayStatus.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            int currentDay = PlayerModel.instance.GetDay() + 1;
            return string.Format("\"This song, Tiphereth’s dirge, it’s for him... Tiphereth. ...I hope this performance and song will appease our souls.\"" +
                "\n\nCore Suppression has begun on Day {0}." +
                "\nWarning! Tiphereth's Qlipha has manifested in the Central Command Department, atop a pile of Tiphereth's broken cores. Ignore P.E. Box generation; Suppression will be complete once Qliphoth Meltdown Level 10 has been achieved." +
                "\nMass Qliphoth Meltdowns are detected. Department immunity has been bypassed." +
                "\n\n\"We are two, but one. Do you know what this means?\"" +
                "\n\"Every Containment Unit is under my reign.\"", currentDay);
        }
    }
}
