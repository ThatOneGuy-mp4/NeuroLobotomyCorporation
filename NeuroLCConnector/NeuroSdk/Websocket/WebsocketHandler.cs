using System.Net.WebSockets;
using System.Text;
using NeuroSDKCsharp.Json;
using NeuroSDKCsharp.Messages.API;
using Newtonsoft.Json.Linq;

namespace NeuroSDKCsharp.Websocket;

public class WebsocketHandler
{
    private bool _tryingReconnect;
    private const float ReconnectInterval = 30;
    
    private static WebsocketHandler? _instance;

    public WebsocketHandler(string gameName, string? uriString)
    {
        GameName = gameName;
        _messageQueue = new MessageQueue();
        _commandHandler = new CommandHandler();
        _uriString = uriString;
    }
    public static WebsocketHandler? Instance
    {
        get
        {
            if (_instance is null)
                Console.WriteLine("WebsocketHandlerInstance was accessed without an instance being present");
            return _instance;
        }
        private set => _instance = value;
    }

    //  ws://localhost:8001/ws/

    private ClientWebSocket? _webSocket = new ClientWebSocket();

    public readonly string GameName; // will be used for Messages
    private readonly MessageQueue _messageQueue;
    private readonly CommandHandler _commandHandler;

    private string? _uriString; // this will be changed to be able to be changed through file in future
    public async void Initialize()
    {
        try
        {
            await StartWs();
        }
        catch (Exception e)
        {
            Console.WriteLine($"issue in initialize: {e}");
        }
    }

    private async Task Reconnect(bool fromUpdate = false)
    {
        if (_tryingReconnect && fromUpdate) return;
        _tryingReconnect = true;
        await Task.Delay(TimeSpan.FromSeconds(ReconnectInterval));
        await StartWs();
    }
    
    private async Task StartWs()
    {
        Console.WriteLine("This is start of Ws");
        Instance = this;
        
        try
        {
            if (_webSocket!.State is WebSocketState.Open or WebSocketState.Connecting)
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Websocket has closed, as one is already open.", CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }


        if (_uriString is null or "")
        {
            _uriString = Environment.GetEnvironmentVariable("NEURO_SDK_WS_URL", EnvironmentVariableTarget.Process) ??
                         Environment.GetEnvironmentVariable("NEURO_SDK_WS_URL", EnvironmentVariableTarget.User) ??
                         Environment.GetEnvironmentVariable("NEURO_SDK_WS_URL", EnvironmentVariableTarget.Machine);
            Console.WriteLine($"UriString: {_uriString}");
        }
        
        if (_uriString is null or "")
        {
            string errorMessage =
                "Could not get websocket URL. You need to set the NEURO_SDK_WS_URL environment variable";

            Console.WriteLine(errorMessage);
            return;
        }

        _webSocket = new ClientWebSocket();
        Uri websocketUri = new Uri(_uriString);

        _webSocket.Options.KeepAliveInterval = TimeSpan.FromMinutes(10); // should substitute ping pong
        
        try
        {
            await _webSocket.ConnectAsync(websocketUri, CancellationToken.None);

            Console.WriteLine($"Starting Task    Websocket connected {_webSocket.State}");
            
            _ = Task.Run(ReceiveMessage);
            _tryingReconnect = false;
        }
        catch (Exception e)
        {
            if (e is WebSocketException we && we.WebSocketErrorCode is WebSocketError.Faulted)
            {
                Console.WriteLine($"Error code is {we.WebSocketErrorCode}  message: {we.Message}  error code: {we.ErrorCode}");
                _ = Reconnect();
            }
        }
    }

    private async Task SendTask(OutgoingMessageHandler handler)
    {
        Console.WriteLine($"running SendTask");

        WsMessage wsMessage = handler.GetWsMessage(); 
        
        Console.WriteLine($"wsMessage before serialize {wsMessage}");
        
        string message = JsonSerialize.Serialize(wsMessage);
        
        Console.WriteLine($"Sending the Ws Message {message}");

        var sendBytes = Encoding.UTF8.GetBytes(message);
        
        try
        {
            await _webSocket!.SendAsync(sendBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _messageQueue.Enqueue(handler);
        }
    }

    public void Send(OutgoingMessageHandler messageHandler) => _messageQueue.Enqueue(messageHandler);

    public async Task SendImmediate(OutgoingMessageHandler messageHandler)
    {
        string message = JsonSerialize.Serialize(messageHandler.GetWsMessage());

        if (_webSocket is null) return;
        
        if (_webSocket.State is not WebSocketState.Open)
        {
            Console.WriteLine($"Websocket is not open. Could not send message: {message}");
        }

        Console.WriteLine($"Sending Immediate message {message}");

        var sendBytes = Encoding.UTF8.GetBytes(message);
        await _webSocket!.SendAsync(sendBytes, WebSocketMessageType.Text, false, CancellationToken.None);
    }

    private async Task ReceiveMessage()
    {
        Console.WriteLine($"Start of ReceiveMessage");

        if (_webSocket is null) return;
        
        var buffer = new byte[1024 * 4];
        
        while (_webSocket.State == WebSocketState.Open)
        {
            Console.WriteLine($"message queue count : {_messageQueue.Count}");
            
            Console.WriteLine($"Current websocket state Start {_webSocket.State}");
            WebSocketReceiveResult result;
            MemoryStream memoryStream = new MemoryStream();
            do
            {
                result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
                memoryStream.Write(buffer,0,result.Count);
            } while (!result.EndOfMessage);
            
            Console.WriteLine($"Receive message result: {result} || {result.MessageType}  || {result.CloseStatus}");
            
            if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("Server closed connection.");
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                break;
            }

            var memoryStreamArray = memoryStream.ToArray();
            var messageData = Encoding.UTF8.GetString(memoryStreamArray,0,memoryStreamArray.Length);
            
            Console.WriteLine($"Running MessageData");
            GetMessage(messageData);
            Console.WriteLine($"After GetMessage");
        }

        if (_webSocket.State != WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,"",CancellationToken.None);
        }
    }

    public async void Update()
    {
        if (_webSocket is { State: WebSocketState.Closed or WebSocketState.Aborted or WebSocketState.CloseReceived or WebSocketState.CloseSent} or null) // Best I can get to OnClose event with default websocket :(
        {
            _ = Reconnect(true);
            return;
        }
        
        try
        {
            if (_webSocket is null) throw new NullReferenceException("Websocket was null.");
            
            if (_webSocket.State != WebSocketState.Open) return;

            while (_messageQueue.Count > 0)
            {
                OutgoingMessageHandler handler = _messageQueue.Dequeue()!;
                Console.WriteLine(handler);
                await SendTask(handler);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Issue in update of ws: {e}");
        }
    }
    
    private void GetMessage(string messageData)
    {
        try
        {
            Dictionary<string, object>? dataArray = ProcessJsonMessage(messageData);
            if (dataArray is null) return;
            // command data
            _commandHandler.Handle((string)dataArray["command"], (IncomingData)dataArray["data"]);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Catching error in GetMessage try   {e}");
        }
    }

    public Dictionary<string, object>? ProcessJsonMessage(string messageData)
    {
        // messageData = @"{\""command\"":\""action\"",\""data\"":{\""id\"":\""123\"",\""name\"":\""name\"",\""Description\"":\""This is Description\""}}";
        JObject message = JObject.Parse(messageData);
        
        string? command = message["command"]?.Value<string>();
        IncomingData data = new(message["data"]);

        if (command is null)
        {
            Console.WriteLine("Received command that could not be deserialized.");
            return new();
        }
        
        Dictionary<string, object> dataDictionary = new Dictionary<string, object>{{"message", message},{"command",command},{"data",data}};
        return dataDictionary;
    }
}