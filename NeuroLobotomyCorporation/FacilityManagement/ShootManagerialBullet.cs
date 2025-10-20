using GlobalBullet;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class ShootManagerialBullet
    {
        //TODO: make sure this works as intended. if it doesn't, just check if UnlockedBulletTypes is not an empty string.
        public static string IsBulletUnlocked()
        {
            return GlobalBulletWindow.CurrentWindow.ActiveControl.activeSelf.ToString().ToLower();
        }

        public enum Parameters
        {
            BULLET_TYPE = 1,
            TARGET_NAME = 2,
            TARGET_DEPARTMENT = 3
        }

        public static string Command(string bulletType, string targetName, string targetDepartment)
        {
            if (GlobalBulletManager.instance.currentBullet <= 0) return "failure|There are no bullets left to fire. Bullets will be reloaded when a Qliphoth Meltdown occurs.";
            UnitModel unit = null;
            AgentModel agent = null;
            if (Helpers.AgentExists(targetName, out agent))
            {
                unit = agent;
            }
            else
            {
                SefiraEnum sefiraDepartment = Helpers.GetSefiraByDepartment(targetDepartment);
                if (sefiraDepartment == SefiraEnum.DUMMY && !targetDepartment.Equals("DUMMY")) return "failure|Target's department was specified but was not valid.";
                unit = Helpers.TryFindAnySuppressableTarget(targetName, sefiraDepartment);
            }
            if (unit == null) return "failure|The specified target does not exist.";
            GlobalBulletType bullet = GlobalBulletType.NONE;
            switch (bulletType)
            {
                case "HP Recovery":
                    if (!ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RECOVER_HP)) return "failure|HP Recovery Bullets have not been researched yet. Continue completing Chesed's missions if you wish to make use of them.";
                    if (agent == null) return "failure|An Agent must be the target of an HP Recovery Bullet.";
                    bullet = GlobalBulletType.RECOVER_HP;
                    break;
                case "SP Recovery":
                    if (!ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RECOVER_MENTAL)) return "failure|SP Recovery Bullets have not been researched yet. Continue completing Chesed's missions if you wish to make use of them.";
                    if (agent == null) return "failure|An Agent must be the target of an SP Recovery Bullet.";
                    if (agent.IsPanic()) return "failure|SP Recovery Bullets cannot be fired at panicking Agents.";
                    bullet = GlobalBulletType.RECOVER_MENTAL;
                    break;
                case "Red Shield":
                    if (!ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RESIST_R)) return "failure|Red Shield Bullets have not been researched yet. Continue completing the Tiphereths' missions if you wish to make use of them.";
                    if (agent == null) return "failure|An Agent must be the target of a Red Shield Bullet.";
                    bullet = GlobalBulletType.RESIST_R;
                    break;
                case "White Shield":
                    if (!ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RESIST_W)) return "failure|White Shield Bullets have not been researched yet. Continue completing the Tiphereths' missions if you wish to make use of them.";
                    if (agent == null) return "failure|An Agent must be the target of a White Shield Bullet.";
                    bullet = GlobalBulletType.RESIST_W;
                    break;
                case "Black Shield":
                    if (!ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RESIST_B)) return "failure|Black Shield Bullets have not been researched yet. Continue completing the Tiphereths' missions if you wish to make use of them.";
                    if (agent == null) return "failure|An Agent must be the target of a Black Shield Bullet.";
                    bullet = GlobalBulletType.RESIST_B;
                    break;
                case "Pale Shield":
                    if (!ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RESIST_P)) return "failure|Only those who have obtained the Expectation for the Meaning of Existence may fire a Pale Shield Bullet.";
                    if (agent == null) return "failure|An Agent must be the target of a Pale Shield Bullet.";
                    bullet = GlobalBulletType.RESIST_P;
                    break;
                case "Slow":
                    if (!ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.SLOW)) return "failure|Slow Bullets have not been researched yet. Continue completing Gebura's missions if you wish to make use of them.";
                    if (agent != null && !agent.IsPanic()) return "failure|A sane Agent must not be the target of a Slow Bullet.";
                    bullet = GlobalBulletType.SLOW;
                    break;
                case "Execution":
                    if (true) return "failure|You are not allowed to fire Execution Bullets."; //TODO: change neuro's ability to fire execution bullets to be a config option. a highly disincentivized option but an option nonetheless.
                    if (!ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.EXCUTE)) return "failure|Execution Bullets have not been researched yet. Continue completing Gebura's missions if you wish to make use of them.";
                    if (agent == null) return "failure|An employee must be the target of a Execution Bullet."; //TODO: allow clerks to be targetted...? might be hard for neuro to keep track of though.
                    bullet = GlobalBulletType.EXCUTE;
                    break;
                default:
                    return String.Format("failure|The specified bullet type is not valid. The following are the bullet types you may currently use: {0}.", UnlockedBulletTypes());
            }
            ThreadPool.QueueUserWorkItem(CommandExecute, new ShootManagerialBulletState(unit, bullet));
            return String.Format("success|The {0} Bullet was successfully fired at {1}.", bulletType, targetName);
        }

        private static string UnlockedBulletTypes()
        {
            List<string> unlockedTypes = new List<string>();
            if (ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RECOVER_HP)) unlockedTypes.Add("HP Recovery");
            if (ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RECOVER_MENTAL)) unlockedTypes.Add("SP Recovery");
            if (ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RESIST_R)) unlockedTypes.Add("Red Shield");
            if (ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RESIST_W)) unlockedTypes.Add("White Shield");
            if (ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RESIST_B)) unlockedTypes.Add("Black Shield");
            if (ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RESIST_P)) unlockedTypes.Add("Pale Shield");
            if (ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.SLOW)) unlockedTypes.Add("Slow");
            if (ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.EXCUTE) && !true) unlockedTypes.Add("Execute"); //TODO: config option again
            string result = "";
            for (int i = 0; i < unlockedTypes.Count; i++)
            {
                result += unlockedTypes[i];
                if (i != unlockedTypes.Count - 1) result += ", ";
            }
            return result;
        }

        private class ShootManagerialBulletState
        {
            public UnitModel Target { get; private set; }

            public GlobalBulletType BulletType { get; private set; }

            public ShootManagerialBulletState(UnitModel target, GlobalBulletType bulletType)
            {
                Target = target;
                BulletType = bulletType;
            }
        }

        private static void CommandExecute(object state)
        {
            ShootManagerialBulletState shootManagerialBulletState = (ShootManagerialBulletState)state;
            UnitModel unit = shootManagerialBulletState.Target;
            GlobalBulletType bulletType = shootManagerialBulletState.BulletType;

            //stolen from GlobalBulletWindow, gets all unitmodels overlapping the target
            Vector2 targetPosition = unit.GetCurrentViewPosition();
            Collider2D[] overlappingColliders = Physics2D.OverlapPointAll(targetPosition);
            List<UnitModel> overlappingTargets = new List<UnitModel>();
            foreach (Collider2D collider2D in overlappingColliders)
            {
                UnitMouseEventTarget component = collider2D.GetComponent<UnitMouseEventTarget>();
                if (!(component == null))
                {
                    if (bulletType != GlobalBulletType.SLOW && component.GetCommandTargetModel() is WorkerModel)
                    {
                        overlappingTargets.Add(component.GetCommandTargetModel() as WorkerModel);
                    }
                    if (bulletType == GlobalBulletType.SLOW)
                    {
                        if (component.GetCommandTargetModel() is CreatureModel)
                        {
                            overlappingTargets.Add(component.GetCommandTargetModel() as CreatureModel);
                        }
                        else
                        {
                            WorkerModel workerModel = component.GetCommandTargetModel() as WorkerModel;
                            if (workerModel != null && workerModel.IsPanic())
                            {
                                overlappingTargets.Add(workerModel);
                            }
                        }
                    }
                }
            }
            neuroFiredBullet = bulletType;
            neuroFiredTargets = overlappingTargets;
        }
        private static GlobalBulletType neuroFiredBullet = GlobalBulletType.NONE;
        private static List<UnitModel> neuroFiredTargets = new List<UnitModel>();

        /*
         * So: every time I try to call ActivateBullet from within CommandExecute, the game crashes. 
         * I ultimately figured out this is because said method accesses UnityEngine.CoreModule, which I'm not allowed to do apparently.
         * However, calling ActivateBullet from within a harmony patch postfix *is* allowed, for some reason. 
         * So CommandExecute instead saves the type of bullet Neuro is trying to fire and the targets that will be hit,
         * then this method, which is a postfix for GlobalBulletWindow's Update, will check to see if Neuro is trying to fire
         * (by checking if the type of bullet she wants to fire is set), and calls ActivateBullet from there.
        */
        public static void NeuroShootBullet()
        {
            if(neuroFiredBullet != GlobalBulletType.NONE)
            {
                GlobalBulletManager.instance.ActivateBullet(neuroFiredBullet, neuroFiredTargets);
                neuroFiredBullet = GlobalBulletType.NONE;
                neuroFiredTargets = new List<UnitModel>();
            }
        }
    }
}
