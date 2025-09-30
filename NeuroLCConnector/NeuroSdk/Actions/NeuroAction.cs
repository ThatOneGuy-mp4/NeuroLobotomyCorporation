using NeuroSDKCsharp.Websocket;

namespace NeuroSDKCsharp.Actions;

    public abstract class NeuroAction : BaseNeuroAction
    {
        protected NeuroAction()
        {
        }

        protected abstract ExecutionResult Validate(ActionData actionData);
        protected abstract Task Execute();

        protected sealed override ExecutionResult Validate(ActionData actionData, out object? resultData)
        {
            ExecutionResult result = Validate(actionData);
            resultData = null;
            return result;
        }

        protected sealed override void Execute(object? data) => Execute();
    }

    public abstract class NeuroAction<TData> : BaseNeuroAction
    {
        protected NeuroAction()
        {
        }

        protected abstract ExecutionResult Validate(ActionData actionData, out TData? resultData);
        protected abstract Task Execute(TData? resultData);

        protected sealed override ExecutionResult Validate(ActionData actionData, out object? resultData)
        {
            ExecutionResult result = Validate(actionData, out TData? tResultData);
            resultData = tResultData;
            return result;
        }

        protected sealed override void Execute(object? resultData) => Execute((TData?) resultData);
    }

    public abstract class NeuroActionS<TData> : NeuroAction<TData?> where TData : struct
    {
        protected NeuroActionS()
        {
        }
        
    }
