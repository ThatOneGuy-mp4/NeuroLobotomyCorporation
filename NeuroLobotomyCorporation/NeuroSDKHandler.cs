using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NeuroLobotomyCorporation
{
    /*
     * The Unity SDK makes an object that isn't ever destroyed and handles connection to the game using Update.
     * And I thought wow that's a good idea for this project it's almost like this is a Unity game or something.
     * So this is essentially just that but in this project instead and using the HttpListener.
     */
    public class NeuroSDKHandler : MonoBehaviour
    {
        private static NeuroSDKHandler _instance;
        public static NeuroSDKHandler Instance
        {
            get
            {
                if (_instance == null) Debug.LogWarning("Accessed the SDK Handler without an instance being present.");
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public static HttpWebRequest GameOutput
        {
            get
            {
                return gameOutput;
            }
            set
            {
                gameOutput = value;
            }
        }
        private static HttpWebRequest gameOutput;

        public HttpListener ServerInput
        {
            get
            {
                if (serverInput == null) Debug.LogWarning("Accessed the ServerInput object while it was null.");
                return serverInput;
            }
            set
            {
                serverInput = value;
            }
        }
        private HttpListener serverInput;

        private bool awaitingInputDone;
        private bool awaitingOutputDone;

        public List<string> QueuedCommands
        {
            get
            {
                if (queuedCommands == null) queuedCommands = new List<string>();
                return queuedCommands;
            }
        }
        private List<string> queuedCommands;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            awaitingInputDone = true;
            awaitingOutputDone = true;
            StartListeningForServer();
        }

        private void StartListeningForServer()
        {
            ServerInput = new HttpListener();
            ServerInput.Prefixes.Add(Harmony_Patch.serverToGameURI);
            ServerInput.Start();
        }

        private void Update()
        {
            if (awaitingInputDone)
            {
                awaitingInputDone = false;
                ServerInput.BeginGetContext(new AsyncCallback(ProcessServerInput), null);
            }
            if (awaitingOutputDone && QueuedCommands.Count > 0)
            {
                awaitingOutputDone = false;
                string command = QueuedCommands[0];
                QueuedCommands.Remove(command);
                ProcessGameOutput(command);
            }
        }

        private void ProcessServerInput(IAsyncResult result)
        {

            HttpListenerContext context = ServerInput.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            StreamReader input = new StreamReader(request.InputStream, request.ContentEncoding);
            string[] serverMessage = input.ReadToEnd().Split('|');
            request.InputStream.Close();
            input.Close();
            string actionResult = ActionScene.Instance.ProcessServerInput(serverMessage);
            if (actionResult == null) actionResult = "";
            using (HttpListenerResponse response = context.Response)
            {
                byte[] responseMessage = Encoding.UTF8.GetBytes(actionResult);
                response.ContentLength64 = responseMessage.Length;
                Stream output = response.OutputStream;
                output.Write(responseMessage, 0, responseMessage.Length);
                output.Close();
            }
            awaitingInputDone = true;
        }

        private void ProcessGameOutput(string command)
        {
            GameOutput = (HttpWebRequest)WebRequest.Create(Harmony_Patch.gameToServerURI);
            GameOutput.KeepAlive = true;
            GameOutput.Method = "POST";
            GameOutput.BeginGetRequestStream((asyncResult) =>
            {
                byte[] parsedMessage = Encoding.UTF8.GetBytes(command);
                Stream postStream = GameOutput.EndGetRequestStream(asyncResult);
                postStream.Write(parsedMessage, 0, parsedMessage.Length);
                postStream.Close();
                GameOutput.BeginGetResponse((asyncResult2) =>
                {
                    using (HttpWebResponse response = (HttpWebResponse)GameOutput.EndGetResponse(asyncResult2))
                    {
                        //nothing lule. just doing this to dispose of the response.
                    }
                    awaitingOutputDone = true;
                }, asyncResult);
            }, command);
        }

        public static void SendCommand(string command)
        {
            NeuroSDKHandler.Instance.QueuedCommands.Add(command);
        }

        public static void SendContext(string message, bool silent = false)
        {
            string fullCommand = "send_context|" + message + "|" + silent.ToString();
            SendCommand(fullCommand);
        }
    }
}
