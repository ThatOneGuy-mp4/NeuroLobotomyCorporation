using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.AbnormalityExtraction
{
    public class ReextractAbnormalities
    {
        public static string Command()
        {
            CreatureSelectUI.instance.OnClickReExtract();
            return "";
        }
    }
}
