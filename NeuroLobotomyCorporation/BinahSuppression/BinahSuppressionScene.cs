using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.BinahSuppression
{
    public class BinahSuppressionScene : FacilityManagementScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return BinahSuppression.GetDayStatus.Command();
                case "get_overloaded_units":
                    return BinahSuppression.GetOverloadedUnits.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            int currentDay = PlayerModel.instance.GetDay() + 1;
            return String.Format("\"Ah, this brings me back to my senses. ...It seems this body of mine can also use these powers. Interesting, yet tenuous.\"" +
                "\n\nCore Suppression has begun on Day {0}." +
                "\nWarning!!! Binah's Qlipha has manifested as An Arbiter in the Extraction Department. Ignore all other objectives; find a way to lower An Arbiter's defences and suppress by force." +
                "\nAn Arbiter has taken control of Qliphoth Deterrence using Key. Abnormalities will deal no damage to her, and she will begin Qliphoth Overloads as she moves." +
                "\n\n\"Now... can you stop me on your own two feet, with your own power?\"", currentDay);
        }

        //i'm slightly concerned this may trigger more than once. i still don't have evidence of it but i'm suspicious anyways.
        public static void PhaseChangeBinah(BinahCoreScript __instance)
        {
            if (__instance.model.hp <= 1 && __instance.IsInvincible)
            {
                if (SefiraBossManager.Instance.IsKetherBoss(KetherBossType.E3))
                {
                    NeuroSDKHandler.SendCommand("change_boss_phase_alt");
                    return;
                }
                NeuroSDKHandler.SendCommand("change_boss_phase");
            }
        }

        public static void InformNeuroArbiterMeltdowns(int overloadCount, OverloadType type)
        {
            string specialMeltdownInfo = "";
            switch (type)
            {
                case OverloadType.GOLDEN:
                    specialMeltdownInfo = String.Format("\"Rise\"" +
                        "\nAn Arbiter has triggered the Meltdown of Gold, overloading {0} containment units.", overloadCount);
                    break;
                case OverloadType.BLACKFOG:
                    specialMeltdownInfo = String.Format("\"Resonate\"" +
                        "\nAn Arbiter has triggered the Meltdown of Dark Fog, overloading {0} containment units.", overloadCount);
                    break;
                case OverloadType.WAVE:
                    specialMeltdownInfo = String.Format("\"Collapse\"" +
                        "\nAn Arbiter has triggered the Meltdown of Waves, overloading {0} containment units.", overloadCount);
                    break;
                case OverloadType.COLUMN:
                    specialMeltdownInfo = String.Format("\"I’ll open the door if I must... Let us sink here together.\"" +
                        "\nAn Arbiter has triggered the Meltdown of Pillars, overloading {0} containment units.", overloadCount);
                    break;
            }
            NeuroSDKHandler.SendContext(specialMeltdownInfo, true);
        }

        //Postfix
        public static void InformNeuroMeltdownGoldCleared()
        {
            NeuroSDKHandler.SendContext("\"The sandman calls me.\"" +
                "\nThe Meltdown of Gold has been completely cleared. An Arbiter has been stunned.", true);
        }

        //Postfix
        public static void InformNeuroMeltdownGoldNotCleared()
        {
            NeuroSDKHandler.SendContext("\"You failed to bend my back.\"" +
                "\nThe Meltdown of Gold was not cleared. An Arbiter has begun healing.", true);
        }

        //Postfix
        public static void InformNeuroMeltdownDarkFogCleared()
        {
            NeuroSDKHandler.SendContext("\"I’m fading.\"" +
                "\nThe Meltdown of Dark Fog has been completely cleared. An Arbiter has become vulnerable to every damage type.", true);
        }

        //Postfix
        public static void InformNeuroMeltdownDarkFogNotCleared()
        {
            NeuroSDKHandler.SendContext("\"You missed the opportunity.\"" +
                "\nThe Meltdown of Dark Fog was not cleared. An Arbiter remains nearly impervious to damage.", true);
        }

        //Postfix
        public static void InformNeuroMeltdownWavesCleared()
        {
            NeuroSDKHandler.SendContext("\"The waves will rock the shore again.\"" +
                "\nThe Meltdown of Waves has been completely cleared. The creatures tearing through the halls have vanished.", true);
        }

        //Postfix
        public static void InformNeuroMeltdownWavesNotCleared()
        {
            NeuroSDKHandler.SendContext("\"You cannot stop the torrent of this world alone.\"" +
                "\nThe Meltdown of Waves was not cleared. The creatures tearing through the halls continue to flood.", true);
        }

        //Postfix
        public static void InformNeuroMeltdownPillarsCleared()
        {
            NeuroSDKHandler.SendContext("\"Excellent.\"" +
                "\nThe Meltdown of Pillars has been completely cleared. Her pillar attack has been shattered, and her guard along with it.", true);
        }

        //Postfix
        public static void InformNeuroMeltdownPillarsNotCleared()
        {
            NeuroSDKHandler.SendContext("\"Your immaturity is to blame.\"" +
                "\nThe Meltdown of Pillars was not cleared. Her pillar attack has torn through the facility. However, her guard has dropped.", true);
        }
    }
}
