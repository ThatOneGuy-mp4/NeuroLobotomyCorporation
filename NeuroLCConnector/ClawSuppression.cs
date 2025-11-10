using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class ClawSuppression : CoreSuppressionBaseScene
    {
        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "Dialogue? modCheck"
        };

        protected override string SuppressionCompleteDialogue => "Curtain Call for Act I of Keter's Core Suppression.";
    }
}
