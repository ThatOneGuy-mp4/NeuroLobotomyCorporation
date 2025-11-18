using CreatureSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.AbnormalityExtraction
{
    public class AbnormalityExtractionScene : ActionScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "extract_abnormality":
                    return ExtractAbnormality.Command(message[ExtractAbnormality.ABNORMALITY_NAME_INDEX]);
                case "reextract_abnormalities":
                    return ReextractAbnormalities.Command();
            }
            return "Command " + message[COMMAND_INDEX] + " does not exist in scene AbnormalityExtractionScene.";
        }
    }
}
