using CreatureSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.AbnormalityExtraction
{
    public class ExtractAbnormality
    {
        public const int ABNORMALITY_NAME_INDEX = 1;
        public static string Command(string abnoName)
        {
            CreatureSelectUnit selected = null;
            foreach (CreatureSelectUnit unit in CreatureSelectUI.instance.Units)
            {
                if (unit != null && unit.IdText.text.Equals(abnoName)) { selected = unit; break; }
            }
            if (selected != null)
            {
                selected.OnPointerClick();
            }
            return "";
        }
    }
}
