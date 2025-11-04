using NeuroSDKCsharp.Messages.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    public class HokmaSuppressionScene : CoreSuppressionBaseScene
    {
        protected override List<string> PhaseTransitionDialogue => new List<string>
        {
            "UNUSED",
            "\"All of your loved ones are finally by your side now.\"" +
            "\n\nHokma's Qlipha has been agitated. The anomaly in the flow of time has intensified.",
            "\"We will not ever lose anyone, as long as the cycle repeats.\"" +
            "\n\nHokma's Qlipha has been agitated. The anomaly in the flow of time has intensified.",
            "\"Why are you trying to let us slip away?\"" +
            "\n\nHokma's Qlipha has been disturbed. The anomaly in the flow of time has intensified.",
            "\"I just wish to stay with you, everyone, and all that we have left in this eternal moment.\"" +
            "\n\nHokma's Qlipha has been agitated. The anomaly in the flow of time has intensified.",
            "\"Please do not steal away the last glimmer of what I treasure.\"" +
            "\n\nHokma's Qlipha has been agitated. The anomaly in the flow of time has intensified.",
            "\"You never knew when to stop, so I shall stop you with absolute certainty this time.\"" +
            "\n\nHokma's Qlipha has been disturbed. The anomaly in the flow of time has greatly intensified." +
            "\nHokma's core cannot sustain this level of corruption for much longer...please face the past as the last stretch of this suppression begins.",
            "\"No, I do not wish to change. I do not want to forget it all. Please, let’s just stay.\"" +
            "\n\nHokma's Qlipha has been disturbed. The anomaly in the flow of time has greatly intensified.",
            "\"I do not understand. What more must you sacrifice? Just what are you trying to achieve?\"" +
            "\n\nHokma's Qlipha has been disturbed. The anomaly in the flow of time has greatly intensified.",
            "\"I just cannot understand. Neither then can I accept it.\"" +
            "\n\nHokma's Qlipha has been disturbed. The anomaly in the flow of time has reached critical levels."
        };

        protected override string SuppressionCompleteDialogue => "\"You could move onward, in spite of everything...\"" +
            "\nHokma's Qlipha has vanished from the Record Department... Core Suppression successfully completed.";



        //vedal please don't ban for me this k thx
        private static readonly double BASE_LATENCY = 0.15;
        private static Random rand = new Random();
        public static async Task InduceLatency(int intensity)
        {
            double secondsToStall = BASE_LATENCY * intensity + BASE_LATENCY * intensity * rand.NextDouble();
            await Task.Delay((int)secondsToStall * 1000);
        }

        public async Task Stall()
        {
            await InduceLatency(phase);
        }
    }
}
