using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class GetOverloadedUnits
    {
        public static string Command()
        {
            string status = "";
            CreatureModel[] allAbnormalities = CreatureManager.instance.GetCreatureList();
            List<CreatureModel> overloadedAbnormalities = new List<CreatureModel>();
            foreach(CreatureModel abnormality in allAbnormalities)
            {
                if(abnormality.isOverloaded)overloadedAbnormalities.Add(abnormality);
            }
            if (overloadedAbnormalities.Count == 0) return "There are currently no overloaded Containment Units.";
            List<Helpers.Entry<SefiraEnum, List<CreatureModel>>> sortedAbnormalities = Helpers.SortAbnormalitiesByDepartment(overloadedAbnormalities.ToArray(), false);
            foreach(Helpers.Entry<SefiraEnum, List<CreatureModel>> entry in sortedAbnormalities)
            {
                if (entry.v.Count == 0) continue;
                status += Helpers.GetDepartmentBySefira(entry.k) + " Department Abnormalities:\n";
                foreach(CreatureModel abnormality in entry.v)
                {
                    status += String.Format("-{0}\n", abnormality.GetUnitName());
                }
                status += "\n";
            }
            return status;
        }
    }
}
