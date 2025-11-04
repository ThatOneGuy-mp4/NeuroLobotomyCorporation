using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.HokmaSuppression
{
    public class HokmaSuppressionScene : FacilityManagementScene
    {

        public override string ProcessServerInput(string[] message)
        {
            switch (message[COMMAND_INDEX])
            {
                case "get_day_start_context":
                    return GetDayStartContext();
                case "get_day_status":
                    return HokmaSuppression.GetDayStatus.Command();
            }
            return base.ProcessServerInput(message);
        }

        private string GetDayStartContext()
        {
            int currentDay = PlayerModel.instance.GetDay() + 1;
            int energyRequired = (int)StageTypeInfo.instnace.GetEnergyNeed(currentDay - 1);
            return string.Format("\"Do we truly need to change?\"" +
                "\n\nCore Suppression has begun on Day {0}." +
                "\nWarning!!! Hokma's Qlipha has manifested in the Record Department. Suppression will be complete once {1} P.E. Boxes have been generated and Qliphoth Meltdown Level Max has been achieved...in other words, when Qliphoth Meltdown Level 10 has been *completed*." +
                "\nAn anomaly with the flow of time has been detected. The price of silence must be paid upon pausing, and latency from our machines is being forcefully transfered to yours...you may noti" +
                "\n\n\"We must cherish each and every moment... Please do not let time flow meaninglessly.\"" +
                "\nce delays in your actions.", currentDay, energyRequired); //haha get it because the other half of the sentence was delayed. g.get it. whatever.
        }

        //Postfix - save the price of silence dialogue for later
        private static string silenceDialogue = "";
        public static void SavePriceOfSilenceDialogue(string text)
        {
            silenceDialogue = text;
        }

        //Postfix
        public static void InformNeuroPriceOfSilencePaid(ChokhmahBossBase __instance)
        {
            NeuroSDKHandler.SendContext(String.Format("'{0}'" +
                "\n{1} Agent(s) have paid the price of silence.", silenceDialogue, __instance.GetTargetAgentCount()), true);
            silenceDialogue = "";
        }
    }
}
