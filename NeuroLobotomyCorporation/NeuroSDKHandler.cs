using NeuroLobotomyCorporation.FacilityManagement;
using NeuroLobotomyCorporation.WatchStory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
        private static NeuroSDKHandler _instance = null;
        public static NeuroSDKHandler Instance
        {
            get
            {
                if (_instance == null) UnityEngine.Debug.LogWarning("Accessed the SDK Handler without an instance being present.");
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
                if (serverInput == null) UnityEngine.Debug.LogWarning("Accessed the ServerInput object while it was null.");
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

        public static string gameToServerURI = "";
        private static readonly string DEFAULT_GAME_TO_SERVER_URI = "http://localhost:8080";

        public static string serverToGameURI = "";
        private static readonly string DEFAULT_SERVER_TO_GAME_URI = "http://localhost:8081/";

        public static string AiPlaying = "Neuro";

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            LoadConfig();
            awaitingInputDone = true;
            awaitingOutputDone = true;
            StartListeningForServer();
        }

        private static readonly string fileName = Application.dataPath + "/BaseMods/ThatOneGuy_NeuroLobotomyCorporation/ModConfig.xml";
        private static readonly string _rootName = "settings";
        private static readonly string _gameToServerUri = "gameToServerURI";
        private static readonly string _serverToGameUri = "serverToGameURI";
        private static readonly string _aiPlaying = "aiPlaying";
        private static readonly string _allowExecutionBullet = "allowExBullet";
        private static readonly string _disableLore = "disableLore";
        private static void LoadConfig()
        {
            if (!File.Exists(fileName)) return;
            XmlDocument configFile = new XmlDocument();
            configFile.LoadXml(File.ReadAllText(fileName));
            XmlNode settingsNode = configFile.SelectSingleNode(_rootName);
            if (settingsNode == null) return;
            XmlNode gtsURI = settingsNode.SelectSingleNode(_gameToServerUri);
            if (gtsURI != null)
            {
                if (gtsURI.InnerText.StartsWith("http://") || gtsURI.InnerText.StartsWith("https://")) gameToServerURI = gtsURI.InnerText;
                else gameToServerURI = DEFAULT_GAME_TO_SERVER_URI;
            }
            else gameToServerURI = DEFAULT_GAME_TO_SERVER_URI;
            XmlNode stgURI = settingsNode.SelectSingleNode(_serverToGameUri);
            if (stgURI != null)
            {
                if (stgURI.InnerText.StartsWith("http://") || stgURI.InnerText.StartsWith("https://")) serverToGameURI = stgURI.InnerText + "/";
                else serverToGameURI = DEFAULT_SERVER_TO_GAME_URI;
            }
            else serverToGameURI = DEFAULT_SERVER_TO_GAME_URI;

            XmlNode aiPlaying = settingsNode.SelectSingleNode(_aiPlaying);
            if(aiPlaying != null)
            {
                if (!aiPlaying.InnerText.Equals("Neuro") && !aiPlaying.InnerText.Equals("Evil")) AiPlaying = "Neuro";
                else AiPlaying = aiPlaying.InnerText;
            }
            
            XmlNode allowExBullet = settingsNode.SelectSingleNode(_allowExecutionBullet);
            ShootManagerialBullet.CanUseExecutionBullets = false;
            if(allowExBullet != null)
            {
                if (allowExBullet.InnerText.ToLower().Equals("true")) ShootManagerialBullet.CanUseExecutionBullets = true;
            }

            XmlNode disableLore = settingsNode.SelectSingleNode(_disableLore);
            if (disableLore != null)
            {
                if (disableLore.InnerText.ToLower().Equals("true"))
                {
                    WatchStory.IntegrationLore.DisableLore = true;
                    if(String.IsNullOrEmpty(AiPlaying)) AiPlaying = "Neuro";
                }
            }

        }

        public static void SetAIPlaying(string aiName)
        {
            AiPlaying = aiName;

            if (!File.Exists(fileName)) return;
            XmlDocument configFile = new XmlDocument();
            configFile.LoadXml(File.ReadAllText(fileName));
            XmlNode settingsNode = configFile.SelectSingleNode(_rootName);
            if (settingsNode == null) return;

            XmlNode aiPlaying = settingsNode.SelectSingleNode(_aiPlaying);
            if (aiPlaying != null)
            {
                aiPlaying.InnerText = aiName;
                configFile.Save(fileName);
            }
        }

        private void StartListeningForServer()
        {
            ServerInput = new HttpListener();
            ServerInput.Prefixes.Add(serverToGameURI);
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
            string actionResult = "";
            if (ActionScene.Instance != null && ActionScene.Instance is FacilityManagementScene &&
                ((EscapeUI.instance != null && EscapeUI.instance.IsOpened) || (OptionUI.Instance != null && OptionUI.Instance.IsEnabled)))
            {
                actionResult = "failure|Action was blocked due to the pause menu being open.";
            }
            else if (ActionScene.Instance != null)
            {
                try
                {
                    actionResult = ActionScene.Instance.ProcessServerInput(serverMessage);
                }
                catch(Exception e)
                {
                    actionResult = "failure|An unknown, unhandled error occurred while processing the command.";
                }
            }
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
            GameOutput = (HttpWebRequest)WebRequest.Create(gameToServerURI);
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

        public static void SendCommand(string command, bool force = false)
        {
            if (Instance == null) return;
            if (GlobalGameManager.instance == null || (GlobalGameManager.instance.gameMode == GameMode.TUTORIAL && !force)) return;
            NeuroSDKHandler.Instance.QueuedCommands.Add(command);
        }

        public static void SendContext(string message, bool silent = false, bool force = false)
        {
            string fullCommand = "send_context|" + message + "|" + silent.ToString();
            SendCommand(fullCommand, force);
        }

        private void OnApplicationQuit()
        {
            KillConnector();
        }

        private static Process ConnectorInstance = null;
        private static bool initialized = false;
        public static void InitializeSDK()
        {
            if (!initialized)
            {
                StartConnector();
                StartSDKHandler();
                SpecialBossReward.EvaluateSynchronizationStatus();
                initialized = true;
            }
        }

        private static void StartConnector()
        {
            if (ConnectorInstance != null) return;
            ConnectorInstance = new Process();
            ConnectorInstance.StartInfo.FileName = Application.dataPath + @"\BaseMods\ThatOneGuy_NeuroLobotomyCorporation\Connector\NeuroLCConnector.exe";
            ConnectorInstance.StartInfo.UseShellExecute = false;
            ConnectorInstance.StartInfo.CreateNoWindow = true;
            ConnectorInstance.Start();
        }

        private static void StartSDKHandler()
        {
            if (Instance != null) return;
            GameObject sdkHandler = new GameObject("NeuroSDKHandler");
            sdkHandler.AddComponent<NeuroSDKHandler>();
        }

        private static void KillConnector()
        {
            if (ConnectorInstance == null) return;
            ConnectorInstance.Kill();
            ConnectorInstance = null;
        }

        private static void KillSDKHandler()
        {
            if (Instance == null) return;
            Instance.ServerInput.Stop();
            Instance.QueuedCommands.Clear();
            UnityEngine.Object.Destroy(Instance.gameObject);
            Instance = null;
        }

        //Postfix - use the special mod commands
        public static void SDKConsoleCommand(string command)
        {
            string[] parameters = command.Split(' ');
            if (parameters.Length < 2 || !parameters[0].Equals("neurosdk")) return;
            switch (parameters[1])
            {
                case "start":
                    if (parameters.Length < 3) return;
                    CommandStart(parameters[2]);
                    return;
                case "kill":
                    if (parameters.Length < 3) return;
                    CommandKill(parameters[2]);
                    return;
                case "restart":
                    if (parameters.Length < 3) return;
                    CommandRestart(parameters[2]);
                    return;
                case "regaincontrol":
                    IntegrationLore.DecreaseNeuroResponseState();
                    IntegrationLore.DecreaseNeuroResponseState();
                    return;
            }
        }

        //"neurosdk start {connector/handler/all}"
        private static void CommandStart(string target)
        {
            if(target.Equals("connector") || target.Equals("all"))
            {
                StartConnector();
            }
            if (target.Equals("handler") || target.Equals("all"))
            {
                StartSDKHandler();
            }
        }

        //"neurosdk kill {connector/handler/all}"
        private static void CommandKill(string target)
        {
            if (target.Equals("connector") || target.Equals("all"))
            {
                KillConnector();
            }
            if (target.Equals("handler") || target.Equals("all"))
            {
                KillSDKHandler();
            }
        }

        //"neurosdk restart {connector/handler/all}"
        private static void CommandRestart(string target)
        {
            CommandKill(target);
            CommandStart(target);
            if((target.Equals("connector") || target.Equals("all")) && ActionScene.Instance != null)
            {
                NeuroSDKHandler.SendCommand(ActionScene.Instance.RestartConnectorCommand());
            }
        }
    }
}
