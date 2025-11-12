using KetherBoss;
using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.DaatSuppression
{
    public class DaatSuppressionScene : FacilityManagementScene
    {
        public static bool DaatSuppressed = false;

        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return DaatSuppression.GetDayStatus.Command();
                case "spin":
                    return Spin.Command(Int32.Parse(message[(int)Spin.Parameters.SPIN_AMOUNT]));
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            return "\"Shall we get to work? All we need to do is what we’ve always done.\"" +
                "\n\nCore Suppression...hm, I suppose it's not a suppression at this point, is it? Then..." +
                "\nDa'at Realization has begun on Day 50." +
                "\nNo errors or meltdowns of any kind are detected. All you must do...is manage the Abnormalities, and collect the last 1500 P.E. Boxes we need.";
        }

        //Prefix - save the current next energy threshold; it it differs from the next energy threshold after updating, then a dialogue played, and therefore, increase phase.
        public static void ChangePhaseDaatPrefix(KetherLastBossBase __instance, out float __state)
        {
            FieldInfo conversationEnergyQueueInfo = typeof(KetherLastBossBase).GetField("convEnergyQueue", BindingFlags.Instance | BindingFlags.NonPublic);
            Queue<float> queue = (Queue<float>)conversationEnergyQueueInfo.GetValue(__instance);
            __state = queue.Peek();
        }

        //Postfix
        public static void ChangePhaseDaatPostfix(KetherLastBossBase __instance, float __state)
        {
            FieldInfo conversationEnergyQueueInfo = typeof(KetherLastBossBase).GetField("convEnergyQueue", BindingFlags.Instance | BindingFlags.NonPublic);
            Queue<float> queue = (Queue<float>)conversationEnergyQueueInfo.GetValue(__instance);
            if (queue.Peek() != __state && __state >= 0.1f) NeuroSDKHandler.SendCommand("change_boss_phase");
            //if (__state == 0.7f) Spin.NeuroCameraRotationEvent.CorrectRotation(); //facility begins moving; reset rotation so the emotional impact is not lost
        }

        public static void InformNeuroFinalAyinConversation(string __result, int index)
        {
            string message = String.Format("\"{0}\"", __result);
            if (index == 40) message += "\n\nTHE END";
            NeuroSDKHandler.SendContext(message, true);
        }

        //Postfix - remove the hidden ending. ved is a busy man he doesn't need to spend the time to get 100% abno dissolution to get one scene.
        public static void RemoveHiddenEndingCondition(ref bool __result)
        {
            if (DaatSuppressed) __result = true;
        }
    }
}
