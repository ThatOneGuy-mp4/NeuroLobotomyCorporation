using NeuroSDKCsharp.Messages.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public abstract class CoreSuppressionBaseScene : FacilityManagementScene
    {
        protected int phase = 0;

        //The first element of this list will always be ignored, and should be assigned as such.
        protected abstract List<string> PhaseTransitionDialogue
        {
            get;
        }

        protected abstract string SuppressionCompleteDialogue
        {
            get;
        }

        public virtual async Task ChangePhase()
        {
            phase++;
            if(phase < PhaseTransitionDialogue.Count)
            {
                Context.Send(PhaseTransitionDialogue[phase], true);
            }
        }

        public async Task CoreSuppressionComplete()
        {
            CleanUpActionScene();
            Context.Send(SuppressionCompleteDialogue);
        }
    }
}
