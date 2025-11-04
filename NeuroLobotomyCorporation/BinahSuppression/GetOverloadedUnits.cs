using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.BinahSuppression
{
    public class GetOverloadedUnits
    {
        public static string Command()
        {
            string status = "";
            CreatureModel[] allAbnormalities = CreatureManager.instance.GetCreatureList();
            List<CreatureModel> overloadedAbnormalities = new List<CreatureModel>();
            foreach (CreatureModel abnormality in allAbnormalities)
            {
                if (abnormality.isOverloaded) overloadedAbnormalities.Add(abnormality);
            }
            if (overloadedAbnormalities.Count == 0) return "There are currently no overloaded Containment Units.";
            List<Helpers.Entry<SefiraEnum, List<CreatureModel>>> sortedAbnormalities = Helpers.SortAbnormalitiesByDepartment(overloadedAbnormalities.ToArray(), false);
            foreach (Helpers.Entry<SefiraEnum, List<CreatureModel>> entry in sortedAbnormalities)
            {
                if (entry.v.Count == 0) continue;
                status += Helpers.GetDepartmentBySefira(entry.k) + " Department Abnormalities:\n";
                foreach (CreatureModel abnormality in entry.v)
                {
                    string typeOfOverload = "";
                    switch (abnormality.overloadType)
                    {
                        case OverloadType.DEFAULT:
                            typeOfOverload = "Non-special Meltdown";
                            break;
                        case OverloadType.GOLDEN:
                            typeOfOverload = "Meltdown of Gold";
                            break;
                        case OverloadType.BLACKFOG:
                            typeOfOverload = "Meltdown of Dark Fog";
                            break;
                        case OverloadType.WAVE:
                            typeOfOverload = "Meltdown of Waves";
                            break;
                        case OverloadType.COLUMN:
                            typeOfOverload = "Meltdown of Pillars";
                            break;
                        default:
                            typeOfOverload = "Meltdown type could not be determined. Complain to the mod developer about it.";
                            break;
                    }
                    status += String.Format("-{0}, {1}\n", abnormality.GetUnitName(), typeOfOverload);
                }
                status += "\n";
            }
            return status;
        }
    }
}
