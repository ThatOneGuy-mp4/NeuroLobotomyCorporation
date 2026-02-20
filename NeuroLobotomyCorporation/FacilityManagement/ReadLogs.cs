using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class ReadLogs
    {
        public enum Parameters
        {
            NUM_OF_LOGS = 1,
            NEURO_LOGS_ONLY = 2
        }

        public static string Command(int numOfLogs, bool neuroLogsOnly)
        {
            if (neuroLogsOnly)
            {
                if (WriteLog.GetNeuroLogs().Count == 0) return "failure|Action failed. You have not written any logs to read.";
                WriteLog.LockNeuroLogs();
                if(numOfLogs >= WriteLog.GetNeuroLogs().Count)
                {
                    ThreadPool.QueueUserWorkItem(CommandExecute, new ReadLogsState(numOfLogs, neuroLogsOnly));
                    return "success|Getting all of your logs as to send as context...";
                }
                ThreadPool.QueueUserWorkItem(CommandExecute, new ReadLogsState(numOfLogs, neuroLogsOnly));
                if(numOfLogs == 1) return "success|Getting the last log you wrote to send as context...";
                return String.Format("success|Getting the last {0} logs you wrote to send as context...", numOfLogs);
            }
            if (WriteLog.GetAllLogs().Count == 0) return "failure|Action failed. There are no system logs to read at this moment.";
            WriteLog.LockAllLogs();
            if(numOfLogs >= WriteLog.GetAllLogs().Count)
            {
                ThreadPool.QueueUserWorkItem(CommandExecute, new ReadLogsState(numOfLogs, neuroLogsOnly));
                return "success|Getting all logs as to send as context...";
            }
            ThreadPool.QueueUserWorkItem(CommandExecute, new ReadLogsState(numOfLogs, neuroLogsOnly));
            if (numOfLogs == 1) return "success|Getting the most recent log to send as context...";
            return String.Format("success|Getting the {0} most recent logs to send as context...", numOfLogs);
        }

        private class ReadLogsState
        {
            public int NumOfLogs { get; private set; }

            public bool NeuroLogsOnly { get; private set; }

            public ReadLogsState(int numOfLogs, bool neuroLogsOnly)
            {
                NumOfLogs = numOfLogs;
                NeuroLogsOnly = neuroLogsOnly;
            }
        }

        private static void CommandExecute(object state)
        {
            ReadLogsState readLogsState = (ReadLogsState)state;
            string finalResult = "";
            if (readLogsState.NeuroLogsOnly)
            {
                List<string> neuroList = WriteLog.GetNeuroLogs();
                for(int i = 1; i <= readLogsState.NumOfLogs; i++)
                {
                    finalResult = neuroList[neuroList.Count - i] + finalResult;
                    if (neuroList.Count - i != 0 && i != readLogsState.NumOfLogs) finalResult = "\n" + finalResult;
                    else break;
                }
                WriteLog.UnlockNeuroLogs();
                NeuroSDKHandler.SendContext(finalResult, true);
                return;
            }
            List<string> list = WriteLog.GetAllLogs();
            for (int i = 1; i <= readLogsState.NumOfLogs; i++)
            {
                finalResult = list[list.Count - i] + finalResult;
                if (list.Count - i != 0 && i != readLogsState.NumOfLogs) finalResult = "\n" + finalResult;
                else break;
            }
            WriteLog.UnlockAllLogs();
            NeuroSDKHandler.SendContext(finalResult, true);
        }
    }
}
