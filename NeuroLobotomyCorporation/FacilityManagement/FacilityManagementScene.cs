using GameStatusUI;
using Harmony;
using NeuroLobotomyCorporation.BinahSuppression;
using Rabbit;
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
                case "DEBUG_fucking_kills_you":
                    return DEBUGFuckingKillsYou.Command(message[(int)DEBUGFuckingKillsYou.Parameters.GUY_TO_FUCKING_KILL]);
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
                case "get_overloaded_units":
                    return GetOverloadedUnits.Command();
                case "assign_work":
                    return AssignWork.Command(message[(int)AssignWork.Parameters.AGENT_NAME], message[(int)AssignWork.Parameters.ABNORMALITY_NAME], message[(int)AssignWork.Parameters.WORK_TYPE]);
                case "use_tool":
                    return UseTool.Command(message[(int)UseTool.Parameters.AGENT_NAME], message[(int)UseTool.Parameters.ABNORMALITY_NAME]);
                case "get_suppressible_targets":
                    return GetSuppressibleTargets.Command();
                case "suppress_target":
                    return SuppressTarget.Command(message[(int)SuppressTarget.Parameters.AGENT_NAME], message[(int)SuppressTarget.Parameters.TARGET_NAME], message[(int)SuppressTarget.Parameters.LOCATION]);
                case "cancel_action":
                    return CancelAction.Command(message[(int)CancelAction.Parameters.AGENT_NAME]);
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

        //Postfix - modify the message to remove anything that does not make sense as an Angela-to-Neuro conversation.
        public static void InformNeuroAngelaMessage(AngelaMessageState state, string __result)
        {
            switch (state)
            {
                case AngelaMessageState.ENERGY_HALF:
                    if (SefiraBossManager.Instance.IsAnyBossSessionActivated()) return;
                    if(__result.Contains("You cut off the broadcast."))
                    {
                        __result = "As of this moment you have accumulated exactly half of the energy needed. Well, to be technical, there is no such thing as “exactly half” of energy; energy is formless. The phrase “exactly half of the energy” is a contradictory statement, completely without reason. In other words— Ah, the broadcast has been cut.";
                    }
                    NeuroSDKHandler.SendContext(__result, true);
                    break;
                case AngelaMessageState.ENERGY_FULL:
                    string finalMessage = "";
                    if (!SefiraBossManager.Instance.IsAnyBossSessionActivated()) 
                    {
                        if (__result.Contains("perfect fit as manager")) finalMessage += "Look now, the result of your effort is before us. It is exactly as we needed. See, you’re a perfect fit as assistant to the manager of the facility.";
                        else finalMessage += __result + "\n"; 
                    }
                    else finalMessage += "The target amount of energy has been collected. ";
                    finalMessage += "Commencing Refinement… ";
                    NeuroSDKHandler.SendContext(finalMessage, true);
                    break;
            }
        }

        //Postfix
        public static void InformNeuroAgentDied(AgentModel __instance)
        {
            if (!__instance.invincible) NeuroSDKHandler.SendContext(String.Format("{0} has died.", __instance.GetUnitName()), true);
        }

        //Postfix
        public static void InformNeuroAgentPanicked(AgentModel __instance)
        {
            if(!__instance.invincible && !__instance.CannotControll() && !__instance.IsDead()) NeuroSDKHandler.SendContext(String.Format("{0} has been thrown into a panic.", __instance.GetUnitName()), true);
        }

        //Postfix
        public static void InformNeuroAgentBecomesUncontrollable(AgentModel __instance)
        {
            if (!__instance.IsDead()) NeuroSDKHandler.SendContext(String.Format("{0} has become uncontrollable.", __instance.GetUnitName()), true);
        }

        //Prefix - check if they were actually panicked before recovering from panic
        public static void InformNeuroAgentRecoverPanic(AgentModel __instance)
        {
            if (!__instance.IsDead() && __instance.IsPanic()) NeuroSDKHandler.SendContext(String.Format("{0} has recovered from panic.", __instance.GetUnitName()), true);
        }

        //Prefix - check if they were actually uncontrollable before recovering from panic
        public static void InformNeuroAgentBecomesControllable(AgentModel __instance)
        {
            if (!__instance.IsDead() && __instance.GetState() == AgentAIState.CANNOT_CONTROLL) NeuroSDKHandler.SendContext(String.Format("{0} has become controllable again.", __instance.GetUnitName()), true);
        }

        /*
         * There's some scuff here in the context because Lobotomy Corporation is spaghetti code garbage,
         * whereby it is entirely possible for a non-escaping entity to escape by spawning a creature instead of escaping.
         * However, the only way I can think to resolve these weird exceptions, without jeopardizing other Abnormalities' output,
         * is to go through every Abnormality individually and explicitly code the output for these exceptions across all relevant patches.
         * And like, who would...who would spend all that time for something ultimately so small?
         * ...
         * TODO: add all exceptions to the standard context sending
         */

        //Prefix - check if the Abnormality's Qliphoth Counter is 0 before it is updated
        public static void InformNeuroAbnormalityQliphothCounterDroppedToZero(CreatureModel __instance)
        {
            //If the Abnormality can escape, do not inform her of the Qliphoth Counter dropping to 0. That information will be implied by the escape context.
            //(prolly need to add an exception for Warm-hearted Woodsman)
            if (__instance.qliphothCounter == 0 && !__instance.metaInfo.isEscapeAble) NeuroSDKHandler.SendContext(String.Format("{0}'s Qliphoth Counter has dropped to 0.", __instance.script.GetName()), true);
        }

        //Postfix
        public static void InformNeuroAbnormalityEscaped(CreatureModel __instance)
        {
            NeuroSDKHandler.SendContext(String.Format("{0} has breached containment.", __instance.script.GetName()), true);
        }

        //Postfix
        public static void InformNeuroChildSpawned(ChildCreatureModel __instance)
        {
            NeuroSDKHandler.SendContext(String.Format("{0} has created a child entity, {1}.", (__instance as ChildCreatureModel).parent.script.GetName(), __instance.script.GetName()), true);
        }

        //Postfix
        //make sure this doesn't count bosses. you'll probably need to add special exceptions here
        public static void InformNeuroAbnormalitySuppressed(CreatureModel __instance)
        {
            if (__instance is OrdealCreatureModel) return; //i don't want to inform her of every defeated ordeal creature. 
            if (__instance is ChildCreatureModel) { /* i don't want to inform her of every defeated child but then there's il piano or whoever else breaches by spawning a child. figure that out later. */ }
            else NeuroSDKHandler.SendContext(String.Format("{0} has been suppressed.", __instance.script.GetName()), true);
        }

        //Prefix - store the current trumpet level as a state
        public static void InformNeuroPrefixTrumpetLevelChanged(PlayerModel.EmergencyController __instance, out EmergencyLevel __state)
        {
            __state = __instance.currentLevel;
        }

        //Postfix - if the trumpet level has changed compared to before it was set, tell Neuro the trumpet has changed
        public static void InformNeuroPostfixTrumpetLevelChanged(PlayerModel.EmergencyController __instance, EmergencyLevel __state)
        {
            if (__state != __instance.currentLevel)
            {
                bool levelIncrease = ((int)__state < (int)__instance.currentLevel);
                switch (__instance.currentLevel)
                {
                    case EmergencyLevel.NORMAL:
                        NeuroSDKHandler.SendContext("The current situation is no longer deemed Trumpet worthy, and the alarms have been disabled.", true);
                        break;
                    case EmergencyLevel.LEVEL1:
                        if (levelIncrease) NeuroSDKHandler.SendContext("Emergency level has risen to the First Trumpet. Please, deal with the current situation before it becomes a threat.", true);
                        else NeuroSDKHandler.SendContext("Emergency level has been lowered to the First Trumpet. Continue to suppress any lingering threats.", true);
                        break;
                    case EmergencyLevel.LEVEL2:
                        if (levelIncrease) NeuroSDKHandler.SendContext("Emergency level has risen to the Second Trumpet. Please, deal with the current situation before more casualties occur.", true);
                        else NeuroSDKHandler.SendContext("Emergency level has been lowered to the Second Trumpet. Continue reclaiming control of the facility.", true);
                        break;
                    case EmergencyLevel.LEVEL3:
                        NeuroSDKHandler.SendContext("Emergency level has risen to the Third Trumpet. At this point, all hope may be lost...but please, try to salvage what's left, if you can.", true);
                        break;
                    case EmergencyLevel.CHAOS:
                        NeuroSDKHandler.SendContext("The Trumpet Level is in a state of chaos. The mod developer doesn't know what this means, so please complain about this message if you get it so he can figure it out.");
                        break;
                }
            }
        }



        private static bool overloadIsFromMeltdown = false;
        [HarmonyPatch(typeof(CreatureOverloadManager), "ActivateOverload", new Type[] { typeof(int), typeof(OverloadType), typeof(float), typeof(bool), typeof(bool), typeof(bool), typeof(long[]) })]
        public class InformNeuroOverloadActivated
        {
            //Prefix - set the flag for if an overload is from a meltdown so context is not sent in the case it is
            public static void Prefix()
            {
                overloadIsFromMeltdown = true;
            }

            //Postfix - we need to use this overloaded version of ActivateOverload specifically because this one is specific to overloads and does not include ordeals (which will be dealt with separately)
            public static void Postfix(int overloadCount, OverloadType type)
            {
                if (SefiraBossManager.Instance.CheckBossActivation(SefiraEnum.BINAH) || (SefiraBossManager.Instance.IsKetherBoss() && SefiraBossManager.Instance.GetKetherBossType() == KetherBossType.E3))
                {
                    if(type != OverloadType.DEFAULT)
                    {
                        BinahSuppressionScene.InformNeuroArbiterMeltdowns(overloadCount, type);
                        return;
                    }
                }
                if (overloadCount == 0) NeuroSDKHandler.SendContext("The Qliphoth Meltdown Level has been raised, however, no overloads or Ordeals have occurred.");
                else NeuroSDKHandler.SendContext(String.Format("The Qliphoth Meltdown Level has been raised, and {0} containment units have become overloaded as a result.", overloadCount), true);
                overloadIsFromMeltdown = false; //begin sending overload context again
            }
        }

        //Postfix - only send context if the overload is not from a meltdown (e.g., from Apocalypse Bird or Black Fixer)
        public static void InformNeuroOverloadActivatedOutsideOfMeltdown(CreatureModel __instance)
        {
            if (overloadIsFromMeltdown) return;
            NeuroSDKHandler.SendContext(String.Format("{0}'s containment unit has become overloaded.", __instance.script.GetName()), true);
        }

        //Postfix
        public static void InformNeuroOrdealStartedAndEnded(string ordealName, bool isStart, int reward)
        {
            string finalMessage = "";
            string ordealType = LocalizeTextDataModel.instance.GetText(String.Format("ordeal_{0}_type", ordealName));
            if (isStart)
            {
                string ordealFullName = LocalizeTextDataModel.instance.GetText(String.Format("ordeal_{0}_name", ordealName));
                string ordealStartText = LocalizeTextDataModel.instance.GetText(String.Format("ordeal_{0}_start", ordealName));
                finalMessage = String.Format(
                    "'{0}'" +
                    "\n'{1}'" +
                    "\nThe Qliphoth Meltdown Level has been raised, and {2} has spawned as a result.", ordealFullName, ordealStartText, ordealType);
            }
            else
            {
                string ordealEndText = LocalizeTextDataModel.instance.GetText(String.Format("ordeal_{0}_end", ordealName));
                finalMessage = String.Format(
                    "'{0}'" +
                    "\n{1} has been suppressed. {2}% of the energy quota has been filled.", ordealEndText, ordealType, reward);
            }
            NeuroSDKHandler.SendContext(finalMessage, true);
        }

        //Postfix - save the start dialogue for later, send anything else as context
        private static string rabbitStartDialogue = "";
        public static void InformNeuroRabbitsDialogue(RabbitCaptainConversationType type, string __result)
        {
            if(type == RabbitCaptainConversationType.START)
            {
                rabbitStartDialogue = __result;
            }
            else
            {
                string finalMessage = String.Format("\"{0}\"", __result);
                if (type == RabbitCaptainConversationType.ALLDEAD || type == RabbitCaptainConversationType.CLEARED) finalMessage += "\n\nYou may resume work in the departments the Rabbits were deployed to.";
                NeuroSDKHandler.SendContext(finalMessage, true);
            }
        }

        //Prefix - store the current selected departments as state
        public static void InformNeuroPreRabbitsDeployed(RabbitProtocolWindow __instance, out List<SefiraEnum> __state)
        {
            __state = new List<SefiraEnum>();
            FieldInfo selectedInfo = typeof(RabbitProtocolWindow).GetField("currentSelected", BindingFlags.Instance | BindingFlags.NonPublic);
            List<SefiraEnum> currentSelected = (List<SefiraEnum>)selectedInfo.GetValue(__instance);
            foreach(SefiraEnum selected in currentSelected)
            {
                __state.Add(selected);
            }
        }

        //Postfix - currentSelected is cleared by the original method, so a copy of it is made in the prefix
        public static void InformNeuroPostRabbitsDeployed(List<SefiraEnum> __state)
        {
            if (__state.Count <= 0) return;
            string departmentsDeployedIn = "";
            for(int i = 0; i < __state.Count; i++)
            {
                departmentsDeployedIn += Helpers.GetDepartmentBySefira(__state[i]);
                if (i != __state.Count - 1) departmentsDeployedIn += ", ";
            }
            NeuroSDKHandler.SendContext(String.Format("\"{0}\"" +
                "\n\nThe Rabbits have been deployed in the following department(s): {1}." +
                "\nAny entity, friend or foe, will be shot on sight in said departments, and be unable to leave. You may not assign any work in those departments until the Rabbits have left." + //TODO: the second half of this is actually not true currently; make it so.
                "\n{2}% of our energy quota has been deducted from our energy as payment for their services.", rabbitStartDialogue, departmentsDeployedIn, (__state.Count * 25)), true);
        }
    }
}
