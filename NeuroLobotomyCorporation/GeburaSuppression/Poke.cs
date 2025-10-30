using GeburahBoss;
using NeuroLobotomyCorporation.FacilityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.GeburaSuppression
{
    public class Poke
    {
        private static bool poking = false;
        public static bool GivePokeStarted = false;
        public static string Command()
        {
            if (poking) return "failure|The previous poke has not gone through yet...please wait a moment...";
            if (SefiraManager.instance.GameOverCheck()) return "failure|Every Agent is dead...suppression cannot continue. Please ask the manager to retry the day."; //i'm not letting vedal tell neuro to spam poke 12000 times to win. because i feel like that's something he'd consider if everyone else was dead.
            SefiraBossCreatureModel redMistModel = (SefiraBossCreatureModel)Helpers.TryFindSefiraCoreTarget("The Red Mist");
            if (RedMistRagebait.Instance == null || RedMistRagebait.Instance.RedMistModel != redMistModel) RedMistRagebait.Instance = new RedMistRagebait(redMistModel);
            string ragebaitResult = RedMistRagebait.Instance.Poked();
            poking = true;
            return String.Format("success|{0}", ragebaitResult);
        }

        /*
         * Calling SefiraBossCreatureModel.TakeDamage outside of a harmony patch crashes the game, so patch the FixedUpdate to trigger it then.
         * The same applies to MakeGeburahText.
         */
        public static void PokeRedMist(SefiraBossCreatureModel __instance) //pokemon red mist and blue reverberation ehehe
        {
            if (poking)
            {
                //if (RedMistRagebait.Instance == null || RedMistRagebait.Instance.RedMistModel != __instance) RedMistRagebait.Instance = new RedMistRagebait(__instance);
                poking = false;               
                RedMistRagebait.Instance.RedMistModel.TakeDamage(null, new DamageInfo(RwbpType.N, 1f));
                if (!String.IsNullOrEmpty(RedMistRagebait.Instance.ragingDialogue)) { (__instance.script as GeburahCoreScript).AnimScript.MakeGeburahText(RedMistRagebait.Instance.ragingDialogue); RedMistRagebait.Instance.ragingDialogue = ""; }
            }
        }

        public class RedMistRagebait
        {
            public static RedMistRagebait Instance 
            {
                get
                {
                    return instance;
                }
                set 
                {
                    instance = value;
                } 
            }
            private static RedMistRagebait instance = null;

            public SefiraBossCreatureModel RedMistModel = null;

            private int timesPokedSinceLastRage = 0;
            private static readonly int MAX_POKES_BEFORE_RAGE = 100;

            private bool hasRaged = false;
            private static readonly int MAX_POKES_FIRST_RAGE = 10;

            private int timesRagedSinceLastBait = 0;
            private static readonly int BAITS_BEFORE_RAGE = 3;
            public string ragingDialogue = "";

            public bool FullyBaited { get; set; }
            public string fullyRagebaitedDialogue = "";

            private static readonly float MALD_TIME = 5f;

            private static Random rand = new Random();

            public RedMistRagebait(SefiraBossCreatureModel redMistModel)
            {
                RedMistModel = redMistModel;
                FullyBaited = false;
            }

            private static string[] rageIncreasing = new string[]
            {
                "Who is poking me?",
                "Is there a mosquito in here?",
                "I can't focus..."
            };

            private static string[] fullyBaited = new string[]
            {
                "I'll find you, you little...",
                "Where is that damn bug?!",
                "Stop distracting me, or I'll..."
            };

            private static string[] calmedDown = new string[]
            {
                "I'll find you soon enough.",
                "I'll crush that bug with the rest of them.",
                "Focus..."
            };

            public string Poked()
            {
                string ragebaitResult = "The Red Mist has been poked.";
                if (FullyBaited) return ragebaitResult;
                timesPokedSinceLastRage++;
                if(rand.Next(0, MAX_POKES_BEFORE_RAGE) < timesPokedSinceLastRage || (!hasRaged && rand.Next(0, MAX_POKES_FIRST_RAGE) < timesPokedSinceLastRage))
                {
                    timesPokedSinceLastRage = 0;
                    timesRagedSinceLastBait++;
                    hasRaged = true;
                    if (timesRagedSinceLastBait < BAITS_BEFORE_RAGE)
                    {
                        ragingDialogue = rageIncreasing[rand.Next(rageIncreasing.Length)];
                        ragebaitResult = String.Format("\"{0}\"", ragingDialogue);
                        //(RedMistModel.script as GeburahCoreScript).AnimScript.MakeGeburahText(ragebaitResult);
                    }
                }
                if(timesRagedSinceLastBait >= BAITS_BEFORE_RAGE)
                {
                    FullyBaited = true;
                    fullyRagebaitedDialogue = fullyBaited[rand.Next(rageIncreasing.Length)];
                    ragebaitResult = String.Format("\"{0}\"", fullyRagebaitedDialogue);
                }
                return ragebaitResult;
            }

            public GeburahAction GetBaitedLULE()
            {
                FullyBaited = false;
                timesPokedSinceLastRage = 0;
                timesRagedSinceLastBait = 0;
                (RedMistModel.script as GeburahCoreScript).AnimScript.MakeGeburahText(fullyRagebaitedDialogue);
                return new GeburahRagebaitedIdle(RedMistModel.script as GeburahCoreScript, MALD_TIME, false);
            }

            private class GeburahRagebaitedIdle : GeburahIdle
            {
                private Timer baitTimer = new Timer();
                private float baitTime;
                private int timesChangedDirection = 0;
                private static readonly int MAX_DIRECTION_CHANGES = 12;
                public GeburahRagebaitedIdle(GeburahCoreScript geburah, float time, bool isGroggy = false) : base(geburah, time, isGroggy)
                {
                    baitTime = time;
                }

                public GeburahRagebaitedIdle(GeburahCoreScript geburah, bool nearClose, float time) : base(geburah, nearClose, time)
                {
                }

                public override void ParamInit()
                {
                    CreatureCommand currentCommand = base.Model.GetCurrentCommand();
                    if (currentCommand != null)
                    {
                        currentCommand.SetEndCommand(null);
                        base.Model.ClearCommand();
                    }
                    this.baitTimer.StartTimer(this.baitTime);
                }

                public override void OnExecute()
                {
                    if (this.baitTimer.started && this.baitTimer.RunTimer())
                    {
                        this.EndAction();
                        return;
                    }
                    if(this.baitTimer.started && this.baitTimer.elapsed > (baitTime / MAX_DIRECTION_CHANGES * timesChangedDirection))
                    {
                        UnitDirection changeDirection = (this.geburah.movable.GetDirection() == UnitDirection.LEFT) ? UnitDirection.RIGHT : UnitDirection.LEFT;
                        this.geburah.movable.SetDirection(changeDirection);
                        timesChangedDirection++;
                    }
                }

                public override void OnEnd()
                {
                    (RedMistRagebait.instance.RedMistModel.script as GeburahCoreScript).AnimScript.MakeGeburahText(calmedDown[rand.Next(rageIncreasing.Length)]);
                }
            }
        }
    }
}
