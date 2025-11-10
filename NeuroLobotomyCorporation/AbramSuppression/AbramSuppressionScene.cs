using KetherBoss;
using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.AbramSuppression
{
    public class AbramSuppressionScene : FacilityManagementScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return AbramSuppression.GetDayStatus.Command();
                case "poke":
                    return GeburaSuppression.Poke.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            return "\"Only regret awaits you tomorrow. It’s a well deserved punishment for someone as trifling as us.\"" +
                "\n\nKeter's Core Suppression, Act III: Regret and Atonement" +
                "\n\nCurtains will close on Act III once one of two conditions have been met:" +
                "\n-1880 P.E. Boxes have been collected, and Qliphoth Meltdown Level 10 has been completed." +
                "\n-The Red Mist is suppressed after she appears at Qliphoth Meltdown Level 5." +
                "\nWarning!! Abram's despair has resonated with the previously suppressed Qlipha of Briah...they begin to stir." +
                "\nAn anomaly with the amount of damage employees receive is detected." +
                "\n\n\"Do not fear the submergence. Accept it. Let us sink together at Carmen’s side.\"";
        }

        /*
         * I include the on-screen text into Neuro's context because I want her to see all the dialogue Vedal does. 
         * However, in this suppression, there is on-screen text that doesn't appear if you fight the Red Mist, 
         * meaning Vedal would not see dialogue Neuro does. And that doesn't make sense now does it?
         * This postfix will override that, and have the on-screen text change regardless of if you're fighting the Red Mist or doing meltdowns.
         */
        public static void AbramDescOverride(ref SefiraBossDescType __result, KetherMiddleBossBase __instance)
        {
            if (__instance.QliphothOverloadLevel < 4) return; 
            SefiraBossCreatureModel redMistModel = null;
            if ((SefiraBossManager.Instance.CurrentBossBase as KetherMiddleBossBase).bossBaseList[2].modelList.Count > 0) //gets GeburahBossBase
            {
                foreach (SefiraBossCreatureModel bossSefira in (SefiraBossManager.Instance.CurrentBossBase as KetherMiddleBossBase).bossBaseList[2].modelList)
                {
                    if (bossSefira.script.GetName().Equals("The Red Mist")) { redMistModel = bossSefira; break; }
                }
            }
            if (redMistModel == null) return;
            SefiraBossDescType redMistResult = SefiraBossDescType.OVERLOAD2;
            switch (((GeburahCoreScript)redMistModel.script).Phase)
            {
                case GeburahBoss.GeburahPhase.P2:
                    redMistResult = SefiraBossDescType.OVERLOAD3;
                    break;
                case GeburahBoss.GeburahPhase.P3:
                    redMistResult = SefiraBossDescType.OVERLOAD4;
                    break;
                case GeburahBoss.GeburahPhase.P4:
                    redMistResult = SefiraBossDescType.OVERLOAD5;
                    break;
            }
            if (__result < redMistResult) __result = redMistResult;
        }
    }
}
