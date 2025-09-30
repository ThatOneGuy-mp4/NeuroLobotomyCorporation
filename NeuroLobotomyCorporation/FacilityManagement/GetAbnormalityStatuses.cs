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
            foreach (CreatureModel abnormality in CreatureManager.instance.GetCreatureList())
            {
                string abnormalityName = abnormality.GetUnitName();
                //string riskLevel = abnormality.GetRiskLevel(); TODO: add the risk level
                string isTool = "";
                if (abnormality.metaInfo.creatureWorkType == CreatureWorkType.KIT) isTool = "Tool ";
                string department = Helpers.GetDepartmentBySefira(abnormality.sefira.sefiraEnum);
                string qliphothCounter = "";
                if (!(abnormality.metaInfo.creatureWorkType == CreatureWorkType.KIT) && !abnormality.observeInfo.GetObserveState(CreatureModel.regionName[0])) //0 is Lob Corp's basic info observe info index
                {
                    qliphothCounter = "?";
                }
                else if (abnormality.metaInfo.creatureWorkType == CreatureWorkType.KIT || (abnormality.observeInfo.GetObserveState(CreatureModel.regionName[0]) && !abnormality.script.HasRoomCounter()))
                {
                    qliphothCounter = "X";
                }
                else
                {
                    qliphothCounter = abnormality.qliphothCounter.ToString();
                }
                string abnormalityStatus = "";
                switch (Helpers.GetAbnormalityWorkingState(abnormality))
                {
                    case Helpers.AbnormalityWorkingState.BREACHING:
                        abnormalityStatus = "! BREACHING !";
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
                } //probably not gonna work right with tool abnos. So considering figuring out how those work perhaps.
                status += String.Format("\n{0}, {1} Department {2}Abnormality: {3} Qliphoth Counter, {4}", abnormalityName, department, isTool, qliphothCounter, abnormalityStatus);
            }
            return status;
        }
    }
}
