using Customizing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NeuroLobotomyCorporation.DayPreparation
{
    public class CustomizeAgent
    {
        public enum Parameters
        {
            AGENT_NAME = 1,
            R_VALUE = 2,
            G_VALUE = 3,
            B_VALUE = 4,
            FRONT_HAIR = 5, 
            BACK_HAIR = 6,
            EYE = 7,
            EYEBROW = 8,
            MOUTH = 9
        }

        public static string Command(string agentName, int rValue, int gValue, int bValue, int frontHairIndex, int backHairIndex, int eyeIndex, int eyebrowIndex, int mouthIndex)
        {
            if (CustomizingWindow.CurrentWindow == null || CustomizingWindow.CurrentWindow.CurrentData == null || CustomizingWindow.CurrentWindow.appearanceUI == null) return "";
            customizedAppearance = new AppearanceState(agentName, rValue, gValue, bValue, frontHairIndex, backHairIndex, eyeIndex, eyebrowIndex, mouthIndex);
            return "";
        }

        private class AppearanceState
        {
            public string AgentName { get; private set; }

            public int RValue { get; private set; }

            public int GValue { get; private set; }

            public int BValue { get; private set; }

            public int FrontHairIndex { get; private set; }

            public int BackHairIndex { get; private set; }

            public int EyeIndex { get; private set; }

            public int EyebrowIndex { get; private set; }

            public int MouthIndex { get; private set; }

            public AppearanceState(string agentName, int rValue, int gValue, int bValue, int frontHairIndex, int backHairIndex, int eyeIndex, int eyebrowIndex, int mouthIndex)
            {
                AgentName = agentName;
                RValue = rValue;
                GValue = gValue;
                BValue = bValue;
                FrontHairIndex = frontHairIndex;
                BackHairIndex = backHairIndex;
                EyeIndex = eyeIndex;
                EyebrowIndex = eyebrowIndex;
                MouthIndex = mouthIndex;
            }
        }
        private static AppearanceState customizedAppearance = null;

        //Postfix - setting the name causes a memory access violation. do it in a patch. as per the usual.
        public static void SetCustomAppearance(CustomizingWindow __instance)
        {
            if (customizedAppearance == null) return;
            if (__instance == null || __instance.CurrentData == null || __instance.appearanceUI == null) return;
            AppearanceUI currentAppearance = __instance.appearanceUI;
            currentAppearance.NameInput.text = customizedAppearance.AgentName;
            if (customizedAppearance.RValue != -1) currentAppearance.palette.OnRedInputUpdate(customizedAppearance.RValue.ToString());
            if (customizedAppearance.GValue != -1) currentAppearance.palette.OnGreenInputUpdate(customizedAppearance.GValue.ToString());
            if (customizedAppearance.BValue != -1) currentAppearance.palette.OnBlueInputUpdate(customizedAppearance.BValue.ToString());
            currentAppearance.palette.Changed(727f); //parameter is completely useless
            if (customizedAppearance.FrontHairIndex != -1) currentAppearance.frontHair.OnClickMovement(currentAppearance.frontHair.CurrentIndex - customizedAppearance.FrontHairIndex);
            if (customizedAppearance.BackHairIndex != -1) currentAppearance.rearHair.OnClickMovement(currentAppearance.rearHair.CurrentIndex - customizedAppearance.BackHairIndex);
            if (customizedAppearance.EyeIndex != -1) currentAppearance.eye_Def.OnClickMovement(currentAppearance.eye_Def.CurrentIndex - customizedAppearance.EyeIndex);
            if (customizedAppearance.EyebrowIndex != -1) currentAppearance.eyebrow_Def.OnClickMovement(currentAppearance.eyebrow_Def.CurrentIndex - customizedAppearance.EyebrowIndex);
            if (customizedAppearance.MouthIndex != -1) currentAppearance.mouth_Def.OnClickMovement(currentAppearance.mouth_Def.CurrentIndex - customizedAppearance.MouthIndex);

            if (customizedAppearance.AgentName.Equals("BongBong")) //we must ensure no aspect of bongbong is left behind, even though Neuro cannot manipulate
            {
                currentAppearance.eye_Panic.OnClickMovement(currentAppearance.eye_Panic.CurrentIndex - 4);
                currentAppearance.eye_Dead.OnClickMovement(currentAppearance.eye_Dead.CurrentIndex - 1);
                currentAppearance.eyebrow_Battle.OnClickMovement(currentAppearance.eyebrow_Battle.CurrentIndex - 7);
                currentAppearance.eyebrow_Panic.OnClickMovement(currentAppearance.eyebrow_Panic.CurrentIndex - 5);
                currentAppearance.mouth_Battle.OnClickMovement(currentAppearance.mouth_Battle.CurrentIndex - 7);
            }
            customizedAppearance = null;
        }
    }
}
