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

        private bool awaitingDone;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            awaitingDone = true;
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
            if (awaitingDone)
            {
                awaitingDone = false;
                ServerInput.BeginGetContext(new AsyncCallback(ProcessServerInput), null);
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
            awaitingDone = true;
        }
    }
}
