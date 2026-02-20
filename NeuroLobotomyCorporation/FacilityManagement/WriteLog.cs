using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class WriteLog
    {
        public const int LOG_TEXT = 1;
        public static string Command(string logText)
        {
            try
            {
                WriteTextToLog(logText);
            }
            catch (Exception e)
            {
                return "failure|An error occurred while writing your log.";
            }
            return "success|Your log was successfully recorded.";
        }

        private static List<string> neuroLogs = new List<string>();
        private static List<string> backupNeuroLogs = new List<string>();
        public static List<string> GetNeuroLogs()
        {
            return neuroLogs;
        }

        private static List<string> allLogs = new List<string>();
        private static List<string> backupAllLogs = new List<string>();
        public static List<string> GetAllLogs()
        {
            return allLogs;
        }

        public static LogItemScript WriteTextToLog(string logText)
        {
            SystemLogScript logScript = GameObject.FindObjectOfType<SystemLogScript>();
            LogItemScript neuroLog = logScript.script.MakeText("");
            neuroLog.SetText(String.Format("▶ <color={0}>({1})</color> {2}", GetColorCodeForSelectedAI(), NeuroSDKHandler.AiPlaying, logText));
            string cleanText = CleanLogOfColorCodes("▶ " + logText);
            TryStoreNeuroLog(cleanText);
            TryStoreAllLog(cleanText);
            return neuroLog;
        }

        //Postfix - store log information when they are made
        public static void StoreLogInformation(string context)
        {
            if (String.IsNullOrEmpty(context)) return;
            TryStoreAllLog(CleanLogOfColorCodes(context));
        }

        private static bool lockNeuroLogs = false;
        public static void TryStoreNeuroLog(string log)
        {
            if (!lockNeuroLogs)
            {
                neuroLogs.Add(log);
            }
            else
            {
                backupNeuroLogs.Add(log);
            }
        }

        public static void LockNeuroLogs()
        {
            lockNeuroLogs = true;
        }

        public static void UnlockNeuroLogs()
        {
            if (backupNeuroLogs.Count > 0)
            {
                neuroLogs.AddRange(backupNeuroLogs);
                backupNeuroLogs.Clear();
            }
            lockNeuroLogs = false;
        }

        private static bool lockAllLogs = false;
        public static void TryStoreAllLog(string log)
        {
            if (!lockAllLogs)
            {
                allLogs.Add(log);
            }
            else
            {
                backupAllLogs.Add(log);
            }
        }

        public static void LockAllLogs()
        {
            lockAllLogs = true;
        }

        public static void UnlockAllLogs()
        {
            if (backupAllLogs.Count > 0)
            {
                allLogs.AddRange(backupAllLogs);
                backupAllLogs.Clear();
            }
            lockAllLogs = false;
        }

        private static string GetColorCodeForSelectedAI()
        {
            if (NeuroSDKHandler.AiPlaying.Equals("Neuro")) return "#fea8ae";
            return "#711537";
        }

        private static string CleanLogOfColorCodes(string text)
        {
            while (text.Contains("<color="))
            {
                int start = text.IndexOf("<color=");
                text = text.Remove(start, 15); //<color=#XXXXXX>
            }
            while (text.Contains("</color>")) text = text.Replace("</color>", "");
            return text;
        }

        public static void ClearLogs()
        {
            neuroLogs.Clear();
            allLogs.Clear();
            backupNeuroLogs.Clear();
            backupAllLogs.Clear();
        }
    }
}
