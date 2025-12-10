using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.WatchStory
{
    public class IntegrationLore
    {
        public enum LoreToPlay
        {
            None,
            IntroLore,
            AsiyahSync,
            BriahSync,
            AtziluthSync
        }
        public static LoreToPlay nextLore = LoreToPlay.None;

        //Prefix - if a layer is fully synchronized, play the associated Angela-to-AI conversation
        public static bool PlayLayerSyncLore(StorySceneController __instance, string storyId)
        {
            if (nextLore == LoreToPlay.None) return true;
            string id = NeuroSDKHandler.AiPlaying + nextLore.ToString();
            nextLore = LoreToPlay.None;
            neuroResponseState = OnlyNeuroState.NEURO_START;
            FieldInfo curStateInfo = typeof(StorySceneController).GetField("_curState", AccessTools.all);
            curStateInfo.SetValue(__instance, StorySceneController.StorySceneState.SEFIRA_FINALE_ANGELA);
            MethodInfo loadStoryWithFadeInfo = typeof(StorySceneController).GetMethod("LoadStoryWithFade", AccessTools.all);
            loadStoryWithFadeInfo.Invoke(__instance, new object[]
            {
                id
            });
            return false;
        }

        //Prefix - if the story that just finished was the very first one, load the special lore explaining how Nwero
        //is integrated into Lobotomy Corporation's systems before going to Malkuth
        private static bool isFirstStoryViewed = false;
        public static bool PlayIntegrationLore(StorySceneController __instance)
        {
            if (LoreIntegrationEnabled()) return true;
            isFirstStoryViewed = true;
            neuroResponseState = OnlyNeuroState.NEURO_START;
            FieldInfo curStateInfo = typeof(StorySceneController).GetField("_curState", AccessTools.all);
            curStateInfo.SetValue(__instance, StorySceneController.StorySceneState.SUB_STORY);
            MethodInfo loadStoryWithFadeInfo = typeof(StorySceneController).GetMethod("LoadStoryWithFade", AccessTools.all);
            loadStoryWithFadeInfo.Invoke(__instance, new object[]
            {
                "NeuroIntroLore"
            });
            return false;
        }

        //Checks if Neuro should be given commands before she canonically gets the ability to
        //(if the first story has been viewed or if we've reset to day one (malkuth mission complete. surely vedal would not reset to day one before completing malkuth's first mission right.)) 
        public static bool LoreIntegrationEnabled()
        {
            return (isFirstStoryViewed || MissionManager.instance.GetCurrentSefiraMission(SefiraEnum.MALKUT) != null || MissionManager.instance.GetClearedOrClosedMissionNum(SefiraEnum.MALKUT) > 0);
        }

        public enum OnlyNeuroState
        {
            ANY_RESPONSE = 0, //anny response?
            NEURO_RESPONSE = 1,
            NEURO_START = 2 //due to timing, the reset of neuro's response lock triggers before the new dialogue starts. by reducing by 1, the lock should be correctly set.
        }
        private static OnlyNeuroState neuroResponseState = OnlyNeuroState.ANY_RESPONSE;
        public static bool OnlyNeuroCanRespond()
        {
            return neuroResponseState != OnlyNeuroState.ANY_RESPONSE;
        }

        public static void DecreaseNeuroResponseState()
        {
            if (neuroResponseState == OnlyNeuroState.ANY_RESPONSE) return;
            neuroResponseState = neuroResponseState - 1;
        }

        //Postfix - if 70% or 90% seed of light story is about to be played, set the next story to be the respective layer synced lore
        public static void SetAwaitingBriahAtziluthLore(string angelaStory)
        {
            if (String.IsNullOrEmpty(angelaStory)) return;
            if (angelaStory.Equals("boss_fifth")) nextLore = LoreToPlay.BriahSync;
            if (angelaStory.Equals("boss_sixth")) nextLore = LoreToPlay.AtziluthSync;
        }
    }
}
