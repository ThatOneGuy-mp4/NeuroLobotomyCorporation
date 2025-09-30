using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation
{
    /*
     * The ActionScene which will activate the commands from the Neuro SDK side. See that program's ActionScene class for more info.
     * The overridden ProcessServerInput will take the inputted commands, and call the correct method to execute them.
     * Each ActionScene on in this program should be associated with an ActionScene on the Connector's side. 
     */
    public abstract class ActionScene
    {
        public static ActionScene Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        private static ActionScene _instance;

        protected const int COMMAND_INDEX = 0;

        public abstract string ProcessServerInput(string[] message);
    }
}
