using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class DEBUGFuckingKillsYou
    {
        public enum Parameters
        {
            GUY_TO_FUCKING_KILL = 1,
        }
        public static string Command(string guyToFuckingKill)
        {
            UnitModel guy = Helpers.TryFindAnySuppressibleTarget(guyToFuckingKill, SefiraEnum.DUMMY);
            if (guy == null) return "failure|wasn't killed";
            if(guy is SefiraBossCreatureModel)
            {
                (guy as SefiraBossCreatureModel).TakeDamage(null, new DamageInfo(RwbpType.P, 1000));
                return "success|probably fucking killed idk";
            }
            //guy.hp -= 99999999;
            guy.TakeDamage(new DamageInfo(RwbpType.A, 99999));
            return "success|fucking killed";
        }
    }
}
