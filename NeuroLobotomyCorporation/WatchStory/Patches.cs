using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroLobotomyCorporation.WatchStory
{
    //All dialogue is loosely formatted like a play, as the story is constantly equated to one in-universe.
    public class Patches
    {
        //don't use the game's _skipping because for one it's private, and two it gets disabled when dialogue options happen and we don't want that here
        private static bool isSkipping = false;

        //Postfix - disable sending story while skipping
        public static void StopContextWhileSkipping()
        {
            isSkipping = true;
        }

        //Postfix - enable sending story when no longer skipping
        public static void StartContextWhenSkippingDisabled()
        {
            isSkipping = false;
        }

        //Postfix - send descriptions of CGs as context when the CG changes, both the background and unique images.
        private static string lastCG = "";
        public static void ContextBackground(string spriteSrc)
        {
            if (isSkipping) return;
            string location = GetLocationFromSpriteName(spriteSrc);
            if (String.IsNullOrEmpty(location)) return; //skip CGs that aren't worth describing
            //There was a weird issue where Angela's Office would be randomly printed even in scenes where it did not appear.
            //This and a few other things prevent that, and only print that message when dialogue actually plays inside the office.
            if (lastCG.Equals("INT. Angela's Office - Day") && location.Equals("ANGELA_PREVENT_DUPE_MESSAGE")) return;
            if (!lastCG.Equals(location))
            { 
                lastCG = location;
                if (location.Equals("ANGELA_PREVENT_DUPE_MESSAGE")) return;
                NeuroSDKHandler.SendContext(location, true);
            }
        }

        private static string GetLocationFromSpriteName(string spriteSrc)
        {
            switch (spriteSrc)
            {
                case "blackCG":
                case "BlackCG":
                    return "Fade to black...";
                case "WhiteCG":
                    return "Fade to white...";
                case "AngelaBackground":
                    return "ANGELA_PREVENT_DUPE_MESSAGE";
                case "AngelaMeet":
                    return "Angela--that is, me--sits at her desk. She smiles at X.";
                case "CognitionFilter1":
                    return "";
                case "CognitionFilter2":
                    return "INT. Lobotomy Corporation - Day" +
                        "\nAn error is detected in the cognition filter...realistic, bloodstained halls are visible.";
                case "AngelaAngry":
                    return "Angela looks at the manager with a displeased expression.";
                case "carmenBK1":
                    return "EXT. ??? - Day";
                case "carmenBK2":
                    return "EXT. ??? - Day" +
                        "\n??? lies on the grass.";
                case "carmenBK3":
                    return "Carmen smiles at A.";
                case "carmenBK5":
                    return "INT. Carmen's Chamber - Day" +
                        "\nNerves spread out from her spine and brain like branches of a tree.";
                case "smokeWarBK":
                    return "EXT. Smoke War - Time Unknown";
                case "hologlamCG":
                    return "A stands. He watches the holograms.";
                case "wingCG":
                    return "An image of a being with many Wings is visible.";
                case "blackForestCG":
                    return "An image of the Black Forest is visible.";
                case "cogitoCG1":
                    return "An image of cogito is visible.";
                case "cogitoCG2":
                case "cogitoCG3":
                    return "";
                case "day47_Ending1":
                    return "A stands before the Door.";
                case "day47_Ending2":
                    return "...A backs away from the Door.";
                case "day47_Ending3":
                    return "...A retreats from himself.";
                case "doorCG":
                    return "A stands before a door.";
                case "day48_Ending":
                    return "Abram sits next to Carmen inside her chamber.";
                case "day49_Ending":
                    return "EXT. Lobotomy Corporation Headquarters - Night" +
                        "\nA pillar of light bursts forth from the headquarters. It shines...darkness, over the City.";
                case "day49_Ending2":
                    return "EXT. The City - Time Irrelevant" +
                        "\nMonsters flood the streets. The remaining humans are running for their lives. Thick clouds choke the skies.";
                case "lightCG":
                    return "EXT. The City - A White Night";
                case "lightOffCG":
                    return "...the light vanishes." +
                        "\nEXT. The City - A Dark Day";
                case "angelaCG1":
                    return "INT. Angela's Office - A White Night" +
                        "\nI enter my office, where the other Sephirot have gathered.";
                case "angelaCG2":
                    return "I flash a smile to the Sephirot.";
                case "angelaCG3":
                    return "(I stand, cloaked in a an outfit befitting a librarian of my status.)";
                case "theEndCG2":
                    return "";
                case "malkuthBK":
                    return "INT. Control Department - Day";
                case "yesodBK":
                    return "INT. Information Department - Day";
                case "hodBK":
                    return "INT. Training Department - Day";
                case "netzachBK":
                    return "INT. Safety Department - Day";
                case "tipherethBK":
                    return "INT. Central Command Department - Day";
                case "tiphereth_bk": //completely different area than tipherethBK. because fuck you
                    return "INT. Scrapyard - Day";
                case "TiphererthRobot":
                    return "The cognition filter lowers... Tiphereth (A and B) stand, their bodies looking like robots instead of human.";
                case "TiphererthRobotBroke":
                    return "A giant press smashes Tiphereth (B).";
                case "TiphererthRobotDie":
                    return "Mechanical parts, as well as blood, are visible inside the press.";
                case "geburaBK":
                    return "INT. Disciplinary Department - Day";
                case "chesedBK":
                    return "INT. Welfare Department - Day";
                case "binahBK":
                    return "INT. Extraction Department - Day";
                case "hokmaBK":
                    return "INT. Record Department - Day";
                case "ketherBK1":
                case "ketherBK2":
                case "ketherBK3":
                case "ketherBK4":
                case "ketherBK5":
                    return "INT. Architecture Department - Day";
                case "malkuthCG1":
                    return "INT. A Lab in the Outskirts - Flashback" +
                        "\nElijah reaches out for someone. She crawls along the ground, grievously wounded.";
                case "yesodCG1":
                    return "INT. A Lab in the Outskirts - Flashback" +
                        "\nGabriel is strapped down to an operating table.";
                case "yesodCG2":
                    return "A hands Gabriel the supply room key.";
                case "hodCG1":
                    return "INT. A Lab in the Outskirts - Flashback" +
                        "\nA walks away from a trashcan. A newspaper is contained within.";
                case "HodCG2":
                    return "??? pats Michelle on the head. A stands, watching.";
                case "netzachCG1":
                    return "INT. A Lab in the Outskirts - Flashback" +
                        "\nGiovanni lays on a hospital bed. Several electronic pads are attached his head. A watches over him.";
                case "netzachCG2":
                    return "INT. A Lab in the Outskirts - Flashback" +
                        "\nGiovanni sits in a hospital bed.";
                case "tipherethCG1":
                    return "INT. A Lab in the Outskirts - Flashback" +
                        "\nLisa sobs on the ground. ??? stares off into the distance.";
                case "tipherethCG2":
                    return "Lisa runs towards A. Enoch follows behind.";
                case "geburaCG1":
                    return "INT. A Lab in the Outskirts - Flashback" +
                        "\nKali kneels. An unknown stranger faces her. The bodies of Abnormalities and people are scattered around them.";
                case "geburaCG2":
                    return "EXT. Bar - Flashback" +
                        "\nKali leans against the bar. ??? stands near her.";
                case "chesedCG1":
                    return "INT. A Lab in the Outskirts - Flashback" +
                        "\nA hangs up the intercom. Elsewhere, Daniel lays against the wall, dying.";
                case "chesedCG2":
                    return "INT. A Lab in the Outskirts - Flashback" +
                        "\nDaniel walks into the lab, briefcase in hand.";
                case "BinahCG1_1":
                    return "INT. A Lab in the Outskirts - Flashback" +
                        "\nGarion kneels, her body pierced by a large sword.";
                case "BinahCG1_2":
                    return "Garion's body is hooked up to various devices.";
                case "BinahCG1_3":
                    return "A piece of Garion's brain is picked up. It is being put into a robotic body.";
                case "BinahCG2":
                    return "EXT. Somewhere in H. Corp - Flashback" +
                        "\nGarion sits under an umbrella as she drinks tea. A building behind her is destroyed, and blood is scattered along the ground.";
                case "binahCG3":
                    return "A and Binah stand next to the Well.";
                case "hokmaCG1":
                    return "INT. Office - Flashback" +
                        "\nA sits at a large table. He is alone.";
                case "hokmaCG2":
                    return "INT. Lab - Flashback" +
                        "\nA and Benjamin stand next to a metallic pod. Inside it is Angela.";
                case "officeBK":
                    return "INT. A Lab in the Outskirts - Flashback";
                case "warehouseBK_2":
                    return "INT. Warehouse - Flashback";
                case "hokmaBeach":
                    return "EXT. Outskirts - Flashback";
                default:
                    return spriteSrc + ", however, this should not be the name you see. Complain to the mod developer.";
            }
        }

        //Postfix - send dialogue along with its speaker directly to Neuro. Only send the speaker if the speaker changes.
        private static string lastSpeaker = "";
        private static bool lastSpeakerWasRobotSephira;
        public static void ContextSpeak(StoryDialogueUI __instance, StoryUI.CharacterVar charVar, string text)
        {
            if (isSkipping) return;
            string finalDialogue = "";
            switch (GetDialogueType(__instance))
            {
                case StoryDialogueBoxType.DEFAULT:
                    if(charVar != null)
                    {
                        string name = charVar.name;
                        if (String.IsNullOrEmpty(name)) name = "\"\t\"";
                        if (name.Equals("Tiphereth"))
                        {
                            if (charVar.spriteResourceName.Contains("Tiphereth1") || charVar.spriteResourceName.Contains("TipherethRobot_a")) name = "Tiphereth (A)";
                            else name = "Tiphereth (B)";
                        }
                        if (!name.Equals(lastSpeaker))
                        {
                            lastSpeaker = name;
                            finalDialogue += name + ": \n";
                        }
                        else if(!lastSpeakerWasRobotSephira && charVar.spriteResourceName.ToLower().Contains("robot"))
                        {
                            finalDialogue += String.Format("The cognition filter lowers... {0}'s true, robotic body is, now and forever, visible.\n", name);
                        }
                        lastSpeakerWasRobotSephira = (charVar.spriteResourceName.ToLower().Contains("robot"));
                        finalDialogue += String.Format("\"{0}\"", text);
                    }
                    else
                    {
                        if (!lastSpeaker.Equals("NO_SPEAKER"))
                        {
                            lastSpeaker = "NO_SPEAKER";
                            finalDialogue += "\n";
                        }
                        finalDialogue += String.Format("\"{0}\"", text);
                    }
                    break;
                case StoryDialogueBoxType.LETTER:
                    if (!lastSpeaker.Equals("LETTER"))
                    {
                        lastSpeaker = "LETTER";
                        finalDialogue += "Letter: \n";
                    }
                    finalDialogue += String.Format("\"{0}\"", text);
                    break;
                case StoryDialogueBoxType.NARRATION: //i don't know what counts as narration. watch out for this, since all non-quoted context is supposed to be from Angela.
                    lastSpeaker = "NARRATOR";
                    finalDialogue = text;
                    break;
            }
            if (lastCG.Equals("ANGELA_PREVENT_DUPE_MESSAGE"))
            {
                lastCG = "INT. Angela's Office - Day";
                NeuroSDKHandler.SendContext(lastCG, true);
            }
            if (String.IsNullOrEmpty(finalDialogue)) return;
            finalDialogue = finalDialogue.Replace("<i>", "");
            finalDialogue = finalDialogue.Replace("</i>", "");
            finalDialogue = finalDialogue.Replace("<b>", "");
            finalDialogue = finalDialogue.Replace("</b>", "");
            NeuroSDKHandler.SendContext(finalDialogue, true);
        }

        private static StoryDialogueBoxType GetDialogueType(StoryDialogueUI __instance)
        {
            if (__instance.defaultDiag.activeSelf) return StoryDialogueBoxType.DEFAULT;
            if (__instance.letterDiag.activeSelf) return StoryDialogueBoxType.LETTER;
            if (__instance.narrationDiag.activeSelf) return StoryDialogueBoxType.NARRATION;
            return StoryDialogueBoxType.HIDE;
        }

        //Postfix - send the selected dialogue option directly to Neuro. also, remove her ability to choose dialogue after it has been selected.
        public static List<string> dialogueOptions = new List<string>(); 
        public static void ContextDialogueOption(StoryUI __instance, StoryUI.StoryScriptCommandEventEnum e, bool __result)
        {
            if(e == StoryUI.StoryScriptCommandEventEnum.EXECUTE)
            {
                return;
            }
            if (!__result) return;
            if (isSkipping) return;
            int selectedDialogueIndex = ((int)e - 2);
            NeuroSDKHandler.SendCommand("dialogue_option_end");
            string dialogue = dialogueOptions[selectedDialogueIndex];
            NeuroSDKHandler.SendContext(String.Format("> {0}", dialogue), true);
        }

        //Postfix - when a dialogue starts, send Neuro the options and give her the ability to pick one.
        public static void StartDialogueOption(string[] options)
        {
            dialogueOptions = new List<string>();
            string finalMessage = "start_dialogue_option";
            foreach (string option in options)
            {
                string dialogue = option;
                dialogue = dialogue.Replace("<i>", "(");
                dialogue = dialogue.Replace("</i>", ")");
                dialogueOptions.Add(dialogue);
                finalMessage += "|" + dialogue;
            }
            if (isSkipping) return;
            NeuroSDKHandler.SendCommand(finalMessage);
        }

        //Postfix - send Neuro the Suppression Required notice
        public static void ContextBossMission()
        {
            NeuroSDKHandler.SendContext("! WARNING ! WARNING ! WARNING ! WARNING ! WARNING !" +
                "\n----------------------------------------------------" +
                "\nMANIFESTATION OF QLIPHA DUE TO SEPHIRAH BREAKDOWN" +
                "\n\nSUPPRESSION OF SEPHIRAH'S CORE REQUIRED" +
                "\n----------------------------------------------------" +
                "\n! WARNING ! WARNING ! WARNING ! WARNING ! WARNING !", false);
        }

        //Postfix - send Neuro the boss clear dialogue
        public static void ContextSeedOfLightGermination(string sefiraText, int to)
        {
            NeuroSDKHandler.SendContext(String.Format("{0}" +
                "\nGermination of the Seed of Light: {1}%", sefiraText, to), false);
        }

        //Postfix - send Neuro the final message
        public static void ContextLORForeshadowing(StoryUI.StoryScriptCommandEventEnum e)
        {
            if (e != StoryUI.StoryScriptCommandEventEnum.VIDEO_END) return;
            NeuroSDKHandler.SendContext("The" +
                "\n\tLibrary" +
                "\n\t\tof" +
                "\n\t\t\tBabel", false);
        }

        //Postfix - tell Neuro when the current story segment ends
        public static void EndStorySegment()
        {
            lastCG = "";
            lastSpeaker = "";
            lastSpeakerWasRobotSephira = false;
            isSkipping = false;
            NeuroSDKHandler.SendContext("END SCENE", true);
        }
    }
}
