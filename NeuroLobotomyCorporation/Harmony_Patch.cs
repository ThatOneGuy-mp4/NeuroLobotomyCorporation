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
using NeuroLobotomyCorporation.ClawSuppression;
using KetherBoss;
using NeuroLobotomyCorporation.AbelSuppression;
using NeuroLobotomyCorporation.AbramSuppression;
using NeuroLobotomyCorporation.AdamSuppression;
using NeuroLobotomyCorporation.DaatSuppression;
using Credit;
using NeuroLobotomyCorporation.WatchStory;
using NeuroLobotomyCorporation.DayPreparation;
using Customizing;

namespace NeuroLobotomyCorporation
{
    public class Harmony_Patch
    {
        public Harmony_Patch()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("ThatOneGuy.NeuroLobotomyCorporation");
            //Initialization and console commands
            harmonyInstance.Patch(typeof(NewTitleScript).GetMethod("Update", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("InitializePreEndingTitleScreen")), null);
            harmonyInstance.Patch(typeof(AlterTitleController).GetMethod("Update", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("InitializePostEndingTitleScreen")), null);
            harmonyInstance.Patch(typeof(ConsoleScript).GetMethod("OnExitEdit", AccessTools.all), null,
                new HarmonyMethod(typeof(NeuroSDKHandler).GetMethod("SDKConsoleCommand")), null);
            //end initialization and console commands

            //Remove actions while the pause menu effects are happening
            harmonyInstance.Patch(typeof(EscapeUI).GetMethod("RestartAtCheckpoint", AccessTools.all),
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("ClearActionsRetryDay")), null, null);
            harmonyInstance.Patch(typeof(EscapeUI).GetMethod("OnClickCheckPointConfirm", AccessTools.all),
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("ClearActionsMemoryRepository")), null, null);
            harmonyInstance.Patch(typeof(EscapeUI).GetMethod("MoveTitle", AccessTools.all),
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("ClearActionsMoveToTitle")), null, null);
            //End Pause Menu fixes

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
            harmonyInstance.Patch(typeof(StoryUI).GetMethod("OnClickSkip", AccessTools.all), null,
               new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("StopContextWhileSkipping")), null);
            harmonyInstance.Patch(typeof(StoryUI).GetMethod("OnClickDialogue", AccessTools.all), null,
               new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("StartContextWhenSkippingDisabled")), null);
            harmonyInstance.Patch(typeof(StoryCGFadeEffecter).GetMethod("ChangeCG", AccessTools.all), null,
                new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("ContextBackground")), null);
            harmonyInstance.Patch(typeof(StoryDialogueUI).GetMethod("Speak", AccessTools.all), null,
                new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("ContextSpeak")), null);
            harmonyInstance.Patch(typeof(StoryUI).GetMethod("OnSelectSelection", AccessTools.all),
                new HarmonyMethod(typeof(WatchStory.Patches).GetMethod("IsNeuroOnlyResponse")), null, null);
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
            //Play Integration Lore
            harmonyInstance.Patch(typeof(StorySceneController).GetMethod("TryPlayNextSubStory", AccessTools.all), 
                new HarmonyMethod(typeof(IntegrationLore).GetMethod("PlayIntegrationLore")), null, null);
            harmonyInstance.Patch(typeof(StorySceneController).GetMethod("LoadStoryWithFade", AccessTools.all),
               new HarmonyMethod(typeof(IntegrationLore).GetMethod("PlayLayerSyncLore")), null, null);
            harmonyInstance.Patch(typeof(StorySceneController).GetMethod("OnEndSeedUI", AccessTools.all),
               new HarmonyMethod(typeof(IntegrationLore).GetMethod("SetAwaitingLayerLore")), null, null);
            //Story fixes end

            //Day Preparation fixes
            harmonyInstance.Patch(typeof(DeployUI).GetMethod("Init", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("DayPreparationStart", AccessTools.all)),
                new HarmonyMethod(typeof(DayPreparation.Patches).GetMethod("ResearchPhaseProgress", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(DeployUI).GetMethod("CheckResearch", AccessTools.all), null,
                new HarmonyMethod(typeof(DayPreparation.Patches).GetMethod("ResearchCheck", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(SefiraPanel).GetMethod("OnStartBossSession", AccessTools.all), null,
                new HarmonyMethod(typeof(DayPreparation.Patches).GetMethod("CoreSuppressionEnabledDisabled", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(ResearchWindow).GetMethod("MakeSefiraBossReward", AccessTools.all), null,
                new HarmonyMethod(typeof(DayPreparation.Patches).GetMethod("CoreSuppressionRewardContext", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(AppearanceUI).GetMethod("OpenWindow", AccessTools.all), null,
                new HarmonyMethod(typeof(DayPreparation.Patches).GetMethod("EnableCustomizeAgent", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(CustomizingWindow).GetMethod("Cancel", AccessTools.all), null,
                new HarmonyMethod(typeof(DayPreparation.Patches).GetMethod("DisableCustomizeAgent", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(CustomizingWindow).GetMethod("Update", AccessTools.all), null,
                new HarmonyMethod(typeof(CustomizeAgent).GetMethod("SetCustomAppearance", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(ResearchWindow).GetMethod("OnConfirm", AccessTools.all),
                new HarmonyMethod(typeof(DayPreparation.Patches).GetMethod("InterruptResearchWithSpecialBossReward", AccessTools.all)), null, null);
            //Day Preparation fixes end

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
            //Red Mist ragebait neuroTomfoolery
            harmonyInstance.Patch(typeof(GeburahBoss.FirstPhase).GetMethod("GetNextAction", AccessTools.all), null,
               new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("Phase1InterruptWithRagebait", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GeburahBoss.SecondPhase).GetMethod("GetNextAction", AccessTools.all), null,
              new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("Phase2InterruptWithRagebait", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GeburahBoss.ThirdPhase).GetMethod("GetNextAction", AccessTools.all), null,
              new HarmonyMethod(typeof(GeburaSuppressionScene).GetMethod("Phase3InterruptWithRagebait", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(SefiraBossCreatureModel).GetMethod("OnFixedUpdate", AccessTools.all), null,
                new HarmonyMethod(typeof(Poke).GetMethod("PokeRedMist", AccessTools.all)), null);
            //end Gebura context

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

            //Core Suppression fixes
            harmonyInstance.Patch(typeof(SefiraBossManager).GetMethod("OnOverloadActivated", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("ChangeBossPhaseMeltdown", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(SefiraBossBase).GetMethod("OnCleared", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("BossCleared", AccessTools.all)), null);
            //end Core Suppression fixes

            //Keter Suppression fixes
            harmonyInstance.Patch(typeof(KetherBossBase).GetMethod("OnCleared", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("KeterBossCleared", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GameManager).GetMethod("GameOverEnding", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("KeterBossFailed", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(KetherMiddleBossBase).GetMethod("GetDescType", AccessTools.all), null,
               new HarmonyMethod(typeof(AbramSuppressionScene).GetMethod("AbramDescOverride", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(KetherLowerBossBase).GetMethod("GetDescType", AccessTools.all), null,
               new HarmonyMethod(typeof(AdamSuppressionScene).GetMethod("AdamDescOverride", AccessTools.all)), null);
            //end Keter Suppression fixes

            //Da'at Suppression fixes/Spinning
            harmonyInstance.Patch(typeof(KetherLastBossBase).GetMethod("EnergyUpdate", AccessTools.all), new HarmonyMethod(typeof(DaatSuppressionScene).GetMethod("ChangePhaseDaatPrefix", AccessTools.all)),
                new HarmonyMethod(typeof(DaatSuppressionScene).GetMethod("ChangePhaseDaatPostfix", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(KetherLastBossBase).GetMethod("Update", AccessTools.all),
                new HarmonyMethod(typeof(Spin).GetMethod("InitiateNURU", AccessTools.all)), null, null);
            harmonyInstance.Patch(typeof(KetherLastBossBase).GetMethod("EnergyLevelChange", AccessTools.all), 
                new HarmonyMethod(typeof(Spin).GetMethod("ReplaceCameraRotate", AccessTools.all)), null, null);
            harmonyInstance.Patch(typeof(CreditConversationController).GetMethod("GetText", AccessTools.all), null,
               new HarmonyMethod(typeof(DaatSuppressionScene).GetMethod("InformNeuroFinalAyinConversation", AccessTools.all)), null);
            //also, disable post-credit sequence requirement
            harmonyInstance.Patch(typeof(CreatureManager).GetMethod("IsMaxHiddenProgress", AccessTools.all), null,
               new HarmonyMethod(typeof(DaatSuppressionScene).GetMethod("RemoveHiddenEndingCondition", AccessTools.all)), null);
            //end Da'at Suppression fixes

            //Special Boss Rewards (a.k.a., research bugfixes)
            harmonyInstance.Patch(typeof(AgentModel).GetMethod("GetMovementValue", AccessTools.all), null, 
                new HarmonyMethod(typeof(SpecialBossReward).GetMethod("FixMalkuthSyncReward", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(CustomizingWindow).GetMethod("SetRandomStatValue", AccessTools.all), null, 
                new HarmonyMethod(typeof(SpecialBossReward).GetMethod("FixHodResearchReward", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GlobalBulletManager).GetMethod("RecoverHPBullet", AccessTools.all),
                new HarmonyMethod(typeof(SpecialBossReward).GetMethod("FixChesedHPResearchReward", AccessTools.all)), null, null);
            harmonyInstance.Patch(typeof(GlobalBulletManager).GetMethod("RecoverMentalBullet", AccessTools.all), 
                new HarmonyMethod(typeof(SpecialBossReward).GetMethod("FixChesedSPResearchReward", AccessTools.all)), null, null);
            harmonyInstance.Patch(typeof(MovableObjectNode).GetMethod("ProcessMoveNode", AccessTools.all),
                new HarmonyMethod(typeof(SpecialBossReward).GetMethod("FixGeburaResearchReward", AccessTools.all)), null, null);
            harmonyInstance.Patch(typeof(UseSkill).GetMethod("FinishWorkSuccessfully", AccessTools.all), null,
                new HarmonyMethod(typeof(SpecialBossReward).GetMethod("TryGiveHairpinEGO", AccessTools.all)), null);
            //end Special Boss Reward fixes

            //Facility Management patches and context
            harmonyInstance.Patch(typeof(DeployUI).GetMethod("OnManagementStart", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("BeginFacilityManagement", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(LoggingScript).GetMethod("MakeText", AccessTools.all), null,
                new HarmonyMethod(typeof(AssignWork).GetMethod("SortSystemLogs", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(SystemLogScript).GetMethod("OnNotice", AccessTools.all), null,
                new HarmonyMethod(typeof(AssignWork).GetMethod("UpdateNeuroLogsObservationLevel", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GlobalBulletWindow).GetMethod("Update", AccessTools.all), null,
                new HarmonyMethod(typeof(ShootManagerialBullet).GetMethod("NeuroShootBullet", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(IsolateRoom).GetMethod("Update", AccessTools.all), null,
                new HarmonyMethod(typeof(CancelAction).GetMethod("CancelChannelledTool", AccessTools.all)), null);
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
            harmonyInstance.Patch(typeof(ResultScreen.Report).GetMethod("OnManagementEnd", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("SendResultScreenInformation", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(Sefira).GetMethod("OnAgentCannotControll", AccessTools.all), new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroPrefixAllAgentsDead", AccessTools.all)),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroPostfixAllAgentsDead", AccessTools.all)), null);
            //FacilityManagement end

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

            //Patch remaining methods that can't be manually patched due to having overloads
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

        }

        public static void InitializePreEndingTitleScreen()
        {
            NeuroSDKHandler.InitializeSDK();
        }

        public static void InitializePostEndingTitleScreen()
        {
            NeuroSDKHandler.InitializeSDK();
        }

        public static void ClearActionsRetryDay()
        {
            NeuroSDKHandler.SendCommand("clear_actions");
        }

        public static void ClearActionsMemoryRepository()
        {
            NeuroSDKHandler.SendCommand("clear_actions");
        }

        public static void ClearActionsMoveToTitle()
        {
            NeuroSDKHandler.SendCommand("clear_actions");
        }

        public static void AbnormalityChoiceSelectStart()
        {
            if (!IntegrationLore.LoreIntegrationEnabled()) return;
            string command = GetAbnormalityStartCommand();
            ActionScene.Instance = new AbnormalityExtraction.AbnormalityExtractionScene();
            NeuroSDKHandler.SendCommand(command);
        }

        public static string GetAbnormalityStartCommand()
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
            return command;
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

        public static void DayPreparationStart()
        {
            ActionScene.Instance = new DayPreparationScene();
            NeuroSDKHandler.SendCommand("change_action_scene|day_preparation");
        }

        public static void BeginFacilityManagement()
        {
            FacilityManagementScene managementScene = null;
            string typeOfManagement = "";
            if (SefiraBossManager.Instance.IsAnyBossSessionActivated())
            {
                switch (SefiraBossManager.Instance.CurrentActivatedSefira)
                {
                    case SefiraEnum.MALKUT:
                        managementScene = new MalkuthSuppressionScene();
                        typeOfManagement = "malkuth";
                        break;
                    case SefiraEnum.YESOD:
                        managementScene = new YesodSuppressionScene();
                        typeOfManagement = "yesod";
                        break;
                    case SefiraEnum.HOD:
                        managementScene = new HodSuppressionScene();
                        typeOfManagement = "hod";
                        break;
                    case SefiraEnum.NETZACH:
                        managementScene = new NetzachSuppressionScene();
                        typeOfManagement = "netzach";
                        break;
                    case SefiraEnum.TIPERERTH1:
                    case SefiraEnum.TIPERERTH2:
                        managementScene = new TipherethSuppressionScene();
                        typeOfManagement = "tiphereth";
                        break;
                    case SefiraEnum.GEBURAH:
                        managementScene = new GeburaSuppressionScene();
                        typeOfManagement = "gebura";
                        break;
                    case SefiraEnum.CHESED:
                        managementScene = new ChesedSuppressionScene();
                        typeOfManagement = "chesed";
                        break;
                    case SefiraEnum.BINAH:
                        managementScene = new BinahSuppressionScene();
                        typeOfManagement = "binah";
                        break;
                    case SefiraEnum.CHOKHMAH:
                        managementScene = new HokmaSuppressionScene();
                        typeOfManagement = "hokma";
                        break;
                    case SefiraEnum.KETHER:
                        switch (PlayerModel.instance.GetDay())
                        {
                            case 45:
                                managementScene = new ClawSuppressionScene();
                                typeOfManagement = "claw";
                                break;
                            case 46:
                                managementScene = new AbelSuppressionScene();
                                typeOfManagement = "abel";
                                break;
                            case 47:
                                managementScene = new AbramSuppressionScene();
                                typeOfManagement = "abram";
                                break;
                            case 48:
                                managementScene = new AdamSuppressionScene();
                                typeOfManagement = "adam";
                                break;
                            case 49:
                                managementScene = new DaatSuppressionScene();
                                Spin.NeuroCameraRotationEvent.ResetSpinParams(); //y'know i've been using a lot of static variables in this mod but i haven't been clearing any of them. might wanna do that so there isn't any issues between days.
                                typeOfManagement = "daat";
                                break;
                        }
                        break;
                }
                typeOfManagement = String.Format("{0}_suppression", typeOfManagement);
            }
            else
            {
                managementScene = new FacilityManagementScene();
                typeOfManagement = "facility_management";
            }
            FacilityManagementScene.ResetStaticParams();
            ActionScene.Instance = managementScene;
            string exParameters = ShootManagerialBullet.IsBulletUnlocked() + "|" + managementScene.GetDayStartContext();
            NeuroSDKHandler.SendCommand("change_action_scene|" + typeOfManagement + "|" + exParameters);
        }

        public static void ChangeBossPhaseMeltdown()
        {
            if (!SefiraBossManager.Instance.IsAnyBossSessionActivated()) return;
            if (SefiraBossManager.Instance.CheckBossActivation(SefiraEnum.GEBURAH) || SefiraBossManager.Instance.CheckBossActivation(SefiraEnum.BINAH)) return;
            if (SefiraBossManager.Instance.IsKetherBoss(KetherBossType.E4)) return;
            NeuroSDKHandler.SendCommand("change_boss_phase");
        }

        public static void BossCleared()
        {
            NeuroSDKHandler.SendCommand("boss_cleared");
        }

        public static void KeterBossCleared()
        {
            //Gebura and Binah's deaths will trigger the boss clear by themselves, so if the battle was won via their won conditions, don't send the boss clear.
            if (SefiraBossManager.Instance.IsKetherBoss(KetherBossType.E2))
            {
                UnitModel redMistModel = Helpers.TryFindSefiraCoreTarget("The Red Mist");
                if (redMistModel == null || redMistModel.hp <= 0) return;
            }
            if (SefiraBossManager.Instance.IsKetherBoss(KetherBossType.E3))
            {
                UnitModel anArbiterModel = Helpers.TryFindSefiraCoreTarget("An Arbiter");
                if (anArbiterModel == null || anArbiterModel.hp <= 0) return;
            }
            if (SefiraBossManager.Instance.IsKetherBoss(KetherBossType.E4))
            {
                DaatSuppressionScene.DaatSuppressed = true;
            }
            NeuroSDKHandler.SendCommand("boss_cleared");
        }

        public static void KeterBossFailed()
        {
            NeuroSDKHandler.SendCommand("boss_failed");
        }
    }
}
