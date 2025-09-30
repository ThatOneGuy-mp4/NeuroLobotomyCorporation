using System.Diagnostics;
using NeuroSDKCsharp.Messages.API;
using NeuroSDKCsharp.Utilities;

namespace NeuroSDKCsharp.Websocket;

public class CommandHandler
{
    private readonly List<IIncomingMessageHandler> _handlers = new();

    private void Start() // this will need to be called manually as there is nothing similar
    {
        _handlers.AddRange(ReflectionHelpers.GetAllInDomain<IIncomingMessageHandler>());
    }

    public virtual void Handle(string command, IncomingData data)
    {
        if (_handlers.Count == 0) Start();
        try
        {
            foreach (IIncomingMessageHandler handler in _handlers)
            {
                Console.WriteLine($"Running CommandHandler foreach    {handler}");
                if (!handler.CanHandle(command)) continue;

                ExecutionResult validationResult;
                object? resultData;
                try
                {
                    Console.WriteLine($"CommandHandler running validation");
                    validationResult = handler.Validate(command, data, out resultData);
                }
                catch (Exception e)
                {
                    validationResult = ExecutionResult.Failure($"Issue with message handling {e.Message}");
                    Console.WriteLine(e);
                    resultData = null;  
                }

                if (!validationResult.Successful)
                {
                    Console.WriteLine("Unsuccessful execution result when handling message");
                }
            
                handler.ReportResult(resultData, validationResult);

                if (validationResult.Successful)
                {
                    Console.WriteLine($"CommandHandler validation successful");
                    handler.Execute(resultData);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Issue in Handle \n  {e}");
            throw;
        }
        
    }
}