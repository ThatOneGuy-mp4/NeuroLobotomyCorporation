using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.AbelSuppression
{
    public class AbelSuppressionScene : FacilityManagementScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return AbelSuppression.GetDayStatus.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            return "\"We had no idea where to go, sailing the ocean without a map or any kind of guidance. Do you think it’ll be any better now? It won’t.\"" +
                "\n\nKeter's Core Suppression, Act II: Fatigue and Waiting" +
                "\n\nCurtains will close on Act II once 1780 P.E. Boxes have been collected, and Qliphoth Meltdown Level 6 has been completed." +
                "\nWarning! Abel's uncertainty has resonated with the previously suppressed Qlipha of Asiyah...they begin to stir." +
                "\nAn error with the work assignment system is detected. Expect further mutations as curtain call approaches." +
                "\n\n\"It is best for everyone if we just turn back to the first act now. Who knows, our unforgivable sin may lighten just a tiny bit if you do.\"";
        }
    }
}
