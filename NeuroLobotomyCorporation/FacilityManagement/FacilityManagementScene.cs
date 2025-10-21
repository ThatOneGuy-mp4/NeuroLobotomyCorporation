using GameStatusUI;
using Harmony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class FacilityManagementScene : ActionScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return GetDayStatus.Command();
                case "get_agent_statuses":
                    return GetAgentStatuses.Command();
                case "get_detailed_agent_info":
                    return GetDetailedAgentInfo.Command(message[GetDetailedAgentInfo.DETAILED_AGENT_NAME]);
                case "get_abnormality_statuses":
                    return GetAbnormalityStatuses.Command();
                case "get_detailed_abnormality_info":
                    return GetDetailedAbnormalityInfo.Command(message[(int)GetDetailedAbnormalityInfo.Parameters.ABNORMALITY_NAME], bool.Parse(message[(int)GetDetailedAbnormalityInfo.Parameters.INCLUDE_BASIC_INFO]), bool.Parse(message[(int)GetDetailedAbnormalityInfo.Parameters.INCLUDE_MANAGERIAL_GUIDELINES]), bool.Parse(message[(int)GetDetailedAbnormalityInfo.Parameters.INCLUDE_WORK_SUCCESS_RATES]), bool.Parse(message[(int)GetDetailedAbnormalityInfo.Parameters.INCLUDE_ESCAPE_INFORMATION]));
                case "assign_work":
                    return AssignWork.Command(message[(int)AssignWork.Parameters.AGENT_NAME], message[(int)AssignWork.Parameters.ABNORMALITY_NAME], message[(int)AssignWork.Parameters.WORK_TYPE]);
                case "use_tool":
                    return UseTool.Command(message[(int)UseTool.Parameters.AGENT_NAME], message[(int)UseTool.Parameters.ABNORMALITY_NAME]);
                case "get_suppressable_targets":
                    return GetSuppressableTargets.Command();
                case "suppress_target":
                    return SuppressTarget.Command(message[(int)SuppressTarget.Parameters.AGENT_NAME], message[(int)SuppressTarget.Parameters.TARGET_NAME], message[(int)SuppressTarget.Parameters.LOCATION]);
                case "is_bullet_unlocked":
                    return ShootManagerialBullet.IsBulletUnlocked();
                case "shoot_managerial_bullet":
                    return ShootManagerialBullet.Command(message[(int)ShootManagerialBullet.Parameters.BULLET_TYPE], message[(int)ShootManagerialBullet.Parameters.TARGET_NAME], message[(int)ShootManagerialBullet.Parameters.TARGET_DEPARTMENT]);
            }
            return "Command " + message[COMMAND_INDEX] + " does not exist in scene FacilityManagementScene.";
        }

        private string GetDayStartContext()
        {
            int currentDay = PlayerModel.instance.GetDay() + 1;
            int energyRequired = (int)StageTypeInfo.instnace.GetEnergyNeed(currentDay - 1);
            return String.Format("Day {0} has begun. Manage the Abnormalities until {1} P.E. Boxes have been collected to end the day.", currentDay, energyRequired);
        }
    }
}
