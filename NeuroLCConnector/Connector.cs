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
            switch (parameters[(int)ProcessGameMessageParameters.Command])
            {
                case "send_context":
                    await SendContext(parameters);
                    break;
                case "change_action_scene":
                    await ChangeCurrentActionScene(parameters);
                    break;
                case "change_boss_phase":
                    if (!(ActionScene.CurrentActionScene is CoreSuppressionBaseScene)) return;
                    await (ActionScene.CurrentActionScene as CoreSuppressionBaseScene).ChangePhase();
                    break;
                case "boss_cleared":
                    if (!(ActionScene.CurrentActionScene is CoreSuppressionBaseScene)) return;
                    await (ActionScene.CurrentActionScene as CoreSuppressionBaseScene).CoreSuppressionComplete();
                    break;
                case "give_poke":
                    if(ActionScene.CurrentActionScene is GeburaSuppressionScene)
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
            Context.Send(message, bool.Parse(parameters[(int)SendContextParameters.Silent]));
        }

        private static async Task ChangeCurrentActionScene(string[] parameters)
        {
            switch (parameters[(int)ProcessGameMessageParameters.Command_Parameter])
            {
                case "abnormality_extraction":
                    List<string> abnoNames = new List<string>();
                    //Loop through the names of the Abnormalities and add them to the name list before creating the scene
                    //Most scene changes do not require additional parameters prior to initialization but the extraction command lists the names, so
                    for (int i = 2; i < parameters.Length; i++)
                    {
                        abnoNames.Add(parameters[i]);
                    }
                    AbnormalityExtractionScene.AbnoNames = abnoNames;
                    ActionScene.ChangeActionScene(new AbnormalityExtractionScene());
                    break;
                case "facility_management":
                    ActionScene.ChangeActionScene(new FacilityManagementScene());
                    break;
                case "malkuth_suppression":
                    ActionScene.ChangeActionScene(new MalkuthSuppressionScene());
                    break;
                case "yesod_suppression":
                    ActionScene.ChangeActionScene(new YesodSuppressionScene());
                    break;
                case "hod_suppression":
                    ActionScene.ChangeActionScene(new HodSuppressionScene());
                    break;
                case "netzach_suppression":
                    ActionScene.ChangeActionScene(new NetzachSuppressionScene());
                    break;
                case "tiphereth_suppression":
                    ActionScene.ChangeActionScene(new TipherethSuppressionScene());
                    break;
                case "gebura_suppression":
                    ActionScene.ChangeActionScene(new GeburaSuppressionScene());
                    break;
                case "chesed_suppression":
                    ActionScene.ChangeActionScene(new ChesedSuppressionScene());
                    break;
            }
        }
    }
}
