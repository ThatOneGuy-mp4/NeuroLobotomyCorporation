using Harmony;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Collections;
using System.IO.Pipes;
using UnityEditor;
using System.Net;
using UnityEngine.SceneManagement;
using System.Text;
using CreatureSelect;
using System.Net.Sockets;
using NeuroLobotomyCorporation.FacilityManagement;
using GlobalBullet;
using NeuroLobotomyCorporation.YesodSuppression;
using NeuroLobotomyCorporation.HodSuppression;
using NeuroLobotomyCorporation.MalkuthSupression;
using NeuroLobotomyCorporation.NetzachSuppression;
using NeuroLobotomyCorporation.TipherethSuppression;
using NeuroLobotomyCorporation.ChesedSuppression;
using GeburahBoss;
using NeuroLobotomyCorporation.GeburaSuppression;
using Rabbit;
using GameStatusUI;
using NeuroLobotomyCorporation.HokmaSuppression;
using NeuroLobotomyCorporation.BinahSuppression;
using BinahBoss;
using WhiteNightSpace;
using NeuroLobotomyCorporation.WatchStory;

namespace NeuroLobotomyCorporation
{
    public class Harmony_Patch
    {
        //TODO: Make this settable somewhere else later
        public static string gameToServerURI = "http://localhost:8080";
        //TODO: this too
        public static string serverToGameURI = "http://localhost:8081/";

        public Harmony_Patch()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("ThatOneGuy.NeuroLobotomyCorporation");
            harmonyInstance.Patch(typeof(NewTitleScript).GetMethod("Update", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("InitializeSDK")), null);

            //Abnormality Selection fixes 
            harmonyInstance.Patch(typeof(CreatureSelectUI).GetMethod("Init", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("AbnormalityChoiceSelectStart")), null);
            harmonyInstance.Patch(typeof(CreatureSelectUI).GetMethod("OnClickReExtract", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("StoreAbnormalitiesToReextract")), null);
            harmonyInstance.Patch(typeof(CreatureSelectUnit).GetMethod("LateInit", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("AbnormalityFinishedReextracting")), null);
            //very very very important. neuroTomfoolery
            harmonyInstance.Patch(typeof(CreatureSelectUnit).GetMethod("GetName", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("VeryVeryVeryImportantNamePostfix")), null);
            harmonyInstance.Patch(typeof(CreatureSelectUnit).GetMethod("GetText", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("VeryVeryVeryImportantTextPostfix")), null);
            //Abnormality Selection fixes end

            //Story fixes
            harmonyInstance.Patch(typeof(StoryUI).GetMethod("LoadStory", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("StoryStart")), null);
            harmonyInstance.Patch(typeof(StoryCGFadeEffecter).GetMethod("ChangeCG", AccessTools.all), null,
                new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("ContextBackground")), null);
            harmonyInstance.Patch(typeof(StoryDialogueUI).GetMethod("Speak", AccessTools.all), null,
                new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("ContextSpeak")), null);
            harmonyInstance.Patch(typeof(StoryUI).GetMethod("Command_select", AccessTools.all), null,
                new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("ContextDialogueOption")), null);
            harmonyInstance.Patch(typeof(StorySelectionUI).GetMethod("ShowSelection", AccessTools.all), null,
                new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("StartDialogueOption")), null);
            harmonyInstance.Patch(typeof(StoryUI).GetMethod("FixedUpdate", AccessTools.all), null,
                new HarmonyMethod(typeof(WatchStory.SelectDialogue).GetMethod("NeuroSelectDialogueOption")), null);
            harmonyInstance.Patch(typeof(BossMissionAppearUI).GetMethod("Show", AccessTools.all), null,
                new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("ContextBossMission")), null);
            harmonyInstance.Patch(typeof(StorySeedUI).GetMethod("Show", AccessTools.all), null,
                new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("ContextSeedOfLightGermination")), null);
            harmonyInstance.Patch(typeof(StoryUI).GetMethod("Command_ending", AccessTools.all), null,
                new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("ContextLORForeshadowing")), null);
            harmonyInstance.Patch(typeof(StorySceneController).GetMethod("OnEndStory", AccessTools.all), null,
                new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("EndStorySegment")), null);
            //Story fixes end

            harmonyInstance.Patch(typeof(DeployUI).GetMethod("OnManagementStart", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("BeginFacilityManagement", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(LoggingScript).GetMethod("MakeText", AccessTools.all), null,
                new HarmonyMethod(typeof(AssignWork).GetMethod("SortSystemLogs", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(SystemLogScript).GetMethod("OnNotice", AccessTools.all), null,
                new HarmonyMethod(typeof(AssignWork).GetMethod("UpdateNeuroLogsObservationLevel", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GlobalBulletWindow).GetMethod("Update", AccessTools.all), null,
                new HarmonyMethod(typeof(ShootManagerialBullet).GetMethod("NeuroShootBullet", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(SefiraBossManager).GetMethod("OnOverloadActivated", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("ChangeBossPhaseMeltdown", AccessTools.all)), null);

            //Give Neuro context for all of Gebura's special attacks + phase changes
            harmonyInstance.Patch(typeof(GeburahCoreScript).GetMethod("OnTakeDamage", AccessTools.all), null,
                new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("PhaseChangeGebura", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GreedyTelepeort).GetMethod("OnReadyForRun", AccessTools.all), null,
                new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("GoldRushTeleport", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(DacapoThrow).GetMethod("OnStart", AccessTools.all), null,
               new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("DecapoThrow", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(DacapoThrow).GetMethod("MoveToDest", AccessTools.all), null,
               new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("MimicryLeap", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(FourthPhase).GetMethod("GetNextAction", AccessTools.all), null,
               new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("StartTwilightChase", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(ChaseAction).GetMethod("OnArriveAttack", AccessTools.all), null,
               new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("TwilightTargetReached", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GreedyThrow).GetMethod("OnReadyForRun", AccessTools.all), null,
                new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("GoldRushThrow", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(ChaseAction).GetMethod("OnEnd", AccessTools.all), null,
                new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("TwilightExhausted", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GreedyThrow).GetMethod("OnEnd", AccessTools.all), null,
                new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("GoldRushThrowExhausted", AccessTools.all)), null);
            //end Gebura context
            //Red Mist ragebait neuroTomfoolery
            harmonyInstance.Patch(typeof(GeburahBoss.FirstPhase).GetMethod("GetNextAction", AccessTools.all), null,
               new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("Phase1InterruptWithRagebait", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GeburahBoss.SecondPhase).GetMethod("GetNextAction", AccessTools.all), null,
              new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("Phase2InterruptWithRagebait", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GeburahBoss.ThirdPhase).GetMethod("GetNextAction", AccessTools.all), null,
              new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("Phase3InterruptWithRagebait", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(SefiraBossCreatureModel).GetMethod("OnFixedUpdate", AccessTools.all), null,
                new HarmonyMethod(typeof(Poke).GetMethod("PokeRedMist", AccessTools.all)), null);
            //end ragebait

            //Give Neuro context for all of Binah's special attacks + phase changes + meltdowns
            harmonyInstance.Patch(typeof(BinahCoreScript).GetMethod("OnTakeDamage", AccessTools.all), null,
                new HarmonyMethod(typeof(BinahSuppressionScene).GetMethod("PhaseChangeBinah", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GoldenOverload).GetMethod("OnSuccess", AccessTools.all), null,
                new HarmonyMethod(typeof(BinahSuppressionScene).GetMethod("InformNeuroMeltdownGoldCleared", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GoldenOverload).GetMethod("OnFail", AccessTools.all), null,
                new HarmonyMethod(typeof(BinahSuppressionScene).GetMethod("InformNeuroMeltdownGoldNotCleared", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(BlackFogOverload).GetMethod("OnSuccess", AccessTools.all), null,
                new HarmonyMethod(typeof(BinahSuppressionScene).GetMethod("InformNeuroMeltdownDarkFogCleared", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(BlackFogOverload).GetMethod("OnFail", AccessTools.all), null,
                new HarmonyMethod(typeof(BinahSuppressionScene).GetMethod("InformNeuroMeltdownDarkFogNotCleared", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(WaveOverload).GetMethod("OnSuccess", AccessTools.all), null,
                new HarmonyMethod(typeof(BinahSuppressionScene).GetMethod("InformNeuroMeltdownWavesCleared", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(WaveOverload).GetMethod("OnFail", AccessTools.all), null,
                new HarmonyMethod(typeof(BinahSuppressionScene).GetMethod("InformNeuroMeltdownWaveNotCleared", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(ColumnOverload).GetMethod("OnSuccess", AccessTools.all), null,
                new HarmonyMethod(typeof(BinahSuppressionScene).GetMethod("InformNeuroMeltdownPillarsCleared", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(ColumnOverload).GetMethod("OnFail", AccessTools.all), null,
                new HarmonyMethod(typeof(BinahSuppressionScene).GetMethod("InformNeuroMeltdownPillarsNotCleared", AccessTools.all)), null);
            //end Binah context

            harmonyInstance.Patch(typeof(SefiraBossBase).GetMethod("OnCleared", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("BossCleared", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(IsolateRoom).GetMethod("Update", AccessTools.all), null,
                new HarmonyMethod(typeof(CancelAction).GetMethod("CancelChannelledTool", AccessTools.all)), null);

            //The FacilityManagement Context Zone
            harmonyInstance.Patch(typeof(AngelaConversation).GetMethod("MakeMessage", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAngelaMessage", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(AgentModel).GetMethod("OnDie", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAgentDied", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(AgentModel).GetMethod("Panic", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAgentPanicked", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(AgentModel).GetMethod("LoseControl", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAgentBecomesUncontrollable", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(AgentModel).GetMethod("StopPanic", AccessTools.all),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAgentRecoverPanic", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(AgentModel).GetMethod("GetControl", AccessTools.all),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAgentBecomesControllable", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(CreatureModel).GetMethod("UpdateQliphothCounter", AccessTools.all),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAbnormalityQliphothCounterDroppedToZero", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(CreatureModel).GetMethod("Escape", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAbnormalityEscaped", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(ChildCreatureModel).GetMethod("Escape", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroChildSpawned", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(CreatureModel).GetMethod("Suppressed", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAbnormalitySuppressed", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(PlayerModel.EmergencyController).GetMethod("SetLevel", AccessTools.all), new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroPrefixTrumpetLevelChanged", AccessTools.all)),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroPostfixTrumpetLevelChanged", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(CreatureModel).GetMethod("ActivateOverload", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroOverloadActivatedOutsideOfMeltdown", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(OrdealBase).GetMethod("OrdealTypo", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroOrdealStartedAndEnded", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(RabbitCaptaionConversation).GetMethod("GetText", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroRabbitsDialogue", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(RabbitProtocolWindow).GetMethod("OnClickCommand", AccessTools.all), new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroPreRabbitsDeployed", AccessTools.all)),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroPostRabbitsDeployed", AccessTools.all)), null);
            //FacilityManagement context end

            //Hokma Price of Silence Information
            harmonyInstance.Patch(typeof(ChokhmahPlaySpeedBlockUI).GetMethod("SetText", AccessTools.all), null,
                new HarmonyMethod(typeof(HokmaSuppressionScene).GetMethod("SavePriceOfSilenceDialogue", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(ChokhmahBossBase).GetMethod("OnTryTimePause", AccessTools.all), null,
                new HarmonyMethod(typeof(HokmaSuppressionScene).GetMethod("InformNeuroPriceOfSilencePaid", AccessTools.all)), null);
            //Price of Silence Information End

            //WhiteNight context
            harmonyInstance.Patch(typeof(PlagueDoctor).GetMethod("GenDeathAngel", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("StoreWhiteNight", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(AdventClockUI).GetMethod("ExecuteNextAdventTarget", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroFirstAdventApostleSpawned", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(DeathAngel).GetMethod("Escape", AccessTools.all), null,
                new HarmonyMethod(typeof(GetSuppressibleTargets).GetMethod("WhiteNightAdvented", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(DeathAngel).GetMethod("OnSuppressedByConfess", AccessTools.all), null,
                new HarmonyMethod(typeof(GetSuppressibleTargets).GetMethod("OneSinSuppressedWhiteNight", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(DeathAngel).GetMethod("OnSuppressedByDamage", AccessTools.all),
                new HarmonyMethod(typeof(GetSuppressibleTargets).GetMethod("VedalSuppressedWhiteNight", AccessTools.all)), null, null);
            //WhiteNight context end

            //Don't Touch Me context
            harmonyInstance.Patch(typeof(DontTouchMe).GetMethod("OnOpenWorkWindow", AccessTools.all),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("DontTouchMeWorkWindowOpened", AccessTools.all)), null, null);
            harmonyInstance.Patch(typeof(DontTouchMe).GetMethod("OnOpenCollectionWindow", AccessTools.all),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("DontTouchMeCollectionWindowOpened", AccessTools.all)), null, null);
            harmonyInstance.Patch(typeof(DontTouchMe).GetMethod("ExitStart", AccessTools.all),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("DontTouchMeCrashGame", AccessTools.all)), null, null);
            harmonyInstance.Patch(typeof(DontTouchMe).GetMethod("OnFixedUpdate", AccessTools.all),
               new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("DontTouchMeButtonMash", AccessTools.all)), null, null);
            //Don't Touch Me context end

            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

        }

        private static bool initialized = false;
        public static void InitializeSDK()
        {
            if (!initialized)
            {
                Process connector = new Process();
                //TODO: prolly make this settable somewhere else in case this is Not the user's (ved's) file structure
                connector.StartInfo.FileName = Application.dataPath + @"\BaseMods\ThatOneGuy_NeuroLobotomyCorporation\Connector\NeuroLCConnector.exe";
                connector.StartInfo.UseShellExecute = false;
                connector.StartInfo.CreateNoWindow = false;
                connector.Start();
                GameObject sdkHandler = new GameObject("NeuroSDKHandler");
                sdkHandler.AddComponent<NeuroSDKHandler>();
                initialized = true;
            }
        }

        public static void AbnormalityChoiceSelectStart()
        {
            string command = "change_action_scene|abnormality_extraction|";
            string canReextract = "false";
            if (CreatureSelectUI.instance.reExtractController.isShow) canReextract = "true";
            string avaliableAbnormalitiesAndTaglines = "";
            foreach (CreatureSelectUnit unit in CreatureSelectUI.instance.Units)
            {
                if (unit.gameObject.activeSelf)
                {
                    if (unit.IdText.text.Equals("0-00-00-A")) continue;
                    avaliableAbnormalitiesAndTaglines = String.Format("|{0}|{1}", unit.IdText.text, unit.GetText()) + avaliableAbnormalitiesAndTaglines;
                }
            }
            if (String.IsNullOrEmpty(avaliableAbnormalitiesAndTaglines)) avaliableAbnormalitiesAndTaglines = "|NO_EXTRACTION";
            command += canReextract + avaliableAbnormalitiesAndTaglines;
            ActionScene.Instance = new AbnormalityExtraction.AbnormalityExtractionScene();
            NeuroSDKHandler.SendCommand(command);
        }

        private static int abnormalitiesBeingReextracted = 0;
        public static void StoreAbnormalitiesToReextract()
        {
            foreach(CreatureSelectUnit unit in CreatureSelectUI.instance.Units)
            {
                if (unit.gameObject.activeSelf) abnormalitiesBeingReextracted++;
            }
        }

        public static void AbnormalityFinishedReextracting(CreatureSelectUnit __instance)
        {
            if(__instance.gameObject.activeSelf) abnormalitiesBeingReextracted--;
            if (abnormalitiesBeingReextracted == 0) AbnormalityChoiceSelectStart();
        }

        public static void VeryVeryVeryImportantNamePostfix(ref string __result)
        {
            if (__result.Equals("Bald-is-awesome!")) __result = "Literally-Vedal";
        }

        public static void VeryVeryVeryImportantTextPostfix(CreatureSelectUnit __instance, ref string __result)
        {
            if (__instance.IdText.text.Equals("Bald-is-awesome!") || __instance.IdText.text.Equals("Literally-Vedal")) __result = "It's literally him.";
        }

        public static void StoryStart()
        {
            ActionScene.Instance = new WatchStoryScene();
            NeuroSDKHandler.SendCommand("change_action_scene|watch_story");
        }

        public static void BeginFacilityManagement()
        {
            if (SefiraBossManager.Instance.IsAnyBossSessionActivated())
            {
                switch (SefiraBossManager.Instance.CurrentActivatedSefira)
                {
                    case SefiraEnum.MALKUT:
                        ActionScene.Instance = new MalkuthSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|malkuth_suppression");
                        break;
                    case SefiraEnum.YESOD:
                        ActionScene.Instance = new YesodSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|yesod_suppression");
                        break;
                    case SefiraEnum.HOD:
                        ActionScene.Instance = new HodSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|hod_suppression");
                        break;
                    case SefiraEnum.NETZACH:
                        ActionScene.Instance = new NetzachSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|netzach_suppression");
                        break;
                    case SefiraEnum.TIPERERTH1:
                    case SefiraEnum.TIPERERTH2:
                        ActionScene.Instance = new TipherethSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|tiphereth_suppression");
                        break;
                    case SefiraEnum.GEBURAH:
                        ActionScene.Instance = new GeburaSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|gebura_suppression");
                        break;
                    case SefiraEnum.CHESED:
                        ActionScene.Instance = new ChesedSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|chesed_suppression");
                        break;
                    case SefiraEnum.BINAH:
                        ActionScene.Instance = new BinahSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|binah_suppression");
                        break;
                    case SefiraEnum.CHOKHMAH:
                        ActionScene.Instance = new HokmaSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|hokma_suppression");
                        break;
                }
                return;
            }
            ActionScene.Instance = new FacilityManagementScene();
            NeuroSDKHandler.SendCommand("change_action_scene|facility_management");
        }

        public static void ChangeBossPhaseMeltdown()
        {
            if (!SefiraBossManager.Instance.IsAnyBossSessionActivated()) return;
            if (SefiraBossManager.Instance.CheckBossActivation(SefiraEnum.GEBURAH) || SefiraBossManager.Instance.CheckBossActivation(SefiraEnum.BINAH)) return;
            NeuroSDKHandler.SendCommand("change_boss_phase");
        }

        public static void BossCleared()
        {
            NeuroSDKHandler.SendCommand("boss_cleared");
        }
    }
}
