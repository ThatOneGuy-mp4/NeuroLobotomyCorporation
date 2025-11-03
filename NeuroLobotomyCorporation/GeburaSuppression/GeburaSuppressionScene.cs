using GeburahBoss;
using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.GeburaSuppression
{
    public class GeburaSuppressionScene : FacilityManagementScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return GeburaSuppression.GetDayStatus.Command();
                case "poke":
                    return Poke.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            int currentDay = PlayerModel.instance.GetDay() + 1;
            return String.Format("\"I’m back; the Red Mist has walked out from a sea of pain. ...I’m no longer weak like I used to be; I can replace any body part even if it gets cut off, and I can be repaired even if I’m broken.\"" +
                "\n\nCore Suppression has begun on Day {0}." +
                "\nWarning!! Gebura's Qlipha has manifested as The Red Mist in the Disciplinary Department. Ignore all other objectives; The Red Mist must be immediately suppressed by force." +
                "\nThe Red Mist has the E.G.O Weapons Red Eyes & Penitence equipped. Expect her to reveal more gear as the suppression progresses." +
                "\n\n\"I’ll destroy a shoddy place like this with my own hands. ...What’s left for me, the one who failed to protect them?\"" +
                "\n\"Do you really think feeble cowards like you can stop me?\"", currentDay);
        }

        //i'm slightly concerned this may trigger more than once. i don't have evidence of it but i'm suspicious anyways.
        public static void PhaseChangeGebura(GeburahCoreScript __instance)
        {
            if (__instance.model.hp <= 1 && __instance.IsInvincible)
            {
                NeuroSDKHandler.SendCommand("change_boss_phase");
                if (Poke.RedMistRagebait.Instance != null) Poke.RedMistRagebait.Instance.ResetPhase();
            }
        }

        /*
         * The Red Mist has a lot of attacks. It's a useless endeavor to inform Neuro of all of them, 
         * especially since she can't maneuver Agents precisely enough to dodge, so to not clog up her context I've only given her the most important.
         * (i.e., phase and mid-phase transitions, and also the final phase's attacks.)
         * All of these are done with Harmony Patches. See Harmony_Patch to see which methods they're patched on.
         */
        //Gold Rush (P1 & P3)
        //(also; give Neuro the poke ability the first time this attack has happened. just so vedal has to actually play the fight a bit before Neuro can tomfool her.)
        public static void GoldRushTeleport(MovementInfo info)
        {
            if (info._isLast)
            {
                NeuroSDKHandler.SendContext(String.Format("\"The Road of Gold opens\"" +
                    "\n\nThe Red Mist has begun charging through several passages with Gold Rush, ending up in the {0} Department.", Helpers.GetDepartmentBySefira(info.end.GetAttachedPassage().GetSefiraEnum())), true);
            }
            if (!Poke.GivePokeStarted)
            {
                NeuroSDKHandler.SendCommand("give_poke");
                Poke.GivePokeStarted = true;
            }
        }

        //Throw De Capo (P2)
        public static void DecapoThrow(DacapoThrow __instance)
        {
            FieldInfo targetSefiraInfo = typeof(DacapoThrow).GetField("targetSefira", BindingFlags.Instance | BindingFlags.NonPublic);
            Sefira target = (Sefira)targetSefiraInfo.GetValue(__instance);
            if(target != null)
            {
                NeuroSDKHandler.SendContext(String.Format("\"Legato\"" +
                    "\n\nThe Red Mist has thrown De Capo to the {0} Department's Main Room...", Helpers.GetDepartmentBySefira(target.sefiraEnum)), true);
            }
        }

        //Leap to De Capo with Mimicry (P2)
        public static void MimicryLeap(DacapoThrow __instance)
        {
            FieldInfo targetSefiraInfo = typeof(DacapoThrow).GetField("targetSefira", BindingFlags.Instance | BindingFlags.NonPublic);
            Sefira target = (Sefira)targetSefiraInfo.GetValue(__instance);
            if (target != null)
            {
                NeuroSDKHandler.SendContext(String.Format("\"Let’s do this, partner... Only bloody mist remains\"" +
                    "\n\nThe Red Mist has lept to the {0} Department's Main Room.", Helpers.GetDepartmentBySefira(target.sefiraEnum)), true);
            }
        }

        //Chase Target (P4)
        public static void StartTwilightChase(GeburahAction __result)
        {
            if (__result is ChaseAction)
            {
                FieldInfo targetAgentInfo = typeof(ChaseAction).GetField("targetAgent", BindingFlags.Instance | BindingFlags.NonPublic);
                AgentModel target = (AgentModel)targetAgentInfo.GetValue(__result as ChaseAction);
                NeuroSDKHandler.SendContext(String.Format("\"Beat it, coward... Don’t try and stop me; You’re weak\"" +
                    "\n\nThe Red Mist is hunting down {0}, and will cut down anyone in her way with Twilight.", target.GetUnitName()), true);
            }
        }

        //Reached Target (P4)
        public static void TwilightTargetReached(ChaseAction __instance)
        {
            FieldInfo targetAgentInfo = typeof(ChaseAction).GetField("targetAgent", BindingFlags.Instance | BindingFlags.NonPublic);
            AgentModel target = (AgentModel)targetAgentInfo.GetValue(__instance);
            if(target != null)
            {
                NeuroSDKHandler.SendContext(String.Format("\"Be torn apart before my eyes\"" +
                    "\n\nThe Red Mist has reached {0}.", target.GetUnitName()), true);
            }
        }

        //Gold Rush (P4)
        public static void GoldRushThrow(MovementInfo info)
        {
            if (info._isLast)
            {
                NeuroSDKHandler.SendContext(String.Format("\"The hunt begins... The Road of the King opens... Be torn apart before my eyes\"" +
                    "\n\nThe Red Mist has thrown Gold Rush through a portal and begins charging though it, ending up in the {0} Department.", Helpers.GetDepartmentBySefira(info.end.GetAttachedPassage().GetSefiraEnum())), true);
            }
        }

        //Stunned after Twilight (P4)
        public static void TwilightExhausted(ChaseAction __instance)
        {
            FieldInfo targetAgentInfo = typeof(ChaseAction).GetField("targetAgent", BindingFlags.Instance | BindingFlags.NonPublic);
            AgentModel target = (AgentModel)targetAgentInfo.GetValue(__instance);
            if (target != null)
            {
                NeuroSDKHandler.SendContext("\"I'll break it down... I’ll kill all of you... I can't stop... It just isn’t enough..." +
                    "\n\"I’m just not as capable as I used to be...\"" +
                    "\n\nThe Red Mist has exhausted herself. Strike before she gets up.", true);
            }
        }

        //Stunned after Gold Rush (P4)
        public static void GoldRushThrowExhausted()
        {
            NeuroSDKHandler.SendContext("\"I'll break it down... I’ll kill all of you... I can't stop... It just isn’t enough..." +
                "\n\"I’m just not as capable as I used to be...\"" +
                "\n\nThe Red Mist has exhausted herself. Strike before she gets up.", true);
        }

        /*
         * Due to the nature of this boss, Neuro is unable to do much. But it's kinda boring for her if she just has to sit there,
         * with only the little bits of context to keep her engaged.
         * So I gave her the ability to poke the Red Mist which eventually ragebaits her into standing still while malding neuroTomfoolery.
         * In phases 1-3, if the Red Mist tries to get an action, and is baited, replace the action with idling.
         * However, if she was already going to idle, or if the Red Mist is using a movement/phase transition attack, cancel the bait without resetting it.
         * She specifically needs to be poked when her next action is a normal attack for it to work.
         */
        public static void Phase1InterruptWithRagebait(ref GeburahAction __result)
        {
            if(Poke.RedMistRagebait.Instance != null && Poke.RedMistRagebait.Instance.FullyBaited)
            {
                if(!(__result is GreedyTelepeort)) //&& !(__result is GeburahIdle))
                {
                    __result = Poke.RedMistRagebait.Instance.GetBaitedLULE();
                }
                else
                {
                    Poke.RedMistRagebait.Instance.FullyBaited = false;
                }
            }
        }

        public static void Phase2InterruptWithRagebait(ref GeburahAction __result)
        {
            if (Poke.RedMistRagebait.Instance != null && Poke.RedMistRagebait.Instance.FullyBaited)
            {
                if (!(__result is DacapoThrow) && !(__result is DacapoMimicriThrow) && !(__result is GeburahIdle))
                {
                    __result = Poke.RedMistRagebait.Instance.GetBaitedLULE();
                }
                else
                {
                    Poke.RedMistRagebait.Instance.FullyBaited = false;
                }
            }
        }

        public static void Phase3InterruptWithRagebait(ref GeburahAction __result)
        {
            if (Poke.RedMistRagebait.Instance != null && Poke.RedMistRagebait.Instance.FullyBaited)
            {
                if (!(__result is GreedyTelepeort) && !(__result is DacapoMimicriThrow) && !(__result is GeburahIdle))
                {
                    __result = Poke.RedMistRagebait.Instance.GetBaitedLULE();
                }
                else
                {
                    Poke.RedMistRagebait.Instance.FullyBaited = false;
                }
            }
        }

    }
}
