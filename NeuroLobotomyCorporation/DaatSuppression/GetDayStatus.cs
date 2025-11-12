using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.DaatSuppression
{
    public class GetDayStatus
    {
        public static string Command()
        {
            int energyCollected, energyRequired, energyProgress;
            FacilityManagement.GetDayStatus.GetEnergyInformation(out energyCollected, out energyRequired, out energyProgress);
            return String.Format("{0}/{1} ({2}%) P.E. Boxes have been collected for the day." +
                "\nWe're so close... You know what to do.", energyCollected, energyRequired, energyProgress);
        }
    }
}
