using System;
using System.Threading.Tasks;
using DurableTask.Core.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using Notification.App.Acitvities;
using Notification.App.Entities;
using Notification.App.Models;

namespace Notification.App.Orchestrator;

    public class SendNotificationOrchestrator
    {
        [Function(nameof(SendNotificationOrchestrator))]
        public async Task<SendNotificationOrchestratorResult> Run([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var input = context.GetInput<SendNotificationOrchestratorInput>();
            // Use Durable Entity to store orchestrator instanceId based on phonenumber
            var entityId = new EntityInstanceId(nameof(NotificationOrchestratorInstanceEntity), input.SupportContact.PhoneNumber);
            await context.Entities.SignalEntityAsync(
                entityId,
                nameof(NotificationOrchestratorInstanceEntity.Set),
                context.InstanceId);

            var activityInput = new SendNotificationActivityInput { 
                Attempt = input.NotificationAttemptCount,
                Message = input.Message,
                PhoneNumber = input.SupportContact.PhoneNumber};
            await context.CallActivityAsync(
                nameof(SendNotificationActivity),
                activityInput);

            var waitTimeBetweenRetry = TimeSpan.FromSeconds(10);//(input.WaitTimeForEscalationInSeconds / input.MaxNotificationAttempts);

            // Orchestrator will wait until the event is received or waitTimeBetweenRetry is passed
            var callBackResult = false;
            try
            { 
                callBackResult = await context.WaitForExternalEvent<bool>("Callback", waitTimeBetweenRetry);
            }
            catch (TaskCanceledException ex)
            {
                if (input.NotificationAttemptCount < input.MaxNotificationAttempts)
                {
                    Retry(context, input);
                }
            }
            if (!callBackResult && input.NotificationAttemptCount < input.MaxNotificationAttempts)
            {
                // Call has not been answered, let's try again!
                Retry(context, input);
            }
            // Call has been answered or MaxNotificationAttempts has been reached.
            return  new SendNotificationOrchestratorResult
            {
                Attempt = input.NotificationAttemptCount,
                PhoneNumber = input.SupportContact.PhoneNumber,
                CallbackReceived = callBackResult
            };
        }

        private static void Retry(TaskOrchestrationContext context, SendNotificationOrchestratorInput input)
        {
            input.NotificationAttemptCount++;
            context.ContinueAsNew(input);
        }
    }