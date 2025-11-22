using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.DayPreparation
{
    public class DayPreparationScene : ActionScene
    {
        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "customize_agent":
                    return CustomizeAgent.Command(message[(int)CustomizeAgent.Parameters.AGENT_NAME], 
                        Int32.Parse(message[(int)CustomizeAgent.Parameters.R_VALUE]), Int32.Parse(message[(int)CustomizeAgent.Parameters.G_VALUE]), Int32.Parse(message[(int)CustomizeAgent.Parameters.B_VALUE]), 
                        Int32.Parse(message[(int)CustomizeAgent.Parameters.FRONT_HAIR]), Int32.Parse(message[(int)CustomizeAgent.Parameters.BACK_HAIR]), 
                        Int32.Parse(message[(int)CustomizeAgent.Parameters.EYE]), Int32.Parse(message[(int)CustomizeAgent.Parameters.EYEBROW]), Int32.Parse(message[(int)CustomizeAgent.Parameters.MOUTH]));
                case "activate_core_suppression":
                    return ActivateCoreSuppression.Command(message[(int)ActivateCoreSuppression.Parameters.SEPHIRAH_NAME]);
            }
            return "Command " + message[COMMAND_INDEX] + " does not exist in scene DayPreparationScene.";
        }
    }
}
