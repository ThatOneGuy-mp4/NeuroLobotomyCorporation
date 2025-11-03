using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class GetSuppressibleTargets
    {
        private static bool apocalypseBirdAdded = false;
        public static string Command()
        {
            string result = "";
            List<string> specialEnemies = new List<string>();
            apocalypseBirdAdded = false;
            if (SefiraBossManager.Instance.IsAnyBossSessionActivated() && SefiraBossManager.Instance.CurrentBossBase.modelList.Count > 0)
            {
                foreach (SefiraBossCreatureModel bossSefira in SefiraBossManager.Instance.CurrentBossBase.modelList)
                {
                    TryCheckIsSpecialEnemy(bossSefira, specialEnemies);
                }
            }
            List<AgentModel> panickingAgents = new List<AgentModel>();
            foreach (AgentModel agent in AgentManager.instance.GetAgentList())
            {
                if (agent.IsPanic() && !agent.IsDead()) panickingAgents.Add(agent);
            }
            if (panickingAgents.Count > 0)
            {
                result += "Panicking Agents (suppress with WHITE or BLACK damage to recover sanity)\n-------------------------\n";
                List<Helpers.Entry<SefiraEnum, List<AgentModel>>> sortedAgents = Helpers.SortAgentsByDepartment(panickingAgents.ToArray(), true);
                foreach (Helpers.Entry<SefiraEnum, List<AgentModel>> sortingEntry in sortedAgents)
                {
                    if (sortingEntry.v.Count == 0) continue;
                    result += String.Format("Currently in the {0} Department:\n", Helpers.GetDepartmentBySefira(sortingEntry.k));
                    foreach (AgentModel agent in sortingEntry.v)
                    {
                        string name = agent.GetUnitName();
                        int hp = (int)agent.hp;
                        int maxHp = agent.maxHp;
                        int sp = (int)agent.mental;
                        int maxSp = agent.maxMental;
                        string panicAction = "";
                        if (agent.CurrentPanicAction is PanicViolence)
                        {
                            panicAction = "Attempting to Commit Murder";
                        }
                        else if (agent.CurrentPanicAction is PanicSuicideExecutor)
                        {
                            panicAction = "Preparing to Commit Suicide";
                        }
                        else if (agent.CurrentPanicAction is PanicRoaming)
                        {
                            panicAction = "Running Around Aimlessly";
                        }
                        else if (agent.CurrentPanicAction is PanicOpenIsolate)
                        {
                            panicAction = "Attempting to Free Abnormalities";
                        }
                        else
                        {
                            panicAction = "Panicking"; //i don't know if AiB's panic will register differently so this is just a backup
                        }
                        result += String.Format("-{0}, {1}/{2} HP, {3}/{4} SP, Currently {5}\n", name, hp, maxHp, sp, maxSp, panicAction);
                    }
                    result += "\n";
                }
                result += "\n";
            }
            foreach (EventCreatureModel eventCreature in SpecialEventManager.instance.GetEventCreatureList())
            {
                TryCheckIsSpecialEnemy(eventCreature, specialEnemies);
            }
            List<CreatureModel> escapedAbnormalities = Helpers.GetAllSuppressibleAbnormalitiesAndChildren();
            if (escapedAbnormalities.Count > 0)
            {
                escapedAbnormalities = RemoveSpecialAbnormalities(escapedAbnormalities, specialEnemies); 
                result += "Breaching Abnormalities\n-------------------------\n";
                List<Helpers.Entry<SefiraEnum, List<CreatureModel>>> sortedAbnormalities = Helpers.SortAbnormalitiesByDepartment(escapedAbnormalities.ToArray(), true);
                foreach (Helpers.Entry<SefiraEnum, List<CreatureModel>> sortingEntry in sortedAbnormalities)
                {
                    if (sortingEntry.v.Count == 0) continue;
                    result += String.Format("Currently in the {0} Department:\n", Helpers.GetDepartmentBySefira(sortingEntry.k));
                    foreach (CreatureModel abnormality in sortingEntry.v)
                    {
                        string name = abnormality.script.GetName();
                        string riskLevel = abnormality.metaInfo.riskLevelForce;
                        string healthPercentRemaining = ((int)((float)(abnormality.hp / abnormality.metaInfo.maxHp) * 100)).ToString();
                        List<string> defenseInfo = Helpers.GetResistanceTypeValues(abnormality);
                        result += String.Format("-{0}, {1} Level Threat, {2}% HP Remaining, " +
                        "{3}/{4}/{5}/{6} (Red/White/Black/Pale) Resistances\n", name, riskLevel, healthPercentRemaining,
                        defenseInfo[(int)Helpers.ResistanceTypes.RED], defenseInfo[(int)Helpers.ResistanceTypes.WHITE], defenseInfo[(int)Helpers.ResistanceTypes.BLACK], defenseInfo[(int)Helpers.ResistanceTypes.PALE]);
                    }
                    result += "\n";
                }
            }
            List<OrdealBase> activatedOrdeals = OrdealManager.instance.GetActivatedOrdeals();
            if (activatedOrdeals.Count > 0) //ordeal creatures are not sorted by current location because grouping them by ordeal type makes more sense
            {
                foreach (OrdealBase ordeal in activatedOrdeals)
                {
                    List<OrdealCreatureModel> ordealCreaturesInOrdeal = new List<OrdealCreatureModel>();
                    foreach (OrdealCreatureModel ordealCreature in OrdealManager.instance.GetOrdealCreatureList())
                    {
                        if (ordealCreature.OrdealBase.OrdealTypeText.Equals(ordeal.OrdealTypeText)) ordealCreaturesInOrdeal.Add(ordealCreature);
                    }
                    string ordealTargets = String.Format("{0}\n-------------------------\n", ordeal.OrdealTypeText);
                    foreach (OrdealCreatureModel ordealCreature in ordealCreaturesInOrdeal)
                    {
                        if (ordealCreature.hp <= 0) continue;
                        string name = ordealCreature.OrdealBase.OrdealNameText(ordealCreature); //would GetUnitName work here?
                        string riskLevel = ordealCreature.OrdealBase.GetRiskLevel(ordealCreature).ToString();
                        string location = Helpers.GetUnitModelLocationText(ordealCreature);
                        string healthPercentRemaining = ((int)((float)(ordealCreature.hp / ordealCreature.metaInfo.maxHp) * 100)).ToString();
                        ordealTargets += String.Format("{0}, {1} Level Threat, {2}, {3}% HP Remaining\n", name, riskLevel, location, healthPercentRemaining);
                    }
                    result += ordealTargets + "\n";
                }
            }
            if (specialEnemies.Count > 0)
            {
                string specialResult = "";
                for (int i = 0; i < specialEnemies.Count; i++)
                {
                    specialResult += specialEnemies[i];
                    if (i != specialEnemies.Count - 1) specialResult += "\n";
                }
                result = String.Format("! WARNING ! WARNING ! WARNING ! WARNING !\n" +
                    "-----------------------------------------\n" +
                    "{0}\n" +
                    "-----------------------------------------\n" +
                    "! WARNING ! WARNING ! WARNING ! WARNING !\n\n", specialResult) + result;
            }

            if (String.IsNullOrEmpty(result)) return "There are no suppressable targets at the moment.";
            return result;
        }



        private static bool TryCheckIsSpecialEnemy(UnitModel unit, List<string> specialEnemies)
        {

            string name = unit.GetUnitName();
            if (unit is SefiraBossCreatureModel)
            {
                SefiraBossCreatureModel bossSefira = (SefiraBossCreatureModel)unit;
                name = bossSefira.script.GetName();
            }
            switch (name)
            {
                case "The Red Mist": //bait used to be believable-
                    SefiraBossCreatureModel redMistModel = (SefiraBossCreatureModel)unit;
                    GeburahCoreScript redMistScript = (GeburahCoreScript)redMistModel.script;
                    int healthPercentRemaining = (int)((float)(redMistModel.hp / redMistModel.metaInfo.maxHp) * 100);
                    int coreSuppressionProgress = (100 - healthPercentRemaining) / 4;
                    string location = Helpers.GetUnitModelLocationText(unit);
                    string equippedEGOGear = "";
                    List<string> defenseInfo = Helpers.GetResistanceTypeValues(redMistModel);
                    switch (redMistScript.Phase)
                    {
                        case GeburahBoss.GeburahPhase.P1:
                            equippedEGOGear = "Red Eyes & Penitence";
                            break;
                        case GeburahBoss.GeburahPhase.P2:
                            equippedEGOGear = "Mimicry & De Capo";
                            coreSuppressionProgress += 25;
                            break;
                        case GeburahBoss.GeburahPhase.P3:
                            equippedEGOGear = "Smile & Justitia";
                            coreSuppressionProgress += 50;
                            break;
                        case GeburahBoss.GeburahPhase.P4:
                            equippedEGOGear = "Twilight & an Effloresced E.G.O";
                            coreSuppressionProgress += 75;
                            break;
                        default:
                            equippedEGOGear = "Nothing";
                            break;
                    }
                    specialEnemies.Add(String.Format("The Red Mist\n" +
                        "Equipped E.G.O Gear: {0}\n" +
                        "{1}% HP Remaining (Overall Suppression is {2}% Complete)\n" +
                        "{3}\n" +
                        "{4}", equippedEGOGear, healthPercentRemaining.ToString(), coreSuppressionProgress.ToString(),
                        location, Helpers.GetResistancesText(defenseInfo)));
                    return true;
                //TODO: fill the rest of these out once you can test them
                case "An Arbiter":
                    specialEnemies.Add("biin");
                    return true;
                case "T-03-46":
                case "WhiteNight":
                    specialEnemies.Add("wiin");
                    return true;
                /* 
                 * Apocalypse Bird doesn't get correctly added to the output if I check for it last time I tried 
                 * and I can't be bothered to figure out why, so instead just check for Judgement Bird and add it if the boss exists.
                 */
                case "O-02-62":
                case "Long Bird":
                case "Judgement Bird":
                    LongBird longBird = (unit as CreatureModel).script as LongBird;
                    if (longBird.Boss_Activated)
                    {
                        if (!apocalypseBirdAdded) specialEnemies.Add(GetApocalypseBirdStatus(longBird.Boss));
                        return true;
                    }
                    if (!longBird.Unit.gameObject.activeSelf) return true;
                    return false;
                //Entrance doesn't go away for some reason even after Apocalypse Bird is dead. so check if the portal is dead.
                case "Entrance to the Black Forest":
                    if ((unit as CreatureModel).hp == 0) return false;
                    specialEnemies.Add(String.Format("Entrance to the Black Forest: {0}" +
                        "\n'Once upon a time, three happy birds lived in a warm and lush forest.'" +
                        "\nPrepare yourself for the dark twilight soon to come...", Helpers.GetUnitModelLocationText(unit)));
                    return true;
                case "The Claw":
                    specialEnemies.Add("caw");
                    return true;
                default:
                    //specialEnemies.Add(name);
                    break;
            }
            return false;
        }

        private static List<CreatureModel> RemoveSpecialAbnormalities(List<CreatureModel> abnormalities, List<string> specialEnemies)
        {
            List<CreatureModel> nonSpecialAbnormalities = new List<CreatureModel>();
            foreach (CreatureModel abnormality in abnormalities)
            {
                if (!TryCheckIsSpecialEnemy(abnormality, specialEnemies)) nonSpecialAbnormalities.Add(abnormality);
            }
            return nonSpecialAbnormalities;
        }

        private static string GetApocalypseBirdStatus(BossBird apocalypseBird)
        {
            apocalypseBirdAdded = true;
            string status = "";
            string healthPercentRemaining = ((int)((float)(apocalypseBird.model.hp / apocalypseBird.model.maxHp) * 100)).ToString();
            string location = Helpers.GetUnitModelLocationText(apocalypseBird.model);
            status += String.Format("Apocalypse Bird" +
                "\nALEPH Level Threat, {0}% HP Remaining, {1}" +
                "\nImmune/Immune/Immune/Immune (Red/White/Black/Pale) Resistances\n\n", healthPercentRemaining, location);

            if ((apocalypseBird.BigEggModel.script as BossEggBase).IsEnabled())
            {

                healthPercentRemaining = ((int)((float)(apocalypseBird.BigEggModel.hp / apocalypseBird.BigEggModel.maxHp) * 100)).ToString();
                location = Helpers.GetUnitModelLocationText(apocalypseBird.BigEggModel);
                List<string> defenseInfo = Helpers.GetResistanceTypeValues(apocalypseBird.BigEggModel);
                status += String.Format("Big eyes: ALEPH Level Threat, {0}% HP Remaining, {1}, " +
                    "{2}/{3}/{4}/{5} (Red/White/Black/Pale) Resistances" +
                    "\n'The Big Bird's eyes imprisoned light.'\n\n", healthPercentRemaining, location,
                     defenseInfo[(int)Helpers.ResistanceTypes.RED], defenseInfo[(int)Helpers.ResistanceTypes.WHITE], defenseInfo[(int)Helpers.ResistanceTypes.BLACK], defenseInfo[(int)Helpers.ResistanceTypes.PALE]);
            }
            else
            {
                status += "'Big Bird's far seeing eyes were blinded,'\n\n";
            }

            if ((apocalypseBird.LongEggModel.script as BossEggBase).IsEnabled())
            {
                healthPercentRemaining = ((int)((float)(apocalypseBird.LongEggModel.hp / apocalypseBird.LongEggModel.maxHp) * 100)).ToString();
                location = Helpers.GetUnitModelLocationText(apocalypseBird.LongEggModel);
                List<string> defenseInfo = Helpers.GetResistanceTypeValues(apocalypseBird.LongEggModel);
                status += String.Format("Long arm: ALEPH Level Threat, {0}% HP Remaining, {1}, " +
                    "{2}/{3}/{4}/{5} (Red/White/Black/Pale) Resistances" +
                    "\n'The Long Bird’s arms concealed time.'\n\n", healthPercentRemaining, location,
                     defenseInfo[(int)Helpers.ResistanceTypes.RED], defenseInfo[(int)Helpers.ResistanceTypes.WHITE], defenseInfo[(int)Helpers.ResistanceTypes.BLACK], defenseInfo[(int)Helpers.ResistanceTypes.PALE]);
            }
            else
            {
                status += "'The head that always gazed upon the cosmos was lowered,'\n\n";
            }

            if ((apocalypseBird.SmallEggModel.script as BossEggBase).IsEnabled())
            {
                healthPercentRemaining = ((int)((float)(apocalypseBird.SmallEggModel.hp / apocalypseBird.SmallEggModel.maxHp) * 100)).ToString();
                location = Helpers.GetUnitModelLocationText(apocalypseBird.SmallEggModel);
                List<string> defenseInfo = Helpers.GetResistanceTypeValues(apocalypseBird.SmallEggModel);
                status += String.Format("Small beak: ALEPH Level Threat, {0}% HP Remaining, {1}, " +
                    "{2}/{3}/{4}/{5} (Red/White/Black/Pale) Resistances" +
                    "\n'And the Small Bird’s beak whispered, endlessly...'\n\n", healthPercentRemaining, location,
                     defenseInfo[(int)Helpers.ResistanceTypes.RED], defenseInfo[(int)Helpers.ResistanceTypes.WHITE], defenseInfo[(int)Helpers.ResistanceTypes.BLACK], defenseInfo[(int)Helpers.ResistanceTypes.PALE]);
            }
            else
            {
                status += "'Small Bird's mouth that could devour anything was shut,'\n\n";
            }
            status += "'As long as they exist, the Apocalypse Bird will not disappear.'";
            return status;
        }

        [HarmonyPatch(typeof(BossBirdAnim), "DisplayNarration", new Type[] { typeof(BossBird.NarrationState), typeof(float), typeof(BossBirdAnim.NarrationUI.NarrationEndEvent) })]
        public class TellNarrationNoFadeoutEvent
        {
            public static void Postfix(BossBirdAnim __instance, BossBird.NarrationState state)
            {
                TellApocalypseBirdNarration(__instance.script, state);
            }
        }

        [HarmonyPatch(typeof(BossBirdAnim), "DisplayNarration", new Type[] { typeof(BossBird.NarrationState), typeof(float), typeof(BossBirdAnim.NarrationUI.NarrationEndEvent), typeof(BossBirdAnim.NarrationUI.FadeOutEvent) })]
        public class TellNarrationFadeoutEvent
        {
            public static void Postfix(BossBirdAnim __instance, BossBird.NarrationState state)
            {
                TellApocalypseBirdNarration(__instance.script, state);
            }
        }

        public static void TellApocalypseBirdNarration(BossBird __instance, BossBird.NarrationState state)
        {
            string narration = String.Format("'{0}'\n", __instance.GetParamData(BossBird.GetNarrationKeyByEnum(state)).Trim());
            switch (state)
            {
                case BossBird.NarrationState.ESCAPE:
                    narration += "The Entrance to the Black Forest has appeared. The three birds have begun to move towards it." +
                        "\nA dark twilight is soon to come...";
                    break;
                case BossBird.NarrationState.BOSS_APPEAR:
                    narration += "WARNING! The three birds have fused into the Apocalypse Bird! The eggs which give it strength have also appeared around the facility." +
                        "\nIf the Apocalypse Bird is successfully suppressed, a special reward may be extracted.";
                    break;
                case BossBird.NarrationState.BIGBIRD_ARRIVED:
                    narration += "Big Bird has arrived at the Entrance to the Black Forest.";
                    break;
                case BossBird.NarrationState.BIGBIRD_BREAKDOWN:
                    narration += "Big Bird's egg has been destroyed. Apocalypse Bird has lost 33% of its Max HP, and can no longer lure Agents.";
                    break;
                case BossBird.NarrationState.LONGBIRD_ARRIVED:
                    narration += "Long Bird has arrived at the Entrance to the Black Forest.";
                    break;
                case BossBird.NarrationState.LONGBIRD_BREAKDOWN:
                    narration += "Long Bird's egg has been destroyed. Apocalypse Bird has lost 33% of its Max HP, and can no longer control time.";
                    break;
                case BossBird.NarrationState.SMALLBIRD_ARRIVED:
                    narration += "Small Bird has arrived at the Entrance to the Black Forest.";
                    break;
                case BossBird.NarrationState.SMALLBIRD_BREAKDOWN:
                    narration += "Small Bird's egg has been destroyed. Apocalypse Bird has lost 33% of its Max HP, and can no longer devour Agents.";
                    break;
                case BossBird.NarrationState.SUPPRESSED:
                    narration += "Apocalypse Bird has been successfully suppressed." +
                        "\nAll Agents who have survived the suppression have received the E.G.O Gift \"Through the Dark Twilight\"." +
                        "\nThe extremely powerful E.G.O Weapon and Suit \"Twilight\" has been obtained.";
                    break;
            }
            NeuroSDKHandler.SendContext(narration, true);
        }
    }
}
