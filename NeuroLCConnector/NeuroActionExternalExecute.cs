using NeuroSDKCsharp.Actions;
using NeuroSDKCsharp.Json;
using NeuroSDKCsharp.Websocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{
    /*
     * The way the Neuro SDK is implemented for this project is such that this program, 
     * which handles the connection to Neuro, has no way to view any data contained in the game itself. 
     * To validate any action, a web request needs to be made to the game, and returned to this program.
     * But at that point, having the Execute method do anything would mean making *another* request to the game,
     * which results in more of...the L word.
     * Instead, it makes more sense to just have the Execute method implemented on the game's side,
     * and queue that when the validation is successful. 
     * All that to say, this class, which extends NeuroAction, just exists to remove that pesky Execute method and simplify the Validate method.
     */
    public abstract class NeuroActionExternalExecute : NeuroAction
    {

        /*
         * The overriden Validate method will perform any immediately possible validation (e.g., empty or null variables),
         * and then pass the validated actionData parameters to this method, which send the validation to the game.
         */
        private const int IS_VALID_INDEX = 0;
        private const int RESULT_MESSAGE_INDEX = 1;
        protected ExecutionResult ValidateGameSide(params string[] actionParams)
        {
            string returnedValidation = NeuroLCConnector.Connector.SendCommand(Name + "|" + String.Join('|', actionParams)).Result;
            string[] returnedValidationSplit = returnedValidation.Split('|');
            if (returnedValidationSplit[IS_VALID_INDEX].Equals("failure")) return ExecutionResult.Failure(returnedValidationSplit[RESULT_MESSAGE_INDEX]);
            return ExecutionResult.Success(returnedValidationSplit[RESULT_MESSAGE_INDEX]);
        }

        protected sealed override Task Execute()
        {
            return new Task(() =>
            {

            });
        }
    }

    /*
     * For actions which require no validation it does not make sense to do the validation on the game's side. Surprisingly.
     * So this one immediately returns success so Neuro can respond and then the Execute method sends the Execution to the game.
     */
    public abstract class NeuroActionNoValidation : NeuroAction
    {
        protected abstract string SuccessMessage { get; }

        protected sealed override JsonSchema? Schema => new()
        {
            Type = JsonSchemaType.Null
        };

        protected sealed override ExecutionResult Validate(ActionData actionData)
        {
            return ExecutionResult.Success(SuccessMessage);
        }

        protected sealed override Task Execute()
        {
            string result = NeuroLCConnector.Connector.SendCommand(Name).Result;
            if (!String.IsNullOrEmpty(result)) result = String.Format("discard|{0}|true", result); 
            return NeuroLCConnector.Connector.SendContext(result.Split('|'));
        }
    }
}
