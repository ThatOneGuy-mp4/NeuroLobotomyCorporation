using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.DayPreparation
{
    public class ActivateCoreSuppression
    {
        public enum Parameters
        {
            SEPHIRAH_NAME = 1
        }

        public static string Command(string sephiraName)
        {
            SefiraEnum sefiraEnum = Patches.SefiraNameToEnum(sephiraName);
            if (sefiraEnum == SefiraEnum.DUMMY || !SefiraBossManager.Instance.IsBossStartable(sefiraEnum)) return "";
            SefiraPanel selectedPanel = null;
            foreach(DeploySefiraList.SefiraPanelData panel in DeployUI.instance.sefiraList.sefiraPanels)
            {
                if(panel.targetSefira == sefiraEnum) { selectedPanel = panel.script; break; }
            }
            if (selectedPanel == null) return "";
            selectedPanel.OnStartBossSession();
            return "";
        }
    }
}
