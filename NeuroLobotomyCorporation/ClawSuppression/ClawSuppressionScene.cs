using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.ClawSuppression
{
    public class ClawSuppressionScene : FacilityManagementScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return ClawSuppression.GetDayStatus.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            return "Core Suppression has begun on Day 46." +
                "\nKeter's Core Suppression, Act I: Proving Oneself" +
                "\n\nCurtains will close on Act I once 1680 P.E. Boxes have been collected, and the Midnight of White has been suppressed." +
                "\nA must prove he can handle the trials ahead. From this point on, all Ordeals shall be replaced with White Ordeals, and no department shall be immune to Qliphoth Overloads, save for the Architecture Department." +
                "\nPlease, for both of our sakes...bring this play to its climax.";
        }
    }
}
