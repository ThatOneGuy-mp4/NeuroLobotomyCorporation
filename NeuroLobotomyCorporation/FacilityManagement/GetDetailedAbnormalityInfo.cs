using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NeuroLobotomyCorporation.FacilityManagement
{
    public class GetDetailedAbnormalityInfo
    {
        public enum Parameters
        {
            ABNORMALITY_NAME = 1,
            INCLUDE_BASIC_INFO = 2,
            INCLUDE_MANAGERIAL_GUIDELINES = 3,
            INCLUDE_WORK_SUCCESS_RATES = 4,
            INCLUDE_ESCAPE_INFORMATION = 5
        }

        public static string Command(string abnormalityName, bool includeBasicInfo, bool includeManagerialGuidelines, bool includeWorkSuccessRates, bool includeEscapeInformation)
        {
            CreatureModel discard = null;
            if (!Helpers.AbnormalityExists(abnormalityName, out discard)) return "failure|Action failed. The specified Abnormality does not exist.";
            ThreadPool.QueueUserWorkItem(CommandExecute, new GetDetailedAbnormalityInfoState(abnormalityName, includeBasicInfo, includeManagerialGuidelines, includeWorkSuccessRates, includeEscapeInformation));
            return String.Format("success|Getting detailed information for {0} to send as context...", abnormalityName);
        }

        private class GetDetailedAbnormalityInfoState
        {
            public string AbnormalityName
            {
                get
                {
                    return abnormalityName;
                }
            }
            private string abnormalityName;

            public bool IncludeBasicInfo
            {
                get
                {
                    return includeBasicInfo;
                }
            }
            private bool includeBasicInfo;

            public bool IncludeManagerialGuidelines
            {
                get
                {
                    return includeManagerialGuidelines;
                }
            }
            private bool includeManagerialGuidelines;

            public bool IncludeWorkSuccessRates
            {
                get
                {
                    return includeWorkSuccessRates;
                }
            }
            private bool includeWorkSuccessRates;

            public bool IncludeEscapeInformation
            {
                get
                {
                    return includeEscapeInformation;
                }
            }
            private bool includeEscapeInformation;

            public GetDetailedAbnormalityInfoState(string abnormalityName, bool includeBasicInfo, bool includeManagerialGuidelines, bool includeWorkSuccessRates, bool includeEscapeInformation)
            {
                this.abnormalityName = abnormalityName;
                this.includeBasicInfo = includeBasicInfo;
                this.includeManagerialGuidelines = includeManagerialGuidelines;
                this.includeWorkSuccessRates = includeWorkSuccessRates;
                this.includeEscapeInformation = includeEscapeInformation;
            }
        }

        //I think lob corp has a passive code-readability-debuffing-aura
        //(TODO: clean up all this stuff generally, prolly put them in their own methods)
        private static void CommandExecute(object state)
        {
            GetDetailedAbnormalityInfoState infoState = (GetDetailedAbnormalityInfoState)state;
            CreatureModel abnormality = null;
            if (!Helpers.AbnormalityExists(infoState.AbnormalityName, out abnormality)) return;
            string info = "";
            if (abnormality.metaInfo.creatureWorkType == CreatureWorkType.NORMAL)
            {
                string department = Helpers.GetDepartmentBySefira(abnormality.sefira.sefiraEnum);
                string abnormalityStatus = "";
                switch (Helpers.GetAbnormalityWorkingState(abnormality))
                {
                    case Helpers.AbnormalityWorkingState.BREACHING:
                        abnormalityStatus = "Breaching";
                        break;
                    case Helpers.AbnormalityWorkingState.WORKING:
                        abnormalityStatus = "In Work";
                        break;
                    case Helpers.AbnormalityWorkingState.IDLE:
                        abnormalityStatus = "Idle";
                        break;
                    case Helpers.AbnormalityWorkingState.COOLDOWN:
                        abnormalityStatus = "On Cooldown";
                        break;
                }
                info += String.Format("{0}, who is a {1} Department Abnormality, is currently {2}.\n", infoState.AbnormalityName, department, abnormalityStatus);
                if (infoState.IncludeBasicInfo)
                {
                    if (!abnormality.observeInfo.GetObserveState(CreatureModel.regionName[(int)Helpers.ObserveInfoIndexes.BasicInfo]))
                    {
                        string basicInfoCost = abnormality.observeInfo.GetObserveCost(CreatureModel.regionName[(int)Helpers.ObserveInfoIndexes.BasicInfo]).ToString();
                        info += String.Format("{0}'s basic info must be unlocked for {1} personal P.E. Boxes before it can be read.\n", infoState.AbnormalityName, basicInfoCost);
                    }
                    else
                    {
                        string riskLevel = String.Format("Risk Level: {0}", abnormality.metaInfo.riskLevelForce);
                        string peBoxBounds = "";
                        for (int i = 0; i < abnormality.metaInfo.feelingStateCubeBounds.upperBounds.Length; i++)
                        {
                            string boundName = "";
                            switch (i)
                            {
                                case 0:
                                    boundName = "Bad";
                                    break;
                                case 1:
                                    boundName = "Normal";
                                    break;
                                case 2:
                                    boundName = "Good";
                                    break;
                            }
                            string lowerBound = "";
                            if (i == 0) lowerBound = "0";
                            else lowerBound = (abnormality.metaInfo.feelingStateCubeBounds.upperBounds[i - 1] + 1).ToString();
                            string upperBound = abnormality.metaInfo.feelingStateCubeBounds.upperBounds[i].ToString();
                            peBoxBounds += String.Format("{0} Work Result: {1}-{2} P.E. Boxes Generated\n", boundName, lowerBound, upperBound);
                        }

                        string workDamageType = Helpers.GetDamageColorByRwbpType(abnormality.metaInfo.workDamage.type);
                        string damageLowerRange = abnormality.metaInfo.workDamage.min.ToString();
                        string damageUpperRange = abnormality.metaInfo.workDamage.max.ToString();
                        string workDamageInfo = String.Format("Work Damage: {0}-{1} {2} Damage", damageLowerRange, damageUpperRange, workDamageType);

                        string qliphothCounterInfo = "";
                        if (!abnormality.script.HasRoomCounter()) qliphothCounterInfo = "This Abnormality has no Qliphoth Counter.";
                        else qliphothCounterInfo = String.Format("Current Qliphoth Counter: {0}", abnormality.qliphothCounter.ToString());
                        info += String.Format("{0}\n{1}{2}\n{3}\n", riskLevel, peBoxBounds, workDamageInfo, qliphothCounterInfo);
                    }
                }

                if (infoState.IncludeEscapeInformation)
                {
                    if (!abnormality.observeInfo.GetObserveState(CreatureModel.regionName[(int)Helpers.ObserveInfoIndexes.EscapeInfo]))
                    {
                        string escapeInformationCost = abnormality.observeInfo.GetObserveCost(CreatureModel.regionName[(int)Helpers.ObserveInfoIndexes.EscapeInfo]).ToString();
                        info += String.Format("{0}'s escape information must be unlocked for {1} personal P.E. Boxes before it can be read.\n", infoState.AbnormalityName, escapeInformationCost);
                    }
                    else
                    {
                        string maxQliphothCounter = "";
                        if (abnormality.script.HasRoomCounter()) maxQliphothCounter = String.Format("Max Qliphoth Counter: {0}\n", abnormality.script.GetQliphothCounterMax());
                        string ableToBreach = "";
                        if (!abnormality.metaInfo.isEscapeAble) ableToBreach = String.Format("{0} is a non-escaping entity.\n", infoState.AbnormalityName);
                        else
                        {
                            List<DefenseInfo> defenseInfoList = abnormality.metaInfo.defenseTable.GetDefenseInfoList();
                            if (defenseInfoList.Count != 0)
                            {
                                DefenseInfo defenseInfo = defenseInfoList[0];
                                string redDefense = UIUtil.GetDefenseText(defenseInfo.GetDefenseType(defenseInfo.R));
                                string whiteDefense = UIUtil.GetDefenseText(defenseInfo.GetDefenseType(defenseInfo.W));
                                string blackDefense = UIUtil.GetDefenseText(defenseInfo.GetDefenseType(defenseInfo.B));
                                string paleDefense = UIUtil.GetDefenseText(defenseInfo.GetDefenseType(defenseInfo.P));
                                ableToBreach = String.Format("{0} can breach, and has following damage resistances:" +
                                    "\n{1} Red Res." +
                                    "\n{2} White Res." +
                                    "\n{3} Black Res." +
                                    "\n{4} Pale Res.\n", infoState.AbnormalityName, redDefense, whiteDefense, blackDefense, paleDefense);
                            }
                        }
                        info += String.Format("{0}{1}", maxQliphothCounter, ableToBreach);
                    }
                }
                if (infoState.IncludeWorkSuccessRates)
                {
                    info += "Work Success Rates (Level I/II/III/IV/V):\n";
                    //copy and pasting this 4 times is definitely the most efficient way to do this. (TODO: fix it)
                    if (!abnormality.observeInfo.GetObserveState(CreatureModel.regionName[(int)Helpers.ObserveInfoIndexes.InstinctSuccess]))
                    {
                        string instinctInformationCost = abnormality.observeInfo.GetObserveCost(CreatureModel.regionName[(int)Helpers.ObserveInfoIndexes.InstinctSuccess]).ToString();
                        info += String.Format("-{0}'s Instinct success rates must be unlocked for {1} personal P.E. Boxes before they can be read.\n", infoState.AbnormalityName, instinctInformationCost);
                    }
                    else
                    {
                        info += "-Instinct: ";
                        for (int i = 1; i <= 5; i++)
                        {
                            info += UICommonTextConverter.GetPercentText(abnormality.metaInfo.workProbTable.GetWorkProb(RwbpType.R, i));
                            if (i != 5) info += "/";
                            else info += "\n";
                        }
                    }
                    if (!abnormality.observeInfo.GetObserveState(CreatureModel.regionName[(int)Helpers.ObserveInfoIndexes.InsightSuccess]))
                    {
                        string insightInformationCost = abnormality.observeInfo.GetObserveCost(CreatureModel.regionName[(int)Helpers.ObserveInfoIndexes.InsightSuccess]).ToString();
                        info += String.Format("-{0}'s Insight success rates must be unlocked for {1} personal P.E. Boxes before they can be read.\n", infoState.AbnormalityName, insightInformationCost);
                    }
                    else
                    {
                        info += "-Insight: ";
                        for (int i = 1; i <= 5; i++)
                        {
                            info += UICommonTextConverter.GetPercentText(abnormality.metaInfo.workProbTable.GetWorkProb(RwbpType.W, i));
                            if (i != 5) info += "/";
                            else info += "\n";
                        }
                    }
                    if (!abnormality.observeInfo.GetObserveState(CreatureModel.regionName[(int)Helpers.ObserveInfoIndexes.AttachmentSuccess]))
                    {
                        string attachmentInformationCost = abnormality.observeInfo.GetObserveCost(CreatureModel.regionName[(int)Helpers.ObserveInfoIndexes.AttachmentSuccess]).ToString();
                        info += String.Format("-{0}'s Attachment success rates must be unlocked for {1} personal P.E. Boxes before they can be read.\n", infoState.AbnormalityName, attachmentInformationCost);
                    }
                    else
                    {
                        info += "-Attachment: ";
                        for (int i = 1; i <= 5; i++)
                        {
                            info += UICommonTextConverter.GetPercentText(abnormality.metaInfo.workProbTable.GetWorkProb(RwbpType.B, i));
                            if (i != 5) info += "/";
                            else info += "\n";
                        }
                    }
                    if (!abnormality.observeInfo.GetObserveState(CreatureModel.regionName[(int)Helpers.ObserveInfoIndexes.RepressionSuccess]))
                    {
                        string repressionInformationCost = abnormality.observeInfo.GetObserveCost(CreatureModel.regionName[(int)Helpers.ObserveInfoIndexes.RepressionSuccess]).ToString();
                        info += String.Format("-{0}'s Repression success rates must be unlocked for {1} personal P.E. Boxes before they can be read.\n", infoState.AbnormalityName, repressionInformationCost);
                    }
                    else
                    {
                        info += "-Repression: ";
                        for (int i = 1; i <= 5; i++)
                        {
                            info += UICommonTextConverter.GetPercentText(abnormality.metaInfo.workProbTable.GetWorkProb(RwbpType.P, i));
                            if (i != 5) info += "/";
                            else info += "\n";
                        }
                    }
                }
                if (infoState.IncludeManagerialGuidelines)
                {
                    List<CreatureSpecialSkillDesc> guidelines = abnormality.metaInfo.specialSkillTable.descList;
                    int i = 0;
                    int lockedGuidelineIndex = -1;
                    string managerialGuidelines = "Managerial Guidelines\n-------------------------------------\n";
                    while (i < guidelines.Count)
                    {

                        if (!abnormality.observeInfo.GetObserveState(CreatureModel.careTakingRegion[i]))
                        {
                            lockedGuidelineIndex = i;
                            break;
                        }

                        string original = guidelines[i].original;
                        string text = original;
                        int num = 0;
                        for (; ; )
                        {

                            int num2 = text.IndexOf("#" + num);
                            if (text == string.Empty || text == null)
                            {
                                break;
                            }
                            if (num2 == -1)
                            {
                                break;
                            }
                            AgentName agentName = null;
                            if (!abnormality.metaInfo.GetAgentName(i * 100 + num, out agentName))
                            {
                                agentName = AgentNameList.instance.GetFakeNameByInfo();
                                abnormality.metaInfo.AddAgentName(i * 100 + num, agentName);
                            }
                            text = text.Replace("#" + num, agentName.GetName());
                            num++;
                        }
                        text = text.Replace("$0", abnormality.GetUnitName());
                        managerialGuidelines += String.Format("{0}\n\n", text);
                        i++;
                    }
                    if (lockedGuidelineIndex != -1)
                    {
                        string guidelinesLeftToUnlock = (guidelines.Count - lockedGuidelineIndex).ToString();
                        string guidelineCost = abnormality.observeInfo.GetObserveCost(CreatureModel.careTakingRegion[lockedGuidelineIndex]).ToString();
                        managerialGuidelines += String.Format("There are {0} managerial guidelines left to unlock before they can be read, each of which costs {1} personal P.E. Boxes.", guidelinesLeftToUnlock, guidelineCost);
                    }
                    info += managerialGuidelines;
                }
            }
            else
            {
                string department = Helpers.GetDepartmentBySefira(abnormality.sefira.sefiraEnum);
                string abnormalityStatus = "";
                switch (Helpers.GetAbnormalityWorkingState(abnormality))
                {
                    case Helpers.AbnormalityWorkingState.BREACHING:
                        abnormalityStatus = "Breaching"; //thanks yang 
                        break;
                    case Helpers.AbnormalityWorkingState.WORKING: //TODO: this state includes channeled tools, but it is yet to be tested if this includes equipped tools. so that needs to be tested
                        abnormalityStatus = "Being Used";
                        break;
                    case Helpers.AbnormalityWorkingState.IDLE: //this is kinda wrong in regards to the train specifically. i don't know if it's worth adding an exception for though.
                        abnormalityStatus = "Idle";
                        break;
                    case Helpers.AbnormalityWorkingState.COOLDOWN: //i don't think this is ever the case for tool abnos. but i'll leave it here anyways.
                        abnormalityStatus = "On Cooldown";
                        break;
                }
                info += String.Format("{0}, which is a {1} Department Tool Abnormality, is currently {2}.\n", infoState.AbnormalityName, department, abnormalityStatus);
                //TODO: for equipped or channeled tools, it might be nice to have the agent who is currently channeling or equipping it be listed
                if (infoState.IncludeBasicInfo)
                {
                    //TODO: add the number of times abnormality has been used/time abnormality has been equipped/channeled in here
                    info += String.Format("Risk Level: {0}\n", abnormality.metaInfo.riskLevelForce);
                    info += String.Format("{0} has no Qliphoth Counter.\n", infoState.AbnormalityName); 
                }
                if (infoState.IncludeEscapeInformation)
                {
                    string yangSubtleForeshadowing = "";
                    if (abnormality.metaInfo.isEscapeAble) yangSubtleForeshadowing = "visible "; //thanks yang
                    info += String.Format("As a Tool Abnormality, {0} has no {1}escape information.\n", infoState.AbnormalityName, yangSubtleForeshadowing);
                }
                if (infoState.IncludeWorkSuccessRates) //this is not an accurate variable name for tool abnormalities. i will probably not change it.
                {
                    string toolType = "";
                    switch (abnormality.metaInfo.creatureKitType)
                    {
                        case CreatureKitType.ONESHOT:
                            toolType = "single-use";
                            break;
                        case CreatureKitType.CHANNEL:
                            toolType = "channellable";
                            break;
                        case CreatureKitType.EQUIP:
                            toolType = "equippable";
                            break;
                    }
                    info += String.Format("{0} is {1}.\n", infoState.AbnormalityName, toolType);
                }
                if (infoState.IncludeManagerialGuidelines)
                {
                    info += "Managerial Guidelines\n-------------------------------------";
                    List<int> lockedGuidelines = new List<int>();
                    int i = 0;
                    while(i < abnormality.metaInfo.specialSkillTable.descList.Count)
                    {
                        int observeCost = abnormality.observeInfo.GetObserveCost(CreatureModel.careTakingRegion[i]);
                        if(abnormality.metaInfo.creatureKitType == CreatureKitType.ONESHOT)
                        {
                            if(abnormality.observeInfo.totalKitUseCount < observeCost)
                            {
                                lockedGuidelines.Add(observeCost);
                            }
                            else 
                            {
                                info += String.Format("{0}\n\n", abnormality.metaInfo.specialSkillTable.descList[i].original.Replace("$0", infoState.AbnormalityName));
                            }
                        }
                        else
                        {
                            if(abnormality.observeInfo.totalKitUseTime < observeCost)
                            {
                                lockedGuidelines.Add(observeCost);
                            }
                            else
                            {
                                info += String.Format("{0}\n\n", abnormality.metaInfo.specialSkillTable.descList[i].original.Replace("$0", infoState.AbnormalityName));
                            }
                        }
                        i++;
                    }
                    if(lockedGuidelines.Count > 0)
                    {
                        if(abnormality.metaInfo.creatureKitType == CreatureKitType.ONESHOT)
                        {
                            info += String.Format("{0} guidelines have yet to be unlocked, and will become visible after {1} has been used ", lockedGuidelines.Count, infoState.AbnormalityName);
                            for(int j = 0; j < lockedGuidelines.Count; j++)
                            {
                                if (j != 0 && j == lockedGuidelines.Count - 1) info += "and ";
                                info += lockedGuidelines[j].ToString();
                                if (j != lockedGuidelines.Count - 1) info += ", ";
                            }
                            info += " times.";
                        }
                        else
                        {
                            info += String.Format("{0} guidelines have yet to be unlocked, and will become visible after {1} has been used for ", lockedGuidelines.Count, infoState.AbnormalityName);
                            for (int j = 0; j < lockedGuidelines.Count; j++)
                            {
                                if (j != 0 && j == lockedGuidelines.Count - 1) info += "and ";
                                info += lockedGuidelines[j].ToString();
                                if (j != lockedGuidelines.Count - 1) info += ", ";
                            }
                            info += " seconds.";
                        }
                    }
                }
            }
            NeuroSDKHandler.SendContext(info, true);
        }
    }
}
