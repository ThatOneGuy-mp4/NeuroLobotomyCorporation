using KetherBoss;
using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.AdamSuppression
{

    //i don't understand half of the shit happening in this battle and it's hard to bug test when all your employees die if you stop and pause.
    //the game crashed a lot; i think i fixed all the crashes; i don't really know. i sure hope i did because i swear to ayin i don't want to see day 49 ever again. it sucks even when you're cheating.
    public class AdamSuppressionScene : FacilityManagementScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return AdamSuppression.GetDayStatus.Command();
                case "get_overloaded_units":
                    return BinahSuppression.GetOverloadedUnits.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            return "\"They will perish in torment if a savior does not come to them, just as mankind was rescued from the Great Flood a long time ago thanks to the man who built the Ark...\"" +
                "\n\nKeter's Core Suppression, Act IV: Freedom and Salvation" +
                "\n\nCurtains will close on Act IV once one of two conditions have been met:" +
                "\n-2000 P.E. Boxes have been collected, and Qliphoth Meltdown Level MAX has been reached (Level 10 Completed)." +
                "\n-An Arbiter is suppressed after she appears at Qliphoth Meltdown Level 5." +
                "\nWarning!!! Adam's insanity has resonated with the previously suppressed Qlipha of Atziluth...they begin to stir." +
                "\nAn anomaly with the flow of time itself is detected." +
                "\n\n\"...And thus, I have revealed myself. I, who shant cling onto the mere past, the regrets, or the trifling memories.\"";
        }

        /*
         * I include the on-screen text into Neuro's context because I want her to see all the dialogue Vedal does. 
         * However, in this suppression, there is on-screen text that doesn't appear if you fight the Red Mist, 
         * meaning Vedal would not see dialogue Neuro does. And that doesn't make sense now does it?
         * This postfix will override that, and have the on-screen text change regardless of if you're fighting An Arbiter or doing meltdowns.
         * It also makes the level 5 meltdown text occur earlier to, again, better line up with the dialogue Neuro is receiving.
         */
        public static void AdamDescOverride(ref SefiraBossDescType __result, KetherLowerBossBase __instance)
        {
            switch (__instance.QliphothOverloadLevel)
            {
                case 0:
                case 1:
                    __result = SefiraBossDescType.OVERLOAD1;
                    break;
                case 2:
                case 3:
                    __result = SefiraBossDescType.OVERLOAD2;
                    break;
                case 4:
                case 5:
                case 6:
                    __result = SefiraBossDescType.OVERLOAD3;
                    break;
                case 7:
                case 8:
                    __result = SefiraBossDescType.OVERLOAD4;
                    break;
                default:
                    __result = SefiraBossDescType.OVERLOAD5;
                    break;
            }
            SefiraBossCreatureModel anArbiterModel = null;
            if ((SefiraBossManager.Instance.CurrentBossBase as KetherLowerBossBase).bossBaseList[1].modelList.Count > 0) //gets BinahBossBase
            {
                foreach (SefiraBossCreatureModel bossSefira in (SefiraBossManager.Instance.CurrentBossBase as KetherLowerBossBase).bossBaseList[1].modelList)
                {
                    if (bossSefira.script.GetName().Equals("An Arbiter")) { anArbiterModel = bossSefira; break; }
                }
            }
            if (anArbiterModel == null) return;
            SefiraBossDescType anArbiterResult = SefiraBossDescType.OVERLOAD3;
            switch (((BinahCoreScript)anArbiterModel.script).Phase)
            {
                case BinahBoss.BinahPhase.P2:
                    anArbiterResult = SefiraBossDescType.OVERLOAD4;
                    break;
                case BinahBoss.BinahPhase.P3:
                    anArbiterResult = SefiraBossDescType.OVERLOAD5;
                    break;
            }
            if (__result < anArbiterResult) __result = anArbiterResult;
        }
    }
}
