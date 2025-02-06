using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Metadata.Models.WorkflowModel.Steps;
using BaSys.Workflows.Steps;
using System.Linq.Expressions;
using WorkflowCore.Models;

namespace BaSys.Workflows.Infrastructure
{
    public sealed class WorkflowBuilder
    {
        public static WorkflowDefinition Build(WorkflowSettings settings)
        {
            var workflowDefinition = new WorkflowDefinition()
            {
                Id = settings.Name,
                Version = (int)settings.Version,
                DataType = typeof(BaSysWorkflowData)
            };

            var stepId = 0;
            var identifiersIndex = new Dictionary<Guid, int>();

            var startStep = new WorkflowStep<StartStep>();
            startStep.Id = stepId;
            startStep.Name = typeof(StartStep).Name;

            workflowDefinition.Steps.Add(startStep);

            stepId++;

            // Create steps.
            foreach (var stepSettings in settings.Steps)
            {
                if (stepSettings is MessageStepSettings messageStepSettings)
                {
                    var messageStep = new WorkflowStep<MessageStep>();
                    messageStep.Id = stepId;
                    messageStep.Name = typeof(MessageStep).Name;
                    messageStep.ExternalId = messageStepSettings.Uid.ToString();

                    var messageParameter = new MemberMapParameter(
                        (Expression<Func<object, string>>)(data => messageStepSettings.Message),
                        (Expression<Func<MessageStep, string>>)(step => step.Message));

                    messageStep.Inputs.Add(messageParameter);

                    workflowDefinition.Steps.Add(messageStep);

                }
                else if (stepSettings is SleepStepSettings sleepStepSettings)
                {
                    var sleepStep = new WorkflowStep<SleepStep>();
                    sleepStep.Id = stepId;
                    sleepStep.Name = typeof(SleepStep).Name;
                    sleepStep.ExternalId = sleepStepSettings.Uid.ToString();

                    sleepStep.Inputs.Add(
                      new MemberMapParameter(
                      (Expression<Func<object, string>>)(data => sleepStepSettings.Delay),
                      (Expression<Func<SleepStep, string>>)(step => step.Delay))
                      );

                    workflowDefinition.Steps.Add(sleepStep);
                }

                identifiersIndex.Add(stepSettings.Uid, stepId);

                stepId++;
            }

            // Define step outcomes.

            if (workflowDefinition.Steps.Count > 1)
            {
                startStep.Outcomes.Add(new ValueOutcome
                {
                    NextStep = 1
                });
            }


            foreach (var stepSettings in settings.Steps)
            {
                if (stepSettings.PreviousStepUid.HasValue)
                {
                    var currentStepId = identifiersIndex[stepSettings.Uid];
                    var previousStepId = identifiersIndex[stepSettings.PreviousStepUid.Value];

                    var previousStep = workflowDefinition.Steps.FindById(previousStepId);

                    if (previousStep != null)
                    {
                        previousStep.Outcomes.Add(new ValueOutcome
                        {
                            NextStep = currentStepId
                        });
                    }
                }
            }

            return workflowDefinition;
        }
    }
}
