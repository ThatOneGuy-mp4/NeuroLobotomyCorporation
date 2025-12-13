using Customizing;
using LobotomyBaseMod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class SpecialBossReward
    {
        public static bool AsiyahSynchronizationComplete = false;

        public static bool BriahSynchronizationComplete = false;

        public static bool AtziluthSynchronizationComplete = false;

        public static void EvaluateSynchronizationStatus()
        {
            if (AsiyahSynchronizationComplete || (MissionManager.instance.ExistsFinishedBossMission(SefiraEnum.MALKUT)
                && MissionManager.instance.ExistsFinishedBossMission(SefiraEnum.YESOD)
                && MissionManager.instance.ExistsFinishedBossMission(SefiraEnum.HOD)
                && MissionManager.instance.ExistsFinishedBossMission(SefiraEnum.NETZACH))) AsiyahSynchronizationComplete = true;
            else return;

            if (BriahSynchronizationComplete || (MissionManager.instance.ExistsFinishedBossMission(SefiraEnum.TIPERERTH1)
                && MissionManager.instance.ExistsFinishedBossMission(SefiraEnum.GEBURAH)
                && MissionManager.instance.ExistsFinishedBossMission(SefiraEnum.CHESED))) BriahSynchronizationComplete = true;
            else return;

            if (AtziluthSynchronizationComplete || (MissionManager.instance.ExistsFinishedBossMission(SefiraEnum.BINAH)
                && MissionManager.instance.ExistsFinishedBossMission(SefiraEnum.CHOKHMAH))) AtziluthSynchronizationComplete = true;
        }

        public static bool ReevaluateSynchronizationStatus()
        {
            bool asiyah = AsiyahSynchronizationComplete;
            bool briah = BriahSynchronizationComplete;
            bool atziluth = AtziluthSynchronizationComplete;
            EvaluateSynchronizationStatus();
            return (asiyah != AsiyahSynchronizationComplete || briah != BriahSynchronizationComplete || atziluth != AtziluthSynchronizationComplete);
        }

        public static bool NoBulletConsumed()
        {
            float bulletNotConsumedChance = 0f;
            if (BriahSynchronizationComplete) bulletNotConsumedChance += 0.1f;
            if (AtziluthSynchronizationComplete) bulletNotConsumedChance += 0.1f;
            return UnityEngine.Random.value < bulletNotConsumedChance;
        }

        public class NeuroAssignmentBuf : UnitStatBuf
        {
            public NeuroAssignmentBuf(UnitModel target, AgentModel agent) : base(float.MaxValue, (UnitBufType)727)
            {
                int buf = 0;
                this.model = agent;
                if (!agent.HasEquipment_Mod(NEURO_HAIRPIN_REAL_ID) && !agent.HasEquipment_Mod(EVIL_HAIRPIN_REAL_ID))
                {
                    if (AsiyahSynchronizationComplete) buf += BASE_INCREASE;
                    if (AtziluthSynchronizationComplete) buf += UPGRADE_INCREASE;
                }
                this.primaryStat.hp = buf;
                this.primaryStat.mental = buf;
                this.primaryStat.work = buf;
                this.primaryStat.battle = buf;
                this.neuroAssignedTarget = target;
            }

            public override void FixedUpdate()
            {
                if (!CanHaveBuf()) Destroy(); 
            }

            //If the Agent is doing something that is not what Neuro asked, destroy the buff.
            private bool CanHaveBuf()
            {
                AgentModel agent = (AgentModel)this.model;
                switch (Helpers.GetAgentWorkingState(agent))
                {
                    case Helpers.AgentWorkingState.WORKING:
                        if (agent.target == null) return false;
                        return agent.target == neuroAssignedTarget;
                    case Helpers.AgentWorkingState.SUPPRESSING:
                        if (agent.target != null) return agent.target == neuroAssignedTarget;
                        if (agent.targetWorker != null) return agent.targetWorker == neuroAssignedTarget;
                        return false;
                }
                return false;
            }

            private const int BASE_INCREASE = 5;
            private const int UPGRADE_INCREASE = 5;

            private UnitModel neuroAssignedTarget = null;
        }

        private static readonly float BASE_EGO_CHANCE = 0.01f;
        private static readonly float ASIYAH_EGO_ADD = 0.01f;
        private static readonly float BRIAH_EGO_ADD = 0.02f;
        private static readonly float ATZILUTH_EGO_ADD = 0.01f;
        private static readonly LcId NEURO_HAIRPIN_FAKE_ID = new LcId("NeuroLobotomyCorporation", 412192022);
        private static readonly LcId NEURO_HAIRPIN_REAL_ID = new LcId("NeuroLobotomyCorporation", 412192024);
        private static readonly LcId EVIL_HAIRPIN_FAKE_ID = new LcId("NeuroLobotomyCorporation", 43252024);
        private static readonly LcId EVIL_HAIRPIN_REAL_ID = new LcId("NeuroLobotomyCorporation", 43252025);

        //Postfix - if Work is completed while an Agent still has the buf from Neuro/Evil assigning work, small chance to make it permanent
        //(is cosmetic until Atziluth synced)
        public static void TryGiveHairpinEGO(UseSkill __instance)
        {
            if (__instance.agent.HasUnitBuf((UnitBufType)727))
            {
                float egoChance = BASE_EGO_CHANCE;
                if (AsiyahSynchronizationComplete) egoChance += ASIYAH_EGO_ADD;
                if (BriahSynchronizationComplete) egoChance += BRIAH_EGO_ADD;
                if (AtziluthSynchronizationComplete) egoChance += ATZILUTH_EGO_ADD;
                if(!UpgradeHairpin(__instance.agent) && egoChance >= UnityEngine.Random.value && !AgentHasHairpin(__instance.agent))
                {
                    if (!AtziluthSynchronizationComplete && __instance.agent.Equipment.gifts.addedGifts.Find((EGOgiftModel e) => e.metaInfo.AttachRegion == EGOgiftAttachRegion.HAIR) != null) return;
                    if (NeuroSDKHandler.AiPlaying.Equals("Neuro"))
                    {
                        if (!AtziluthSynchronizationComplete)
                        {
                            ConsoleCommand_Mod.AddGift_Mod(__instance.agent.instanceId, NEURO_HAIRPIN_FAKE_ID);
                            NeuroSDKHandler.SendContext(String.Format("...something seems to have attached itself to {0}'s head.", __instance.agent.GetUnitName()), true);
                            return;
                        }
                        else
                        {
                            ConsoleCommand_Mod.AddGift_Mod(__instance.agent.instanceId, NEURO_HAIRPIN_REAL_ID);
                            NeuroSDKHandler.SendContext(String.Format("{0} has received your E.G.O Gift.", __instance.agent.GetUnitName()), true);
                            return;
                        }
                    }
                    else
                    {
                        if (!AtziluthSynchronizationComplete)
                        {
                            ConsoleCommand_Mod.AddGift_Mod(__instance.agent.instanceId, EVIL_HAIRPIN_FAKE_ID);
                            NeuroSDKHandler.SendContext(String.Format("...something seems to have attached itself to {0}'s head.", __instance.agent.GetUnitName()), true);
                            return;
                        }
                        else
                        {
                            ConsoleCommand_Mod.AddGift_Mod(__instance.agent.instanceId, EVIL_HAIRPIN_REAL_ID);
                            NeuroSDKHandler.SendContext(String.Format("{0} has received your E.G.O Gift.", __instance.agent.GetUnitName()), true);
                            return;
                        }
                    }
                }
            }
        }

        //Upgrade the hairpin after synchronization, or when Neuro/Evil assigned work finishes after synchronization (in case of memory repo wiping the upgrades)
        private static bool UpgradeHairpin(AgentModel agent)
        {
            if (!AtziluthSynchronizationComplete) return false;
            if (agent.HasEquipment_Mod(NEURO_HAIRPIN_FAKE_ID))
            {
                ConsoleCommand_Mod.AddGift_Mod(agent.instanceId, NEURO_HAIRPIN_REAL_ID);
                return true;
            }
            else if (agent.HasEquipment_Mod(EVIL_HAIRPIN_FAKE_ID))
            {
                ConsoleCommand_Mod.AddGift_Mod(agent.instanceId, EVIL_HAIRPIN_REAL_ID);
                return true;
            }
            return false;
        }

        private static bool AgentHasHairpin(AgentModel agent)
        {
            return (agent.HasEquipment_Mod(NEURO_HAIRPIN_FAKE_ID)
                || agent.HasEquipment_Mod(NEURO_HAIRPIN_REAL_ID)
                || agent.HasEquipment_Mod(EVIL_HAIRPIN_FAKE_ID)
                || agent.HasEquipment_Mod(EVIL_HAIRPIN_REAL_ID));
        }

        private static void UpgradeAllHairpins()
        {
            List<AgentModel> allAgents = new List<AgentModel>();
            allAgents.AddRange(AgentManager.instance.GetAgentList());
            allAgents.AddRange(AgentManager.instance.GetAgentSpareList());
            foreach(AgentModel agent in allAgents)
            {
                UpgradeHairpin(agent);
            }
        }

        //Postfix - when all Asiyah Core Suppressions are complete, fix the movement speed increase Malkuth's Synchronization should have given
        public static void FixMalkuthSyncReward(ref float __result)
        {
            if (AsiyahSynchronizationComplete) __result += 0.5f;
        }

        //Postfix - ... fix Hod's hiring stat bonus
        public static void FixHodResearchReward(ref int __result, CustomizingWindow __instance)
        {
            if (AsiyahSynchronizationComplete)
            {
                if (__instance.CurrentWindowType == CustomizingType.GENERATE) __result += 5;
            }
        }

        //Prefix - ... Briah ... fix Chesed's HP & SP Bullet recovery amount upgrade
        public static bool FixChesedHPResearchReward(UnitModel target)
        {
            if (!BriahSynchronizationComplete) return true;
            WorkerModel workerModel = target as WorkerModel;
            if (workerModel != null && !workerModel.HasUnitBuf(UnitBufType.QUEENBEE_SPORE))
            {
                int num = 25 + 15;
                workerModel.RecoverHP((float)num);
            }
            return false;
        }

        public static bool FixChesedSPResearchReward(UnitModel target)
        {
            if (!BriahSynchronizationComplete) return true;
            WorkerModel workerModel = target as WorkerModel;
            if (workerModel != null && !workerModel.IsPanic())
            {
                int num = 25 + 15;
                workerModel.RecoverMental((float)num);
            }
            return false;
        }

        //Prefix - ... fix Gebura's Slow Bullets (and other slowing sources) not affecting Ordeals
        public static void FixGeburaResearchReward(MovableObjectNode __instance, ref float movement)
        {
            if (!BriahSynchronizationComplete || __instance.GetUnit() == null || !(__instance.GetUnit() is OrdealCreatureModel)) return;
            movement *= __instance.GetUnit().GetMovementScaleByBuf();
        }

        //Create the pop-up describing the special upgrades given after synchronizing each layer.
        public static void MakeSpecialBossRewardNotice(ResearchWindow instance)
        {
            InitSpecialRewardWindow(instance);
            string newlySyncedLayer = "";
            if (AtziluthSynchronizationComplete) { newlySyncedLayer = "Atziluth"; UpgradeAllHairpins(); }
            else if (BriahSynchronizationComplete) newlySyncedLayer = "Briah";
            else newlySyncedLayer = "Asiyah";
            List<string> descs = new List<string>();
            string title = LocalizeTextDataModel.instance.GetText(String.Format("{0}Sync_Title", newlySyncedLayer));
            string finalRewardMessage = title + "\nREWARD:\n";
            if (!String.IsNullOrEmpty(title)) instance.sefiraBoss_Prefix.text = title;
            int i = 1;
            string nextDesc = String.Format(LocalizeTextDataModel.instance.GetText(String.Format("{0}Sync_{1}", newlySyncedLayer, i)), NeuroSDKHandler.AiPlaying);
            while(!String.IsNullOrEmpty(nextDesc) && !nextDesc.Equals("UNKNOWN"))
            {
                descs.Add(nextDesc);
                finalRewardMessage += "\n" + nextDesc;
                i++;
                nextDesc = String.Format(LocalizeTextDataModel.instance.GetText(String.Format("{0}Sync_{1}", newlySyncedLayer, i)), NeuroSDKHandler.AiPlaying);
            }
            instance.researchPanelArea.SetActive(false);
            instance.sefiraBossRoot.SetActive(true);
            instance.LowerArea.gameObject.SetActive(false);
            instance.sefiraBossButton.SetActive(true);
            IEnumerator enumerator = instance.sefiraBoss_ListParent.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object obj = enumerator.Current;
                    Transform transform = (Transform)obj;
                    if (!(transform == instance.sefiraBoss_ListParent.transform))
                    {
                        UnityEngine.Object.Destroy(transform.gameObject);
                    }
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = (enumerator as IDisposable)) != null)
                {
                    disposable.Dispose();
                }
            }
            foreach(string s in descs)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(instance.sefiraTextUnit);
                gameObject.transform.SetParent(instance.sefiraBoss_ListParent);
                gameObject.transform.localScale = Vector3.one;
                gameObject.transform.GetChild(1).GetComponent<Text>().text = s;
            }
            NeuroSDKHandler.SendContext(finalRewardMessage);
        }

        //ResearchWindow's Init, modified to not use any Sefira objects and set color properly
        private static void InitSpecialRewardWindow(ResearchWindow instance)
        {
            instance.ui.AreaName.text = "Integration Team";
            instance.ui.AreaDesc.text = "Integration Team\nUpdate Pushed";
            instance.LowerArea.gameObject.SetActive(true);
            instance.SetActive(false);
            instance.selectedPanel.SetSelectedPanel();
            instance.selectedPanel.SetWindow(instance);
            instance.researchPanelArea.SetActive(true);
            instance.sefiraBossRoot.SetActive(false);
            instance.sefiraBossButton.SetActive(false);
            SetColor(instance);
            foreach (ResearchPanel researchPanel in instance.panels)
            {
                researchPanel.SetWindow(instance);
                researchPanel.PanelReset();
            }
            instance.dropHandler.SetDropEvent(new Drop.OnDropEvent(instance.OnDropEvent));
            instance.selectedPanel.SetColor(AIPlayingColor().imageColor);
            instance.OnSetPanel(false);
            instance.SetInst(instance.grayFactor);
            instance.DropFeildPivot.transform.localScale = Vector3.one;
            instance.sefiraBossRoot.SetActive(false);
            instance.Portrait.sprite = GetAISprite(NeuroSDKHandler.AiPlaying);
            instance.controller.Show();
        }

        //Get Neuro or Evil's sprite for the reward window. Signature Look of Superiority
        private static Sprite GetAISprite(string ai)
        {
            string spriteName = "";
            if(ai.Equals("Neuro"))
            {
                spriteName = "NeuroSprite.png";
            }
            else
            {
                spriteName = "EvilSprite.png";
            }
            return LobotomyBaseMod.ExtenionUtil.CreateSpriteByPng(Application.dataPath + "/BaseMods/ThatOneGuy_NeuroLobotomyCorporation/Resources/" + spriteName);
        }

        //Create special UI colors for Neuro and Evil.
        public static SefiraUIColor AIPlayingColor()
        {
            if (NeuroSDKHandler.AiPlaying.Equals("Neuro")) return NeuroColor();
            else return EvilColor();
        }

        private static SefiraUIColor NeuroColor()
        {
            SefiraUIColor nc = new SefiraUIColor();
            nc.sefira = (SefiraEnum)1219;
            nc.imageColor = new UnityEngine.Color((float)254 / 255, (float)168 / 255, (float)174 / 255);
            nc.textColor = new UnityEngine.Color(0, 0, 0);
            nc.ui_1 = new UnityEngine.Color((float)254 / 255, (float)168 / 255, (float)174 / 255);
            nc.ui_2 = new UnityEngine.Color(1, 1, 1);
            return nc;
        }

        private static SefiraUIColor EvilColor()
        {
            SefiraUIColor nc = new SefiraUIColor();
            nc.sefira = (SefiraEnum)325;
            nc.imageColor = new UnityEngine.Color((float)113 / 255, (float)21 / 255, (float)55 / 255);
            nc.textColor = new UnityEngine.Color(0, 0, 0);
            nc.ui_1 = new UnityEngine.Color((float)113 / 255, (float)21 / 255, (float)55 / 255);
            nc.ui_2 = new UnityEngine.Color(1, 1, 1);
            return nc;
        }

        //Set the UI elements to use Neuro or Evil's colors.
        private static void SetColor(ResearchWindow instance)
        {
            SefiraUIColor color = AIPlayingColor();
            foreach (ColorMultiplier colorMultiplier in instance.buttonColorMultiplier)
            {
                colorMultiplier.Init(color.imageColor);
            }
            foreach (ColorMultiplier colorMultiplier2 in instance.instructions)
            {
                colorMultiplier2.Init(color.imageColor);
            }
            foreach (MaskableGraphic maskableGraphic in instance.coloredTargets)
            {
                maskableGraphic.color = color.imageColor;
            }
            foreach (ResearchPanel researchPanel in instance.panels)
            {
                researchPanel.SetColor(color.imageColor);
            }
        }
    }
}
