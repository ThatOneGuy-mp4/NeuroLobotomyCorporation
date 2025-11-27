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
    }
}
