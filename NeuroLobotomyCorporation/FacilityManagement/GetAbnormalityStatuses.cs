using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class GetAbnormalityStatuses
    {
        public static string Command()
        {
            string status = "";
            List<Helpers.Entry<SefiraEnum, List<CreatureModel>>> sortedAbnormalities = Helpers.SortAbnormalitiesByDepartment(CreatureManager.instance.GetCreatureList(), false);
            foreach (Helpers.Entry<SefiraEnum, List<CreatureModel>> sortingEntry in sortedAbnormalities)
            {
                status += Helpers.GetDepartmentBySefira(sortingEntry.k) + " Department Abnormalities:\n";
                foreach (CreatureModel abnormality in sortingEntry.v)
                {
                    string abnormalityName = abnormality.GetUnitName();
                    string riskLevel = "";
                    if (abnormality.observeInfo.GetObserveState(CreatureModel.regionName[0]) //0 is Lob Corp's basic info observe info index
                        || abnormality.metaInfo.creatureWorkType == CreatureWorkType.KIT) //the previous doesn't work for tools. this technically lets neuro see tool abnormalities' risk level before they're unlocked but like. who cares they suck anyways. 
                    {
                        riskLevel = abnormality.metaInfo.riskLevelForce;
                    }
                    else
                    {
                        riskLevel = "Unknown";
                    }
                    string qliphothCounterOrTool = "";
                    if(abnormality.metaInfo.creatureWorkType == CreatureWorkType.KIT)
                    {
                        qliphothCounterOrTool = "Tool Abnormality";
                    }
                    else if (!abnormality.observeInfo.GetObserveState(CreatureModel.regionName[0])) 
                    {
                        qliphothCounterOrTool = "? Qliphoth Counter";
                    }
                    else if (abnormality.observeInfo.GetObserveState(CreatureModel.regionName[0]) && !abnormality.script.HasRoomCounter())
                    {
                        qliphothCounterOrTool = "X Qliphoth Counter";
                    }
                    else
                    {
                        qliphothCounterOrTool = abnormality.qliphothCounter.ToString() + " Qliphoth Counter";
                    }
                    string abnormalityStatus = "";
                    switch (Helpers.GetAbnormalityWorkingState(abnormality))
                    {
                        case Helpers.AbnormalityWorkingState.BREACHING:
                            abnormalityStatus = "Breaching";
                            break;
                        case Helpers.AbnormalityWorkingState.WORKING:
                            abnormalityStatus = "In Work";
                            break;
                        case Helpers.AbnormalityWorkingState.IDLE:
                            abnormalityStatus = "Idle";
                            break;
                        case Helpers.AbnormalityWorkingState.COOLDOWN:
                            abnormalityStatus = "On Cooldown";
                            break;
                    }
                    status += String.Format("-{0}: {1} Risk Level, {2}, Currently {3}\n", abnormalityName, riskLevel, qliphothCounterOrTool, abnormalityStatus);
                }
                status += "\n";
            }
            return status;
        }
    }
}
