using NeuroSDKCsharp;
using NeuroSDKCsharp.Messages.API;
using NeuroSDKCsharp.Messages.Outgoing;
using NeuroSDKCsharp.Websocket;
using System;
using System.Collections.Generic;
using System.Configuration;
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

    public class Connector
    {
        public static string gameToServerURI = "";
        private static readonly string DEFAULT_GAME_TO_SERVER_URI = "http://localhost:8080/";

        public static string serverToGameURI = "";
        private static readonly string DEFAULT_SERVER_TO_GAME_URI = "http://localhost:8081";

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
            string? nURI = ConfigurationManager.AppSettings["NeuroURI"];
            if (!String.IsNullOrEmpty(nURI)) neuroURI = nURI;
            string? gtsURI = ConfigurationManager.AppSettings["GameToServerURI"];
            if (!String.IsNullOrEmpty(gtsURI))
            {
                if (gtsURI.StartsWith("http://") || gtsURI.StartsWith("https://")) gameToServerURI = gtsURI + "/";
                else Console.WriteLine("The GameToServerURI config option was not an http or https URI. Using http://localhost:8080 as a default.");
            }
            else Console.WriteLine("The GameToServerURI config option was not set. Using http://localhost:8080 as a default.");
            if (String.IsNullOrEmpty(gameToServerURI)) gameToServerURI = DEFAULT_GAME_TO_SERVER_URI;
            string? stgURI = ConfigurationManager.AppSettings["ServerToGameURI"];
            if (!String.IsNullOrEmpty(stgURI))
            {
                if (stgURI.StartsWith("http://") || stgURI.StartsWith("https://")) serverToGameURI = stgURI;
                else Console.WriteLine("The ServerToGameURI config option was not an http or https URI. Using http://localhost:8081 as a default.");
            }
            else Console.WriteLine("The ServerToGame config option was not set. Using http://localhost:8081 as a default.");
            if (String.IsNullOrEmpty(serverToGameURI)) serverToGameURI = DEFAULT_SERVER_TO_GAME_URI;
            SdkSetup.Initialize("Lobotomy", neuroURI);
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
                try
                {
                    await ProcessGameMessage(clientMessage);
                }
                catch(Exception e)
                {
                    Context.Send("An unknown, unhandled exception occurred while processing a game message.", true);
                }
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
                case "clear_actions":
                    ActionScene.CurrentActionScene.CleanUpActionScene();
                    break;
                case "enable_customize_agent":
                    if (parameters[1].Equals("true")) DayPreparationScene.CustomizeAgent.IsBongBong = true;
                    else DayPreparationScene.CustomizeAgent.IsBongBong = false;
                    ActionScene.CurrentActionScene.RegisterAction("customize_agent");
                    break;
                case "disable_customize_agent":
                    ActionScene.CurrentActionScene.UnregisterAction("customize_agent");
                    break;
                case "enable_activate_core_suppression":
                    if (parameters.Length > 1)
                    {
                        if (DayPreparationScene.ActivateCoreSuppression.SephirahReadyToSuppress.Count > 0) break;
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

        public enum FacilityManagementStartParams
        {
            IsBulletUnlocked = 2,
            DayStartContext = 3,
            BossPhase = 4, 
            AltPhase = 5
        }
        private static async Task ChangeCurrentActionScene(string[] parameters)
        {

            ActionScene nextScene = null;
            switch (parameters[(int)ProcessGameMessageParameters.Command_Parameter])
            {
                case "abnormality_extraction":
                    AbnormalityExtractionScene.ParseParameters(parameters);
                    nextScene = new AbnormalityExtractionScene();
                    break;
                case "watch_story":
                    nextScene = new WatchStoryScene();
                    break;
                case "day_preparation":
                    DayPreparationScene.ActivateCoreSuppression.SephirahReadyToSuppress.Clear();
                    nextScene = new DayPreparationScene();
                    break;
                case "facility_management":
                    nextScene = new FacilityManagementScene();
                    break;
                case "malkuth_suppression":
                    nextScene = new MalkuthSuppressionScene();
                    break;
                case "yesod_suppression":
                    nextScene = new YesodSuppressionScene();
                    break;
                case "hod_suppression":
                    nextScene = new HodSuppressionScene();
                    break;
                case "netzach_suppression":
                    nextScene = new NetzachSuppressionScene();
                    break;
                case "tiphereth_suppression":
                    nextScene = new TipherethSuppressionScene();
                    break;
                case "gebura_suppression":
                    nextScene = new GeburaSuppressionScene();
                    break;
                case "chesed_suppression":
                    nextScene = new ChesedSuppressionScene();
                    break;
                case "binah_suppression":
                    nextScene = new BinahSuppressionScene();
                    break;
                case "hokma_suppression":
                    nextScene = new HokmaSuppressionScene();
                    break;
                case "claw_suppression":
                    nextScene = new ClawSuppressionScene();
                    break;
                case "abel_suppression":
                    nextScene = new AbelSuppressionScene();
                    break;
                case "abram_suppression":
                    AbramSuppressionScene abram = new AbramSuppressionScene();
                    if (parameters.Length > 4)
                    {
                        abram.SetAltPhase(Int32.Parse(parameters[(int)FacilityManagementStartParams.AltPhase]));
                    }
                    nextScene = abram;
                    break;
                case "adam_suppression":
                    AdamSuppressionScene adam = new AdamSuppressionScene();
                    if (parameters.Length > 4)
                    {
                        adam.SetAltPhase(Int32.Parse(parameters[(int)FacilityManagementStartParams.AltPhase]));
                    }
                    nextScene = adam;
                    break;
                case "daat_suppression":
                    nextScene = new DaatSuppressionScene();
                    break;
            }
            if (nextScene != null && nextScene is FacilityManagementScene) (nextScene as FacilityManagementScene).ParseParameters(parameters);
            if (nextScene != null && nextScene is CoreSuppressionBaseScene && parameters.Length > 4) (nextScene as CoreSuppressionBaseScene).SetPhase(Int32.Parse(parameters[(int)FacilityManagementStartParams.BossPhase]));
            if (nextScene != null) ActionScene.ChangeActionScene(nextScene);
        }
    }
}
