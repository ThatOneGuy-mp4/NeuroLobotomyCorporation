using Customizing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.DayPreparation
{
    public class Patches
    {
        //Postfix - check what stage of the research phase we're at when the game does
        public static void ResearchCheck()
        {
            ResearchPhaseProgress();
        }

        //Postfix - perform actions based on the stage of the research phase
        public static void ResearchPhaseProgress()
        {
            if (!DeployUI.instance.researchWindow.rootObject.activeSelf) //if no research is happening, and a core suppression is avaliable, give Neuro the ability to start it
            {
                if (CheckCoreSuppressionIsAvaliable())
                {
                    string finalMessage = "enable_activate_core_suppression";
                    foreach(string sephirah in avaliableSuppressions)
                    {
                        finalMessage += "|" + sephirah;
                    }
                    NeuroSDKHandler.SendCommand(finalMessage);
                }
            }
            else if (!DeployUI.instance.researchWindow.sefiraBossButton.activeSelf) //if research is happening, and it's not a boss reward, give Neuro the ability to choose research (maybe)
            {
                //TODO: give neuro ability to choose research, maybe
            }
        }

        private static List<string> avaliableSuppressions = new List<string>();
        public static bool CheckCoreSuppressionIsAvaliable()
        {
            avaliableSuppressions = new List<string>();
            foreach(Sefira sefira in SefiraManager.instance.GetOpendSefiraList())
            {
                if (sefira.sefiraEnum == SefiraEnum.KETHER || sefira.sefiraEnum == SefiraEnum.DAAT) continue;
                if (SefiraBossManager.Instance.IsBossReady(sefira.sefiraEnum) && SefiraBossManager.Instance.IsBossStartable(sefira.sefiraEnum)) avaliableSuppressions.Add(SefiraEnumToName(sefira.sefiraEnum));
            }
            return avaliableSuppressions.Count > 0;
        }

        //Postfix - enable/disable Neuro's ability to start core suppressions based on if any are active
        //she is able to start them, but not disable them. this should railroad ved into doing them because Neuro will keep pressing the start suppression button every day.
        public static void CoreSuppressionEnabledDisabled(SefiraPanel __instance)
        {
            if(SefiraBossManager.Instance.CurrentActivatedSefira == SefiraEnum.DUMMY)
            {
                NeuroSDKHandler.SendContext("Core Suppression has been deactivated.", true);
                NeuroSDKHandler.SendCommand("enable_activate_core_suppression");
                return;
            }
            NeuroSDKHandler.SendCommand("disable_activate_core_suppression");
            NeuroSDKHandler.SendContext(String.Format("{0}'s Core Suppression has been enabled...once day preparation has been finished, suppression will begin.", SefiraEnumToName(__instance.Sefira.sefiraEnum)), false);
        }

        //Postfix - give Neuro the information on the boss reward
        public static void CoreSuppressionRewardContext(SefiraEnum sefiraEnum)
        {
            string finalRewardMessage = String.Format("{0} Synchronization Completed" +
                "\nREWARD:\n\n", SefiraEnumToName(sefiraEnum));
            finalRewardMessage += GetSefiraSpecificCoreSuppressionReward(sefiraEnum);
            finalRewardMessage += "\nThe department in which the Suppression took place will no longer be influenced by Qliphoth Meltdowns.";
            NeuroSDKHandler.SendContext(finalRewardMessage, true);
        }

        //could probably get the info from the panel itself but it's easier to hardcode it so
        private static string GetSefiraSpecificCoreSuppressionReward(SefiraEnum sefiraEnum)
        {
            switch (sefiraEnum)
            {
                case SefiraEnum.MALKUT:
                    return "Higher LOB payout for each completed workday." +
                        "\nAll employees have a higher movement speed.";
                case SefiraEnum.YESOD:
                    return "25% increase in Unique PE-BOX gains from Abnormalities.";
                case SefiraEnum.HOD:
                    return "All newly contracted employees will have their stats at Level 3.";
                case SefiraEnum.NETZACH:
                    return "The regenerator will heal all employees anywhere in the department. Those in the hallways will be healed with 50% efficiency.";
                case SefiraEnum.TIPERERTH1:
                case SefiraEnum.TIPERERTH2:
                    return "The base number of managerial bullets is increased by 30% and the PALE Shield Bullet is unlocked.";
                case SefiraEnum.GEBURAH:
                    return "The maximum number of E.G.O that can be acquired from each Abnormality is increased by 1, up to a maximum of 5.";
                case SefiraEnum.CHESED:
                    return "There will be a 25% chance for an employee to recover before they panic or die. This recovery is limited to once a day for each employee.";
                case SefiraEnum.BINAH:
                    return "E.G.O will no longer be lost or destroyed when the wielder dies.";
                case SefiraEnum.CHOKHMAH:
                    return "The maximum limit upon all employee statistics is now elevated to 130." +
                        "\nWhen contracting a new employee, their stats can now be fortified to 130.";
                default:
                    return "The reward could not be found. Complain to the mod developer.";
            }
        }

        private static string SefiraEnumToName(SefiraEnum sefiraEnum)
        {
            switch (sefiraEnum)
            {
                case SefiraEnum.MALKUT:
                    return "Malkuth";
                case SefiraEnum.YESOD:
                    return "Yesod";
                case SefiraEnum.HOD:
                    return "Hod";
                case SefiraEnum.NETZACH:
                    return "Netzach";
                case SefiraEnum.TIPERERTH1:
                    return "Tiphereth";
                case SefiraEnum.TIPERERTH2:
                    return "";
                case SefiraEnum.GEBURAH:
                    return "Gebura";
                case SefiraEnum.CHESED:
                    return "Chesed";
                case SefiraEnum.BINAH:
                    return "Binah";
                case SefiraEnum.CHOKHMAH:
                    return "Hokma";
                case SefiraEnum.KETHER:
                case SefiraEnum.DAAT:
                case SefiraEnum.DUMMY:
                default:
                    return "";
            }
        }

        public static SefiraEnum SefiraNameToEnum(string name)
        {
            switch (name)
            {
                case "Malkuth":
                    return SefiraEnum.MALKUT;
                case "Yesod":
                    return SefiraEnum.YESOD;
                case "Hod":
                    return SefiraEnum.HOD;
                case "Netzach":
                    return SefiraEnum.NETZACH;
                case "Tiphereth":
                    return SefiraEnum.TIPERERTH1;
                case "Gebura":
                    return SefiraEnum.GEBURAH;
                case "Chesed":
                    return SefiraEnum.CHESED;
                case "Binah":
                    return SefiraEnum.BINAH;
                case "Hokma":
                    return SefiraEnum.CHOKHMAH;
                default:
                    return SefiraEnum.DUMMY;
            }
        }

        //Postfix - give Neuro the ability to customize Agents
        public static void EnableCustomizeAgent()
        {
            NeuroSDKHandler.SendCommand("enable_customize_agent");
        }

        //Postfix - remove Neuro's ability to customize Agents
        public static void DisableCustomizeAgent()
        {
            //if (__instance.rootObject.activeSelf) return;
            NeuroSDKHandler.SendCommand("disable_customize_agent");
        }
    }
}
