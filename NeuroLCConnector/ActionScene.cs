using NeuroSDKCsharp.Actions;
using NeuroSDKCsharp.Json;
using NeuroSDKCsharp.Messages.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLCConnector
{

    /* 
     * Given how the SDK and game have no direct connection, 
     * unregistering and registering every action individually every time the game 'phase' changes would be a pain.
     * Instead, we create ActionScenes, which contain all the possible Actions possible in a given situation,
     * then, we can just initialize and clean up that so phase changes can be done in one simple command
     * and the game can't register commands at a time it wouldn't work. 
     * Note that despite the name, these are not one-to-one with the current Unity Scene! 
     * A single Scene may contain several ActionScenes if they possess several distinct phases with incompatible commands,
     * or multiple versions of an ActionScene could be used in the same Scene to provide small changes
     * (e.g., all of the Core Suppressions compared to their base class, FacilityManagementScene).
     */

    /*
     * TODO:
     * -Add ActionScene for watching the story (either just to make sure she doesn't have another scene's actions avaliable while ved is reading, or to let her read or choose dialogue)
     * -Add ActionScene for department expansion (same reason as above, but for choosing the next department)
     * -Add ActionScene for pre-management preparation 
     * --Add a similar scene specific to E.G.O. equipping? 
     */
    public abstract class ActionScene
    {
        public static ActionScene CurrentActionScene
        {
            get
            {
                return currentActionScene;
            }
            set
            {
                currentActionScene = value;
            }
        }
        private static ActionScene currentActionScene;

        protected abstract List<INeuroAction> InitActions
        {
            get;
        }

        protected abstract List<INeuroAction> AllPossibleActions
        {
            get;
        }
        protected List<INeuroAction> allPossibleActions;

        protected List<INeuroAction> RegisteredActions = new List<INeuroAction>();

        public virtual void InitializeActionScene()
        {
            NeuroActionHandler.RegisterActions(InitActions);
            RegisteredActions.AddRange(InitActions);
            //TODO: add a virtual method that registers optional scene start commands here. so that actions don't get registered after scene start context is sent.
            string startContext = GetActionSceneStartContext();
            if (!String.IsNullOrEmpty(startContext)) Context.Send(startContext);
        }

        public virtual void CleanUpActionScene()
        {
            NeuroActionHandler.UnregisterActions(RegisteredActions);
            RegisteredActions.Clear();
        }

        protected abstract string GetActionSceneStartContext();

        public void RegisterAction(string actionName)
        {
            INeuroAction? neuroAction = AllPossibleActions.Find((INeuroAction NA) => NA.Name.Equals(actionName));
            if(neuroAction == null || neuroAction.GetType() == typeof(INeuroAction))
            {
                Console.WriteLine("Action " + actionName + " does not exist in the current scene, skipping.");
                return;
            }
            if (RegisteredActions.Contains(neuroAction))
            {
                Console.WriteLine("Action " + actionName + " is already registered in the current scene, skipping.");
                return;
            }
            NeuroActionHandler.RegisterActions(neuroAction);
            RegisteredActions.Add(neuroAction);
        }

        public void UnregisterAction(string actionName)
        {
            INeuroAction? neuroAction = RegisteredActions.Find((INeuroAction NA) => NA.Name.Equals(actionName));
            if (neuroAction == null || neuroAction.GetType() == typeof(INeuroAction))
            {
                Console.WriteLine("Action " + actionName + " is not registered in the current scene, skipping.");
                return;
            }
            NeuroActionHandler.UnregisterActions(neuroAction);
            RegisteredActions.Remove(neuroAction);
        }

        public static void ChangeActionScene(ActionScene scene)
        {
            if(CurrentActionScene != null) CurrentActionScene.CleanUpActionScene();
            CurrentActionScene = scene;
            CurrentActionScene.InitializeActionScene();
        }
    }
}
