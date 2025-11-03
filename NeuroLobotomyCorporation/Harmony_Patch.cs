using Harmony;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Collections;
using System.IO.Pipes;
using UnityEditor;
using System.Net;
using UnityEngine.SceneManagement;
using System.Text;
using CreatureSelect;
using System.Net.Sockets;
using NeuroLobotomyCorporation.FacilityManagement;
using GlobalBullet;
using NeuroLobotomyCorporation.YesodSuppression;
using NeuroLobotomyCorporation.HodSuppression;
using NeuroLobotomyCorporation.MalkuthSupression;
using NeuroLobotomyCorporation.NetzachSuppression;
using Rabbit;

namespace NeuroLobotomyCorporation
{
    public class Harmony_Patch
    {
        //TODO: Make this settable somewhere else later
        public static string gameToServerURI = "http://localhost:8080";
        //TODO: this too
        public static string serverToGameURI = "http://localhost:8081/";

        public Harmony_Patch()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("ThatOneGuy.NeuroLobotomyCorporation");
            harmonyInstance.Patch(typeof(NewTitleScript).GetMethod("Update", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("InitializeSDK")), null);
            harmonyInstance.Patch(typeof(CreatureSelectUI).GetMethod("Init", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("AbnormalityChoiceSelectStart")), null);
            harmonyInstance.Patch(typeof(DeployUI).GetMethod("OnManagementStart", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("BeginFacilityManagement", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(LoggingScript).GetMethod("MakeText", AccessTools.all), null,
                new HarmonyMethod(typeof(AssignWork).GetMethod("SortSystemLogs", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(SystemLogScript).GetMethod("OnNotice", AccessTools.all), null,
                new HarmonyMethod(typeof(AssignWork).GetMethod("UpdateNeuroLogsObservationLevel", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(GlobalBulletWindow).GetMethod("Update", AccessTools.all), null,
                new HarmonyMethod(typeof(ShootManagerialBullet).GetMethod("NeuroShootBullet", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(SefiraBossManager).GetMethod("OnOverloadActivated", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("ChangeBossPhaseMeltdown", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(SefiraBossBase).GetMethod("OnCleared", AccessTools.all), null,
                new HarmonyMethod(typeof(Harmony_Patch).GetMethod("BossCleared", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(IsolateRoom).GetMethod("Update", AccessTools.all), null,
                new HarmonyMethod(typeof(CancelAction).GetMethod("CancelChannelledTool", AccessTools.all)), null);
            //The FacilityManagement Context Zone
            harmonyInstance.Patch(typeof(AngelaConversation).GetMethod("MakeMessage", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAngelaMessage", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(AgentModel).GetMethod("OnDie", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAgentDied", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(AgentModel).GetMethod("Panic", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAgentPanicked", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(AgentModel).GetMethod("LoseControl", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAgentBecomesUncontrollable", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(AgentModel).GetMethod("StopPanic", AccessTools.all), 
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAgentRecoverPanic", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(AgentModel).GetMethod("GetControl", AccessTools.all),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAgentBecomesControllable", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(CreatureModel).GetMethod("UpdateQliphothCounter", AccessTools.all),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAbnormalityQliphothCounterDroppedToZero", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(CreatureModel).GetMethod("Escape", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAbnormalityEscaped", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(ChildCreatureModel).GetMethod("Escape", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroChildSpawned", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(CreatureModel).GetMethod("Suppressed", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroAbnormalitySuppressed", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(PlayerModel.EmergencyController).GetMethod("SetLevel", AccessTools.all), new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroPrefixTrumpetLevelChanged", AccessTools.all)),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroPostfixTrumpetLevelChanged", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(CreatureModel).GetMethod("ActivateOverload", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroOverloadActivatedOutsideOfMeltdown", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(OrdealBase).GetMethod("OrdealTypo", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroOrdealStartedAndEnded", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(RabbitCaptaionConversation).GetMethod("GetText", AccessTools.all), null,
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroRabbitsDialogue", AccessTools.all)), null);
            harmonyInstance.Patch(typeof(RabbitProtocolWindow).GetMethod("OnClickCommand", AccessTools.all), new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroPreRabbitsDeployed", AccessTools.all)),
                new HarmonyMethod(typeof(FacilityManagementScene).GetMethod("InformNeuroPostRabbitsDeployed", AccessTools.all)), null);
            //FacilityManagement context end
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

        }

        private static bool initialized = false;
        public static void InitializeSDK()
        {
            if (!initialized)
            {
                Process connector = new Process();
                //TODO: prolly make this settable somewhere else in case this is Not the user's (ved's) file structure
                connector.StartInfo.FileName = Application.dataPath + @"\BaseMods\ThatOneGuy_NeuroLobotomyCorporation\Connector\NeuroLCConnector.exe";
                connector.StartInfo.UseShellExecute = false;
                connector.StartInfo.CreateNoWindow = false;
                connector.Start();
                GameObject sdkHandler = new GameObject("NeuroSDKHandler");
                sdkHandler.AddComponent<NeuroSDKHandler>();
                initialized = true;
            }
        }

        public static void AbnormalityChoiceSelectStart(CreatureSelectUI __instance)
        {
            string command = "change_action_scene|abnormality_extraction";
            foreach (CreatureSelectUnit unit in CreatureSelectUI.instance.Units)
            {
                if (unit.gameObject.activeSelf)
                {
                    command += "|" + unit.IdText.text;
                }
            }
            //TODO: add whether or not re-extraction will be possible here
            ActionScene.Instance = new AbnormalityExtraction.AbnormalityExtractionScene();
            NeuroSDKHandler.SendCommand(command);
        }

        public static void BeginFacilityManagement()
        {
            if (SefiraBossManager.Instance.IsAnyBossSessionActivated())
            {
                switch (SefiraBossManager.Instance.CurrentActivatedSefira)
                {
                    case SefiraEnum.MALKUT:
                        ActionScene.Instance = new MalkuthSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|malkuth_suppression");
                        break;
                    case SefiraEnum.YESOD:
                        ActionScene.Instance = new YesodSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|yesod_suppression");
                        break;
                    case SefiraEnum.HOD:
                        ActionScene.Instance = new HodSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|hod_suppression");
                        break;
                    case SefiraEnum.NETZACH:
                        ActionScene.Instance = new NetzachSuppressionScene();
                        NeuroSDKHandler.SendCommand("change_action_scene|netzach_suppression");
                        break;
                }
                return;
            }
            ActionScene.Instance = new FacilityManagementScene();
            NeuroSDKHandler.SendCommand("change_action_scene|facility_management");
        }

        public static void ChangeBossPhaseMeltdown()
        {
            if (!SefiraBossManager.Instance.IsAnyBossSessionActivated()) return;
            if (SefiraBossManager.Instance.CheckBossActivation(SefiraEnum.GEBURAH) || SefiraBossManager.Instance.CheckBossActivation(SefiraEnum.BINAH)) return;
            NeuroSDKHandler.SendCommand("change_boss_phase");
        }

        public static void BossCleared()
        {
            NeuroSDKHandler.SendCommand("boss_cleared");
        }
    }
}
