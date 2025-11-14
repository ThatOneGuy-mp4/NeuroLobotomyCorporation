using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.WatchStory
{
    public class WatchStoryScene : ActionScene
    {
        private readonly int DIALOGUE_OPTION = 1;
        public override string ProcessServerInput(string[] message)
        {
            if (message[COMMAND_INDEX].Equals("select_dialogue"))
            {
                return SelectDialogue.Command(message[DIALOGUE_OPTION]);
            }
            return "Command " + message[COMMAND_INDEX] + " does not exist in scene WatchStoryScene.";
        }
    }
}
