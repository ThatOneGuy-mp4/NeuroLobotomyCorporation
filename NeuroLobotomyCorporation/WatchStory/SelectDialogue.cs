using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.WatchStory
{
    public class SelectDialogue
    {
        private static int dialogueIndex = -1;
        public static string Command(string selectedDialogue)
        {
            for(int i = 0; i < Patches.dialogueOptions.Count; i++)
            {
                string selected = Patches.dialogueOptions[i];
                if (selected.Equals(selectedDialogue))
                {
                    WatchStory.Patches.responseFromNeuro = true;
                    dialogueIndex = i;
                    if (selected.Equals("Neuro") || selected.Equals("Evil")) NeuroSDKHandler.SetAIPlaying(selected);
                    break;
                }
            }
            return "";
        }

        //Postfix - calling OnSelectSelection within Command crashes the game when a previous dialogue option
        //had 1 option and the current one has 3. For some reason. So do it in a postfix.
        public static void NeuroSelectDialogueOption(StoryUI __instance)
        {
            if (dialogueIndex != -1)
            {
                __instance.OnSelectSelection(dialogueIndex);
                dialogueIndex = -1;
            }
        }
    }
}
