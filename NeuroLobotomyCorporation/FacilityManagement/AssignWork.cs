using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class AssignWork
    {
        public enum Parameters
        {
            AGENT_NAME = 1,
            ABNORMALITY_NAME = 2,
            WORK_TYPE = 3
        }

        //TODO: add all the weird exceptions for special work types (der freischutz, red riding hood, el piano, one sin + whitenight, aib)
        public static string Command(string agentName, string abnoName, string workType)
        {
            AgentModel agent = null;
            CreatureModel abnormality = null;
            if (!Helpers.AgentExists(agentName, out agent)) return "failure|Action failed. Specified agent does not exist.";
            if (!Helpers.AbnormalityExists(abnoName, out abnormality)) return "failure|Action failed. Specified Abnormality does not exist.";
            if (abnormality.metaInfo.creatureWorkType == CreatureWorkType.KIT) return "failure|Action failed. Specified Abnormality is a Tool. Use the 'UseTool' command instead.";
            int workId = 0;
            switch (workType)
            {
                case "Instinct":
                    workId = 1;
                    break;
                case "Insight":
                    workId = 2;
                    break;
                case "Attachment":
                    workId = 3;
                    break;
                case "Repression":
                    workId = 4;
                    break;
                default:
                    return "failure|Action failed. Specified work type is not valid.";
            }
            switch (Helpers.GetAgentWorkingState(agent))
            {
                case Helpers.AgentWorkingState.WORKING:
                    return "failure|Work could not be assigned because the specified agent is already working.";
                case Helpers.AgentWorkingState.DEAD:
                    return "failure|Work could not be assigned because the specified agent is dead.";
                case Helpers.AgentWorkingState.PANICKING:
                    return "failure|Work could not be assigned because the specified agent is panicking.";
                case Helpers.AgentWorkingState.UNCONTROLLABLE:
                    return "failure|Work could not be assigned because the specified agent is uncontrollable.";
                case Helpers.AgentWorkingState.HERETIC:
                    if (!abnoName.Equals("One Sin and Hundreds of Good Deeds")) return "failure|Work could not be assigned because the specified agent is a heretic. They can do naught but hope someone will hear their confession.";
                    break;
            }
            switch (Helpers.GetAbnormalityWorkingState(abnormality))
            {
                case Helpers.AbnormalityWorkingState.BREACHING:
                    return "failure|Work could not be assigned because the specified Abnormality is currently breaching.";
                case Helpers.AbnormalityWorkingState.WORKING:
                    return "failure|Work could not be assigned because the specified Abnormality is already being worked on.";
                case Helpers.AbnormalityWorkingState.COOLDOWN:
                    return "failure|Work could not be assigned because the specified Abnormality is in cooldown. Try again in a bit.";
            }
            string specialResultMessage = "";
            bool specialResultSuccess = false;
            if(IsSpecialWork(agent, abnormality, workId, out specialResultMessage, out specialResultSuccess))
            {
                if (!specialResultSuccess) return String.Format("failure|{0}", specialResultMessage);
                if (abnormality.script is OneBadManyGood) workId = 6; //Confession
                else if (abnormality.script is Piano) workId = 7; //Play
                ThreadPool.QueueUserWorkItem(CommandExecute, new AssignWorkState(agent, abnormality, workId));
                return String.Format("success|{0}", specialResultMessage);
            }
            workId = SefiraBossManager.Instance.GetWorkId(workId);
            string workTypeAfterCheckingForMalkuth = "";
            switch (workId) //I hate this shit as much as you do, not my fault project moon forgot what an enum was
            {
                case 1:
                    workTypeAfterCheckingForMalkuth = "Instinct";
                    break;
                case 2:
                    workTypeAfterCheckingForMalkuth = "Insight";
                    break;
                case 3:
                    workTypeAfterCheckingForMalkuth = "Attachment";
                    break;
                case 4:
                    workTypeAfterCheckingForMalkuth = "Repression";
                    break;
                default:
                    workTypeAfterCheckingForMalkuth = "Unknown. Complain to the mod developer about it.";
                    break;
            }
            ThreadPool.QueueUserWorkItem(CommandExecute, new AssignWorkState(agent, abnormality, workId));
            return String.Format("success|{0} begins their {1} Work with {2}.", agentName, workTypeAfterCheckingForMalkuth, abnoName);
        }

        private static bool IsSpecialWork(AgentModel agent, CreatureModel abnormality, int workId, out string specialResultMessage, out bool specialResultSuccess)
        {
            specialResultMessage = "";
            specialResultSuccess = false;
            if (abnormality.script is OneBadManyGood)
            {
                //copied from OneBadManyGood's CheckDeathAngel
                CreatureModel creatureModel = CreatureManager.instance.FindCreature(100015L); 
                if(creatureModel != null && creatureModel.IsEscapedOnlyEscape())
                {
                    if (agent.HasUnitBuf(UnitBufType.DEATH_ANGEL_BETRAYER))
                    {
                        specialResultMessage = String.Format("{0} begins their Confession Assignment with One Sin and Hundreds of Good Deeds.", agent.GetUnitName());
                        specialResultSuccess = true;
                        return true;
                    }
                    else
                    {
                        specialResultMessage = "One Sin and Hundreds of Good Deeds knows a great sin has been committed. It does nothing but wait for your Confession.";
                        specialResultSuccess = false;
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        private class AssignWorkState
        {
            public AgentModel Agent{ get; private set; }

            public CreatureModel Abnormality{ get; private set; }

            public int WorkID{ get; private set; }

            public AssignWorkState(AgentModel agent, CreatureModel abnormality, int id)
            {
                Agent = agent;
                Abnormality = abnormality;
                WorkID = id;
            }
        }

        private static List<SystemLogScript.CreatureSystemLog> neuroCreatedLogItems = new List<SystemLogScript.CreatureSystemLog>();
        public static void CommandExecute(object state)
        {
            AssignWorkState parameters = (AssignWorkState)state;
            AgentModel agent = parameters.Agent;
            CreatureModel abnormality = parameters.Abnormality;
            int workId = parameters.WorkID;
            

            if(workId < 6) workId = SefiraBossManager.Instance.GetWorkId(workId); //randomize if not special
            SkillTypeInfo data = SkillTypeList.instance.GetData(workId);

            Sprite workSprite = null;
            if (workId < 6) workSprite = CommandWindow.CommandWindow.CurrentWindow.GetWorkSprite((RwbpType)workId); //Get sprite if work is not special
            else if (workId == 6) workSprite = CommandWindow.CommandWindow.CurrentWindow.Work_C; //Confession sprite
            else workSprite = CommandWindow.CommandWindow.CurrentWindow.Work_I; //Play sprite which might just be the insight sprite but whatever
            agent.ManageCreature(abnormality, data, workSprite);
            agent.counterAttackEnabled = false;
            abnormality.Unit.room.OnWorkAllocated(agent);
            abnormality.script.OnWorkAllocated(data, agent);

            SystemLogScript logScript = GameObject.FindObjectOfType<SystemLogScript>();
            string workTypeAfterCheckingForMalkuth = "";
            if (SefiraBossManager.Instance.CheckBossActivation(SefiraEnum.MALKUT) || SefiraBossManager.Instance.IsKetherBoss(KetherBossType.E1))
            {
                workTypeAfterCheckingForMalkuth = "Unknown";
            }
            else
            {
                workTypeAfterCheckingForMalkuth = data.calledName;
            }
            LogItemScript neuroLog = logScript.script.MakeText("");
            neuroLog.SetText(String.Format("▶  <color=#66bfcd>{0}</color> begins their <color=#84bd36>{1}</color> with <color=#ef9696>{2}</color>.", agent.GetUnitName(), workTypeAfterCheckingForMalkuth, abnormality.GetUnitName()));
            SystemLogScript.CreatureSystemLog neuroCreatureLog = GetNeuroCreatureSystemLog(abnormality);
            if (neuroCreatureLog == null)
            {
                neuroCreatureLog = new SystemLogScript.CreatureSystemLog(abnormality.instanceId);
                neuroCreatedLogItems.Add(neuroCreatureLog);
            }
            neuroCreatureLog.AddLog(neuroLog);
        }

        //All of the normal ways to put something in the system log crash the game and I have literally no idea why.
        //I found a workaround, however, said workaround does not sort the logs correctly. And I can't sort them myself because that also crashes the game.
        //So I'm using this harmony patch to sort them every time one is added. Because doing it with a harmony patch doesn't crash the game. Whatever man.
        public static void SortSystemLogs(LoggingScript __instance)
        {
            __instance.Sort();
        }

        //This one updates the logs when observation level increases. Because yes I have to do this manually too.
        //Pretty much stolen directly from SystemLogScript.
        public static void UpdateNeuroLogsObservationLevel(string notice, params object[] param)
        {
            if (NoticeName.CreatureObserveLevelAdded == notice)
            {
                CreatureModel creatureModel = param[0] as CreatureModel;
                if (creatureModel.observeInfo.GetObserveState(CreatureModel.regionName[0]))
                {
                    SystemLogScript.CreatureSystemLog creatureSystemLog = GetNeuroCreatureSystemLog(creatureModel);
                    if (creatureSystemLog != null)
                    {
                        creatureSystemLog.OnObserveLevelUpdated(creatureModel);
                    }
                }
            }
        }

        private static SystemLogScript.CreatureSystemLog GetNeuroCreatureSystemLog(CreatureModel abnormality)
        {
            foreach (SystemLogScript.CreatureSystemLog creatureSystemLog in neuroCreatedLogItems)
            {
                if (creatureSystemLog.targetId == abnormality.instanceId)
                {
                    return creatureSystemLog;
                }
            }
            return null;
        }
    }
}
