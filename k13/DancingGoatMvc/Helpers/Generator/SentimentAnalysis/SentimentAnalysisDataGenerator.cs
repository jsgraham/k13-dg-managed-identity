﻿using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Activities;
using CMS.Automation;
using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Helpers.UniGraphConfig;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Newsletters;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Factories;

namespace DancingGoat.Helpers.Generator
{
    public class SentimentAnalysisDataGenerator
    {
        private const string ACTION_PARAMETERS = @"<form version=""2""><field allowempty=""false"" column=""FormField"" columnsize=""200"" columntype=""text"" guid=""132fd2eb-e2cb-44a5-8a11-3fe2573b0800"" visible=""true""><properties><fieldcaption>Form field</fieldcaption></properties><settings><controlname>FormFieldSelector</controlname><FieldsDataType>longtext</FieldsDataType></settings></field></form>";
        private static readonly Guid AUTOMATION_GUID = Guid.Parse("6f3bd127-1a8f-4fc3-9c46-87d4806973e8");
        private static readonly Guid STEP_START_GUID = Guid.Parse("1f9481fd-a5fb-41bb-b388-7be2da6e6408");
        private static readonly Guid STEP_SA_GUID = Guid.Parse("98268f0b-d903-4f78-8ee9-f08be61f11c4");
        private static readonly Guid STEP_CONDITION_GUID = Guid.Parse("113d5dbe-e6e7-4f30-bc3c-bb90db3d4430");
        private static readonly Guid STEP_APOLOGY_GUID = Guid.Parse("8e81b988-8d66-4936-adad-d2a2803c7019");
        private static readonly Guid STEP_NOTIFICATION_NEUTRAL_GUID = Guid.Parse("308ad43f-8057-4121-9804-62053ef10c2b");
        private static readonly Guid STEP_NOTIFICATION_NEGATIVE_GUID = Guid.Parse("f174295f-96e1-4fe1-b4d4-34a8f29a5327");
        private static readonly Guid STEP_CONFIRMATION_GUID = Guid.Parse("646d00b1-e50e-4f07-9c7c-b6866231f842");
        private static readonly Guid STEP_FINISHED_GUID = Guid.Parse("271f345b-4549-462d-97a9-55b5267e106b");

        private static readonly Guid TRIGGER_GUID = Guid.Parse("93bc3fd7-3466-48bc-a080-e30491e1d641");

        private static readonly Guid CONDITION_SOURCE_POINT_ELSE = Guid.Parse("e236c022-eb43-4646-8652-865cda8d5874");
        private static readonly Guid CONDITION_SOURCE_POINT_NEUTRAL = Guid.Parse("02aa973e-5144-42b3-9cca-ce6a1e626417");
        private static readonly Guid CONDITION_SOURCE_POINT_NEGATIVE = Guid.Parse("df93dde1-2fa2-492f-8ce5-0197ce251968");

        private static readonly Guid SA_ACTION_GUID = Guid.Parse("a79eadaf-2b57-46d0-b5bf-3e7f1d1fe02d");
        private static readonly Guid TRANSACTIONAL_EMAIL_ACTION = Guid.Parse("a578a254-b251-4f1e-abb7-533e5898f012");
        private static readonly Guid SEND_NEWSLETTER_ACTION = Guid.Parse("b33d5c68-235d-406e-89d7-4f1714078940");
      

        private const string CONTACT_US_FORM = "DancingGoatMvcContactUsNew";

        private readonly ISiteInfo site;
        private WorkflowActionInfo sentimentAnalysisAction;


        public SentimentAnalysisDataGenerator(ISiteInfo site)
        {
            this.site = site;
        }


        public void Generate()
        {
            SentimentAnalysisMacroRuleGenerator.Generate();

            new SentimentAnalysisApologyEmailGenerator(site).Generate();

            CreateActionStep();

            CreateAutomationProcess();
        }


        private void CreateAutomationProcess()
        {
            var automation = CreateAutomation();

            CreateTrigger(automation);

            var startStep = CreateStartStep(automation);
            var sentimentAnalysisStep = CreateSentimentAnalysisStep(automation);
            var condition = CreateConditionStep(automation);
            var apologyEmail = CreateApologyEmailStep(automation);
            var notificationNeutral = CreateNotificationNeutralStep(automation);
            var notificationNegative = CreateNotificationNegativeStep(automation);
            var confirmation = CreateConfirmationStep(automation);
            var finished = CreateFinishStep(automation);

            EnsureTransition(automation, startStep, sentimentAnalysisStep);
            EnsureTransition(automation, sentimentAnalysisStep, condition);

            EnsureTransition(automation, condition, finished, CONDITION_SOURCE_POINT_ELSE);
            EnsureTransition(automation, condition, notificationNeutral, CONDITION_SOURCE_POINT_NEUTRAL);
            EnsureTransition(automation, condition, apologyEmail, CONDITION_SOURCE_POINT_NEGATIVE);

            EnsureTransition(automation, apologyEmail, notificationNegative);
            EnsureTransition(automation, notificationNeutral, confirmation);
            EnsureTransition(automation, notificationNegative, confirmation);
            EnsureTransition(automation, confirmation, finished, type: WorkflowTransitionTypeEnum.Manual);
        }


        private WorkflowStepInfo CreateFinishStep(WorkflowInfo automation)
        {
            var step = WorkflowStepInfo.Provider.Get(STEP_FINISHED_GUID);
            if (step != null)
            {
                return step;
            }

            step = CreateWorkflowStep(automation, STEP_FINISHED_GUID, "Finished", "Finished", WorkflowStepTypeEnum.Finished);
            step.StepDefinition = CreateStepDefinition(WorkflowStepTypeEnum.Finished, 2830, 960, Guid.NewGuid());
            step.StepAllowReject = true;
            step.StepSendApproveEmails = true;
            step.StepSendEmails = true;
            step.StepSendReadyForApprovalEmails = true;
            step.StepSendRejectEmails = true;

            step.Insert();

            return step;
        }


        private WorkflowStepInfo CreateConfirmationStep(WorkflowInfo automation)
        {
            var step = WorkflowStepInfo.Provider.Get(STEP_CONFIRMATION_GUID);
            if (step != null)
            {
                return step;
            }

            step = CreateWorkflowStep(automation, STEP_CONFIRMATION_GUID, "Confirmation_ContactProcessed", "Confirmation - contact processed", WorkflowStepTypeEnum.Standard);
            step.StepDefinition = CreateStepDefinition(WorkflowStepTypeEnum.Standard, 3180, 800, Guid.NewGuid());
            step.StepAllowReject = true;
            step.StepSendApproveEmails = true;
            step.StepSendEmails = true;
            step.StepSendReadyForApprovalEmails = true;
            step.StepSendRejectEmails = true;

            step.Insert();

            return step;
        }


        private WorkflowStepInfo CreateNotificationNegativeStep(WorkflowInfo automation)
        {
            var step = WorkflowStepInfo.Provider.Get(STEP_NOTIFICATION_NEGATIVE_GUID);
            if (step != null)
            {
                return step;
            }

            var sendTransactionalEmailAction = WorkflowActionInfo.Provider.Get(TRANSACTIONAL_EMAIL_ACTION);
            step = CreateWorkflowStep(automation, STEP_NOTIFICATION_NEGATIVE_GUID, "SendNotificationToContactOwner_Negative", "Send notification to contact owner (negative)", WorkflowStepTypeEnum.Action);
            step.StepDefinition = CreateStepDefinition(WorkflowStepTypeEnum.Action, 2980, 650, Guid.NewGuid());
            step.StepActionParameters.SetValue("BasedOn", "1");
            step.StepActionParameters.SetValue("From", "dancinggoat@localhost.local");
            step.StepActionParameters.SetValue("To", "dancinggoat.contactowner@localhost.local");
            step.StepActionParameters.SetValue("Subject", "New contact message (negative)");
            step.StepActionParameters.SetValue("Body", "The contact &#39;" + SignMacro("{% Contact.ContactEmail%}") + "&#39; just sent us a message via the &#39;Contact Us&#39; form where negative text sentiment was detected.<br /><br />Please clear things up with the contact and make sure they receive some special care.");
            step.StepActionID = sendTransactionalEmailAction.ActionID;

            step.Insert();

            return step;
        }


        private WorkflowStepInfo CreateNotificationNeutralStep(WorkflowInfo automation)
        {
            var step = WorkflowStepInfo.Provider.Get(STEP_NOTIFICATION_NEUTRAL_GUID);
            if (step != null)
            {
                return step;
            }

            var sendTransactionalEmailAction = WorkflowActionInfo.Provider.Get(TRANSACTIONAL_EMAIL_ACTION);
            step = CreateWorkflowStep(automation, STEP_NOTIFICATION_NEUTRAL_GUID, "SendNotificationToContactOwner", "Send notification to contact owner (neutral or mixed)", WorkflowStepTypeEnum.Action);
            step.StepDefinition = CreateStepDefinition(WorkflowStepTypeEnum.Action, 3240, 480, Guid.NewGuid());
            step.StepActionParameters.SetValue("BasedOn", "1");
            step.StepActionParameters.SetValue("From", "dancinggoat@localhost.local");
            step.StepActionParameters.SetValue("To", "dancinggoat.contactowner@localhost.local");
            step.StepActionParameters.SetValue("Subject", "New contact message");
            step.StepActionParameters.SetValue("Body", "The contact &#39;" + SignMacro("{% Contact.ContactEmail%}") + "&#39; just sent us a message via the 'Contact Us' form.");

            step.StepActionID = sendTransactionalEmailAction.ActionID;

            step.Insert();

            return step;
        }


        private WorkflowStepInfo CreateApologyEmailStep(WorkflowInfo automation)
        {
            var step = WorkflowStepInfo.Provider.Get(STEP_APOLOGY_GUID);
            if (step != null)
            {
                return step;
            }

            var newsletter = NewsletterInfo.Provider.Get(SentimentAnalysisApologyEmailGenerator.NEWSLETTER_NAME, site.SiteID);
            var sendNewsletterIssueAction = WorkflowActionInfo.Provider.Get(SEND_NEWSLETTER_ACTION);
            step = CreateWorkflowStep(automation, STEP_APOLOGY_GUID, "SendApologyEmail", "Send apology email", WorkflowStepTypeEnum.Action);
            step.StepDefinition = CreateStepDefinition(WorkflowStepTypeEnum.Action, 2850, 480, Guid.NewGuid());
            step.StepActionParameters.SetValue("Newsletter", newsletter.NewsletterGUID);
            step.StepActionParameters.SetValue("NewsletterIssue", SentimentAnalysisApologyEmailGenerator.ISSUE_GUID);
            step.StepActionParameters.SetValue("Site", site.SiteName);
            step.StepActionID = sendNewsletterIssueAction.ActionID;

            step.Insert();

            return step;
        }

        private WorkflowStepInfo CreateConditionStep(WorkflowInfo automation)
        {
            var step = WorkflowStepInfo.Provider.Get(STEP_CONDITION_GUID);
            if (step != null)
            {
                return step;
            }

            step = CreateWorkflowStep(automation, STEP_CONDITION_GUID, "If_Else", "If/Else", WorkflowStepTypeEnum.MultichoiceFirstWin);
            var definition = CreateStepDefinition(WorkflowStepTypeEnum.Condition, 2540, 320);

            definition.SourcePoints.Add(new CMS.WorkflowEngine.Definitions.ConditionSourcePoint
            {
                Guid = CONDITION_SOURCE_POINT_NEUTRAL,
                Type = SourcePointTypeEnum.SwitchCase,
                Label = "If neutral or mixed",
                Condition = SignMacro(@"{%Rule(""(AutomationState.StateCustomData[\""SENTIMENT_ANALYSIS_RESULT\""] == 1) || (AutomationState.StateCustomData[\""SENTIMENT_ANALYSIS_RESULT\""] == 3)"", ""<rules><r pos=\""0\"" par=\""\"" op=\""or\"" n=\""" + SentimentAnalysisMacroRuleGenerator.MACRO_RULE_NAME + @"\"" ><p n=\""Sentiment\""><t>Neutral</t><v>1</v><r>0</r><d>Sentiment</d><vt>text</vt><tv>0</tv></p></r><r pos=\""1\"" par=\""\"" op=\""or\"" n=\""" + SentimentAnalysisMacroRuleGenerator.MACRO_RULE_NAME + @"\"" ><p n=\""Sentiment\""><t>Mixed</t><v>3</v><r>0</r><d>Sentiment</d><vt>text</vt><tv>0</tv></p></r></rules>"") %}")
            });

            definition.SourcePoints.Add(new CMS.WorkflowEngine.Definitions.CaseSourcePoint
            {
                Guid = CONDITION_SOURCE_POINT_NEGATIVE,
                Type = SourcePointTypeEnum.SwitchCase,
                Label = "If negative sentiment",
                Condition = SignMacro(@"{%Rule(""(AutomationState.StateCustomData[\""SENTIMENT_ANALYSIS_RESULT\""] == 2)"", ""<rules><r pos=\""0\"" par=\""\"" op=\""and\"" n=\""" + SentimentAnalysisMacroRuleGenerator.MACRO_RULE_NAME + @"\"" ><p n=\""Sentiment\""><t>Negative</t><v>2</v><r>0</r><d>Sentiment</d><vt>text</vt><tv>0</tv></p></r></rules>"") %}")
            });

            definition.SourcePoints.Add(new CMS.WorkflowEngine.Definitions.ElseSourcePoint
            {
                Guid = CONDITION_SOURCE_POINT_ELSE,
                Type = SourcePointTypeEnum.SwitchDefault,
                Label = "If positive or unknown sentiment"
            }); ;

            step.StepDefinition = definition;
            step.Insert();

            return step;
        }


        private WorkflowStepInfo CreateSentimentAnalysisStep(WorkflowInfo automation)
        {
            var step = WorkflowStepInfo.Provider.Get(STEP_SA_GUID);
            if (step != null)
            {
                return step;
            }

            step = CreateWorkflowStep(automation, STEP_SA_GUID, "SentimentAnalysis", "Form sentiment analysis", WorkflowStepTypeEnum.Action);
            step.StepDefinition = CreateStepDefinition(WorkflowStepTypeEnum.Action, 2740, 160, Guid.NewGuid());
            step.StepActionParameters.SetValue("FormField", $"{site.SiteName};{CONTACT_US_FORM};UserMessage");
            step.StepActionID = sentimentAnalysisAction.ActionID;
            step.Insert();

            return step;
        }

        private WorkflowStepInfo CreateStartStep(WorkflowInfo automation)
        {
            var step = WorkflowStepInfo.Provider.Get(STEP_START_GUID);
            if (step != null)
            {
                return step;
            }

            step = CreateWorkflowStep(automation, STEP_START_GUID, "start", "Start", WorkflowStepTypeEnum.Start);
            step.StepDefinition = CreateStepDefinition(WorkflowStepTypeEnum.Start, 3070, 40, Guid.NewGuid());
            step.StepSendApproveEmails = true;
            step.Insert();

            return step;
        }


        private WorkflowInfo CreateAutomation()
        {
            var automation = WorkflowInfo.Provider.Get(AUTOMATION_GUID);
            if (automation != null)
            {
                return automation;
            }

            automation = new WorkflowInfo()
            {
                WorkflowGUID = AUTOMATION_GUID,
                WorkflowDisplayName = $"Analyze messages from Contact us form ({site.DisplayName})",
                WorkflowName = $"{site.SiteName}.AnalyzeContactUsForm",
                WorkflowEnabled = true,
                WorkflowLastModified = new DateTime(2021, 5, 6, 0, 0, 0),
                WorkflowType = WorkflowTypeEnum.Automation,
                WorkflowRecurrenceType = ProcessRecurrenceTypeEnum.NonConcurrentRecurring
            };

            WorkflowInfo.Provider.Set(automation);

            return automation;
        }


        private static void CreateTrigger(WorkflowInfo automation)
        {
            var trigger = ObjectWorkflowTriggerInfo.Provider.Get().WhereEquals("TriggerGUID", TRIGGER_GUID).FirstOrDefault();
            if (trigger != null)
            {
                return;
            }

            var rule = @"{%Rule(""(Activity.LinkedToObject(\""cms.form\"", \""" + CONTACT_US_FORM + @"\""))"", ""<rules><r pos=\""0\"" par=\""\"" op=\""and\"" n=\""CMSActivityFormSubmissionPerformedForForm\"" ><p n=\""form\""><t>Contact Us</t><v>" + CONTACT_US_FORM + @"</v><r>1</r><d>select form</d><vt>text</vt><tv>0</tv></p><p n=\""_was\""><t>was</t><v></v><r>0</r><d>select operation</d><vt>text</vt><tv>0</tv></p></r></rules>"") %}";
            rule = MacroSecurityProcessor.AddSecurityParameters(rule, MacroIdentityOption.FromUserInfo(UserInfoProvider.AdministratorUser), null);

            trigger = new ObjectWorkflowTriggerInfo()
            {
                TriggerGUID = TRIGGER_GUID,
                TriggerDisplayName = "Contact submitted Contact Us form",
                TriggerObjectType = ActivityInfo.OBJECT_TYPE,
                TriggerTargetObjectID = GetBizFormSubmitActivityID(),
                TriggerTargetObjectType = ActivityTypeInfo.OBJECT_TYPE,
                TriggerType = WorkflowTriggerTypeEnum.Creation,
                TriggerMacroCondition = rule,
                TriggerWorkflowID = automation.WorkflowID
            };

            trigger.Insert();
        }


        private static void EnsureTransition(WorkflowInfo automation, WorkflowStepInfo source, WorkflowStepInfo target, Guid? sourcePointGuid = null, WorkflowTransitionTypeEnum type = WorkflowTransitionTypeEnum.Automatic)
        {
            var transition = WorkflowTransitionInfo.Provider.Get()
                            .WhereEquals("TransitionStartStepID", source.StepID)
                            .WhereEquals("TransitionEndStepID", target.StepID)
                            .FirstOrDefault();

            if (transition == null)
            {
                transition = CreateTransition(automation, source, target, sourcePointGuid, type);
                transition.Insert();
            }
        }


        private static WorkflowTransitionInfo CreateTransition(WorkflowInfo automation, WorkflowStepInfo sourece, WorkflowStepInfo target, Guid? sourcePointGuid = null, WorkflowTransitionTypeEnum type = WorkflowTransitionTypeEnum.Automatic)
        {
            return new WorkflowTransitionInfo
            {
                TransitionStartStepID = sourece.StepID,
                TransitionSourcePointGUID = sourcePointGuid ?? sourece.StepDefinition.SourcePoints.First().Guid,
                TransitionEndStepID = target.StepID,
                TransitionType = type,
                TransitionWorkflowID = automation.WorkflowID,
            };
        }


        private static string SignMacro(string macro)
        {
            return MacroSecurityProcessor.AddSecurityParameters(macro, MacroIdentityOption.FromUserInfo(UserInfoProvider.AdministratorUser), null);
        }


        private static WorkflowStepInfo CreateWorkflowStep(WorkflowInfo automation, Guid guid, string name, string displayName, WorkflowStepTypeEnum type)
        {
            return new WorkflowStepInfo()
            {
                StepGUID = guid,
                StepAllowReject = false,
                StepDisplayName = displayName,
                StepName = name,
                StepRolesSecurity = WorkflowStepSecurityEnum.Default,
                StepUsersSecurity = WorkflowStepSecurityEnum.Default,
                StepWorkflowID = automation.WorkflowID,
                StepType = type,
                StepSendApproveEmails = false,
                StepSendEmails = false,
                StepSendRejectEmails = false,
                StepSendReadyForApprovalEmails = false,
                StepWorkflowType = WorkflowTypeEnum.Automation
            };
        }

        private static CMS.WorkflowEngine.Definitions.Step CreateStepDefinition(WorkflowStepTypeEnum stepType, int positionX, int positionY, Guid? sourcePointGuid = null)
        {
            var step = StepFactory.CreateStep(stepType);
            step.TimeoutEnabled = false;
            step.TimeoutTarget = Guid.Empty;
            step.Position = new GraphPoint(positionX, positionY);
            
            step.SourcePoints = new List<CMS.WorkflowEngine.Definitions.SourcePoint>();
            if (sourcePointGuid != null)
            {
                step.SourcePoints.Add(GetSourcePoint(sourcePointGuid.Value));
            }
            return step;
        }

        private static CMS.WorkflowEngine.Definitions.SourcePoint GetSourcePoint(Guid guid)
        {
            return new CMS.WorkflowEngine.Definitions.SourcePoint
            {
                Guid = guid,
                Label = "Default",
                StepRolesSecurity = WorkflowStepSecurityEnum.Default,
                StepUsersSecurity = WorkflowStepSecurityEnum.Default
            };
        }

        private static int GetBizFormSubmitActivityID()
        {
            return ActivityTypeInfo.Provider.Get("bizformsubmit").ActivityTypeID;
        }


        private void CreateActionStep()
        {
            sentimentAnalysisAction = WorkflowActionInfo.Provider.Get(SA_ACTION_GUID);
            if (sentimentAnalysisAction != null)
            {
                return;
            }

            sentimentAnalysisAction = new WorkflowActionInfo()
            {
                ActionGUID = SA_ACTION_GUID,
                ActionWorkflowType = WorkflowTypeEnum.Automation,
                ActionDisplayName = "Form sentiment analysis",
                ActionName = "FormSentimentAnalysis",
                ActionParameters = ACTION_PARAMETERS,
                ActionIconClass = "icon-magnifier-emoticon",
                ActionThumbnailClass = "icon-magnifier-emoticon",
                ActionAssemblyName = "CMS.UIControls",
                ActionClass = "CMS.UIControls.Samples.SentimentAnalysisAction",
                ActionDataProviderAssemblyName = "CMS.UIControls",
                ActionDataProviderClass = "CMS.UIControls.Samples.SentimentAnalysisDataProvider",
                ActionAllowedObjects = ";om.contact;",
                ActionLastModified = new DateTime(2021, 5, 6, 0, 0, 0),
                ActionEnabled = true,
                ActionDescription = "Sample step that performs sentiment analysis for the content of a selected form field."
            };

            WorkflowActionInfo.Provider.Set(sentimentAnalysisAction);
        }
    }
}
