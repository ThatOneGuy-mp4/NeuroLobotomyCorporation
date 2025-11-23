using NeuroSDKCsharp;
using NeuroSDKCsharp.Messages.API;
using NeuroSDKCsharp.Messages.Outgoing;
using NeuroSDKCsharp.Websocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    /*
    * So essentially, Lobotomy Corporation is a spaghetti coded game that runs on Netframework3.5, 
    * which has the highest possible C# version of...C# version 3. Which is way too old to run the Neuro SDK.
    * Maybe it's somehow possible to convert it with that low of a C# version, but I don't know how, 
    * and I'm sure not gonna try when I don't understand half of what it's doing.
    * So instead...when the Neuro Modded Lobotomy Corporation starts up, it opens *this* program,
    * which *can* run the Neuro SDK (using the non-Unity C# implementation by pandapanda135, https://github.com/pandapanda135/CSharp-Neuro-SDK]).
    * Then the game and this program send commands to each other using web requests to actually run the integration.
    * This adds some latency but like. I could not for the life of me figure out another way to do it. So.
    */

    //TODO: probably include some methods to reconnect if a disconnect happens. 
    public class Connector
    {
        //TODO: Make this settable somewhere else later
        public static string gameToServerURI = "http://localhost:8080/";
        //TODO: this one too
        public static string serverToGameURI = "http://localhost:8081";

        //TODO: Make this settable as well, or don't, whatever
        public static string neuroURI = null;
        public static HttpListener GameInput
        {
            get
            {
                return gameInput;
            }
            set
            {
                gameInput = value;
            }
        }
        public static HttpListener gameInput;

        public static HttpWebRequest ServerOutput
        {
            get
            {
                return serverOutput;
            }
            set
            {
                serverOutput = value;
            }
        }
        private static HttpWebRequest serverOutput;

        public static void Main(string[] args)
        {
            Init();
            while (true)
            {
                //Keep the app alive
            }
        }

        public static async Task Init()
        {
            SdkSetup.Initialize("Lobotomy Corporation", neuroURI);
            try
            {
                GameInput = new HttpListener();
                GameInput.Prefixes.Add(gameToServerURI);
                GameInput.Start();
                GameInput.TimeoutManager.IdleConnection = new TimeSpan(7, 2, 7);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            WaitForGameInput();
            while (true)
            {
                await Loop();
            }
        }


        public static async Task Loop()
        {
            WebsocketHandler.Instance.Update();
        }

        public static async Task<string> SendCommand(string command)
        {
            if (ActionScene.CurrentActionScene != null && ActionScene.CurrentActionScene is HokmaSuppressionScene) await (ActionScene.CurrentActionScene as HokmaSuppressionScene).Stall();
            if (ActionScene.CurrentActionScene != null && ActionScene.CurrentActionScene is AdamSuppressionScene) await (ActionScene.CurrentActionScene as AdamSuppressionScene).Stall();
            ServerOutput = (HttpWebRequest)WebRequest.Create(serverToGameURI);
            ServerOutput.KeepAlive = true;
            ServerOutput.Method = "POST";
            Stream commandStream = await ServerOutput.GetRequestStreamAsync();
            byte[] parsedMessage = Encoding.UTF8.GetBytes(command);
            commandStream.Write(parsedMessage, 0, parsedMessage.Length);
            commandStream.Close();
            string responseString = "";
            using (HttpWebResponse gameResponse = (HttpWebResponse)await ServerOutput.GetResponseAsync())
            {
                Stream streamResponse = gameResponse.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);
                responseString = streamRead.ReadToEnd();
                streamResponse.Close();
                streamRead.Close();
            }
            return responseString;
        }

        public static async Task WaitForGameInput()
        {
            while (true)
            {
                HttpListenerContext context = await GameInput.GetContextAsync();
                HttpListenerRequest request = context.Request;
                StreamReader input = new StreamReader(request.InputStream, request.ContentEncoding);
                byte[] clientMessage = Encoding.UTF8.GetBytes(input.ReadToEnd());
                request.InputStream.Close();
                input.Close();
                using (HttpListenerResponse response = context.Response)
                {
                    //nothing lule. just using this to dispose of the response.
                }
                await ProcessGameMessage(clientMessage);
            }
        }

        private enum ProcessGameMessageParameters
        {
            Command = 0,
            Command_Parameter = 1
        }

        public static async Task ProcessGameMessage(byte[] message)
        {
            string parsedMessage = Encoding.UTF8.GetString(message);
            string[] parameters = parsedMessage.Split("|");
            string command = parameters[(int)ProcessGameMessageParameters.Command];
            if (ActionScene.CurrentActionScene == null && !(command.Equals("send_context") || command.Equals("change_action_scene"))) return;
            switch (command)
            {
                case "send_context":
                    await SendContext(parameters);
                    break;
                case "change_action_scene":
                    await ChangeCurrentActionScene(parameters);
                    break;
                case "enable_customize_agent":
                    ActionScene.CurrentActionScene.RegisterAction("customize_agent");
                    break;
                case "disable_customize_agent":
                    ActionScene.CurrentActionScene.UnregisterAction("customize_agent");
                    break;
                case "enable_activate_core_suppression":
                    if (parameters.Length > 1)
                    {
                        DayPreparationScene.ActivateCoreSuppression.SephirahReadyToSuppress = new List<string>();
                        for (int i = 1; i < parameters.Length; i++)
                        {
                            DayPreparationScene.ActivateCoreSuppression.SephirahReadyToSuppress.Add(parameters[i]);
                        }
                        Context.Send("A Sephirah's core requires suppression...");
                    }
                    ActionScene.CurrentActionScene.RegisterAction("activate_core_suppression");
                    break;
                case "disable_activate_core_suppression":
                    ActionScene.CurrentActionScene.UnregisterAction("activate_core_suppression");
                    break;
                case "change_boss_phase":
                    if (!(ActionScene.CurrentActionScene is CoreSuppressionBaseScene)) return;
                    await (ActionScene.CurrentActionScene as CoreSuppressionBaseScene).ChangePhase();
                    break;
                case "change_boss_phase_alt":
                    if (ActionScene.CurrentActionScene is AbramSuppressionScene) await (ActionScene.CurrentActionScene as AbramSuppressionScene).ChangeAltPhase();
                    if (ActionScene.CurrentActionScene is AdamSuppressionScene) await (ActionScene.CurrentActionScene as AdamSuppressionScene).ChangeAltPhase();
                    break;
                case "boss_cleared":
                    if (!(ActionScene.CurrentActionScene is CoreSuppressionBaseScene)) return;
                    await (ActionScene.CurrentActionScene as CoreSuppressionBaseScene).CoreSuppressionComplete();
                    break;
                case "boss_failed":
                    if (ActionScene.CurrentActionScene is AbelSuppressionScene) (ActionScene.CurrentActionScene as AbelSuppressionScene).FailCoreSuppression();
                    if (ActionScene.CurrentActionScene is AbramSuppressionScene) (ActionScene.CurrentActionScene as AbramSuppressionScene).FailCoreSuppression(); //i forgot to test this before moving onto day 49. go back and test that.
                    if (ActionScene.CurrentActionScene is AdamSuppressionScene) (ActionScene.CurrentActionScene as AdamSuppressionScene).FailCoreSuppression();
                    break;
                case "give_poke":
                    if (ActionScene.CurrentActionScene is GeburaSuppressionScene)
                    {
                        Task.Run(new Func<Task>((ActionScene.CurrentActionScene as GeburaSuppressionScene).GivePokeAction));
                    }
                    break;
                case "dont_touch_me_touched_event_start":
                    DontTouchMeTouchedEventStart();
                    break;
                case "dont_touch_me_touched_event_end":
                    DontTouchMeTouchedEventEnd();
                    break;
                case "start_dialogue_option":
                    (ActionScene.CurrentActionScene as WatchStoryScene).SetDialogueOptions(parameters);
                    ActionScene.CurrentActionScene.RegisterAction("select_dialogue");
                    break;
                case "dialogue_option_end":
                    ActionScene.CurrentActionScene.UnregisterAction("select_dialogue");
                    break;
                default:
                    Console.WriteLine("The command " + parameters[(int)ProcessGameMessageParameters.Command] + " was not found. Ensure it is spelt correctly on both game and server side.");
                    break;
            }
        }

        private static void DontTouchMeTouchedEventStart()
        {
            if (!(ActionScene.CurrentActionScene is FacilityManagementScene)) return;
            ActionScene.CurrentActionScene.CleanUpActionScene();
            Context.Send("O-05-47 has been pressed. An extremely strong urge to continue pressing it washes over you.", false);
            ActionScene.CurrentActionScene.RegisterAction("keep_pressing");
        }

        private static void DontTouchMeTouchedEventEnd()
        {
            ActionScene.CurrentActionScene.CleanUpActionScene();
            Context.Send("Pressing Don't Touch Me has triggered a forceful shutdown of all systems...terminating connection.", false);
        }

        private enum SendContextParameters
        {
            Message = 1,
            Silent = 2
        }
        public static async Task SendContext(string[] parameters)
        {
            if (parameters.Length < 3) return;
            string message = parameters[(int)SendContextParameters.Message];
            if (ActionScene.CurrentActionScene != null && ActionScene.CurrentActionScene is YesodSuppressionScene) message = (ActionScene.CurrentActionScene as YesodSuppressionScene).GetScrambledMessage(message);
            else if (ActionScene.CurrentActionScene != null && ActionScene.CurrentActionScene is AbelSuppressionScene) message = (ActionScene.CurrentActionScene as AbelSuppressionScene).GetScrambledMessage(message);
            Context.Send(message, bool.Parse(parameters[(int)SendContextParameters.Silent]));
        }

        public enum BossPhase
        {
            Phase = 2, 
            AltPhase = 3
        }
        private static async Task ChangeCurrentActionScene(string[] parameters)
        {
            switch (parameters[(int)ProcessGameMessageParameters.Command_Parameter])
            {
                case "abnormality_extraction":
                    AbnormalityExtractionScene.ParseParameters(parameters);
                    ActionScene.ChangeActionScene(new AbnormalityExtractionScene());
                    break;
                case "watch_story":
                    ActionScene.ChangeActionScene(new WatchStoryScene());
                    break;
                case "day_preparation":
                    ActionScene.ChangeActionScene(new DayPreparationScene());
                    break;
                case "facility_management":
                    ActionScene.ChangeActionScene(new FacilityManagementScene());
                    break;
                case "malkuth_suppression":
                    MalkuthSuppressionScene malkuth = new MalkuthSuppressionScene();
                    if (parameters.Length > 2) malkuth.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                    ActionScene.ChangeActionScene(malkuth);
                    break;
                case "yesod_suppression":
                    YesodSuppressionScene yesod = new YesodSuppressionScene();
                    if (parameters.Length > 2) yesod.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                    ActionScene.ChangeActionScene(yesod);
                    break;
                case "hod_suppression":
                    HodSuppressionScene hod = new HodSuppressionScene();
                    if (parameters.Length > 2) hod.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                    ActionScene.ChangeActionScene(hod);
                    break;
                case "netzach_suppression":
                    NetzachSuppressionScene netzach = new NetzachSuppressionScene();
                    if (parameters.Length > 2) netzach.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                    ActionScene.ChangeActionScene(netzach);
                    break;
                case "tiphereth_suppression":
                    TipherethSuppressionScene tiphereth = new TipherethSuppressionScene();
                    if (parameters.Length > 2) tiphereth.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                    ActionScene.ChangeActionScene(tiphereth);
                    break;
                case "gebura_suppression":
                    GeburaSuppressionScene gebura = new GeburaSuppressionScene();
                    if (parameters.Length > 2) gebura.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                    ActionScene.ChangeActionScene(gebura);
                    break;
                case "chesed_suppression":
                    ChesedSuppressionScene chesed = new ChesedSuppressionScene();
                    if (parameters.Length > 2) chesed.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                    ActionScene.ChangeActionScene(chesed);
                    break;
                case "binah_suppression":
                    BinahSuppressionScene binah = new BinahSuppressionScene();
                    if (parameters.Length > 2) binah.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                    ActionScene.ChangeActionScene(binah);
                    break;
                case "hokma_suppression":
                    HokmaSuppressionScene hokma = new HokmaSuppressionScene();
                    if (parameters.Length > 2) hokma.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                    ActionScene.ChangeActionScene(hokma);
                    break;
                case "claw_suppression":
                    ClawSuppressionScene claw = new ClawSuppressionScene();
                    if (parameters.Length > 2) claw.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                    ActionScene.ChangeActionScene(claw);
                    break;
                case "abel_suppression":
                    AbelSuppressionScene abel = new AbelSuppressionScene();
                    if (parameters.Length > 2) abel.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                    ActionScene.ChangeActionScene(abel);
                    break;
                case "abram_suppression":
                    AbramSuppressionScene abram = new AbramSuppressionScene();
                    if (parameters.Length > 2)
                    {
                        abram.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                        abram.SetAltPhase(Int32.Parse(parameters[(int)BossPhase.AltPhase]));
                    }
                    ActionScene.ChangeActionScene(abram);
                    break;
                case "adam_suppression":
                    AdamSuppressionScene adam = new AdamSuppressionScene();
                    if (parameters.Length > 2)
                    {
                        adam.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                        adam.SetAltPhase(Int32.Parse(parameters[(int)BossPhase.AltPhase]));
                    }
                    ActionScene.ChangeActionScene(adam);
                    break;
                case "daat_suppression":
                    DaatSuppressionScene daat = new DaatSuppressionScene();
                    if (parameters.Length > 2) daat.SetPhase(Int32.Parse(parameters[(int)BossPhase.Phase]));
                    ActionScene.ChangeActionScene(daat);
                    break;
            }
        }
    }
}
