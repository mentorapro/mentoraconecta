using MentoraConecta.Models;
using AdaptiveCards.Templating;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;

using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace MentoraConecta
{
    public sealed class NotifyHttpTrigger
    {
        private readonly ConversationBot _conversation;
        private readonly ILogger<NotifyHttpTrigger> _log;

        public NotifyHttpTrigger(ConversationBot conversation, ILogger<NotifyHttpTrigger> log)
        {
            _conversation = conversation;
            _log = log;
        }

        [FunctionName("NotifyHttpTrigger")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "api/notification")] HttpRequest req, ExecutionContext context)
        {
            _log.LogInformation("NotifyHttpTrigger is triggered.");

            // Read adaptive card template
            var adaptiveCardFilePath = Path.Combine(context.FunctionAppDirectory, "Resources", "NotificationDefault.json");
            var cardTemplate = await File.ReadAllTextAsync(adaptiveCardFilePath, req.HttpContext.RequestAborted);

            var installations = await _conversation.Notification.GetInstallationsAsync(req.HttpContext.RequestAborted);
            var whatsAppCallBack = "";

            using (StreamReader reader = new StreamReader(req.Body))
                whatsAppCallBack = await reader.ReadToEndAsync();

            var wpEntry = JsonConvert.DeserializeObject<WhatsAppMessageModel>(whatsAppCallBack) ?? new WhatsAppMessageModel();
            
            foreach (var installation in installations)
            {

                // Build and send adaptive card
                var cardContent = new AdaptiveCardTemplate(cardTemplate).Expand
                (
                    new NotificationDefaultModel
                    {
                        Title = "WhatsApp Conversation has oppneded!",
                        AppName = "Mentora Conecta",
                        NotificationUrl = "https://www.adaptivecards.io/",
                        ClientName = wpEntry.Body.Entry[0].Changes[0].Value.Contacts[0].Profile.Name,
                        Message = wpEntry.Body.Entry[0].Changes[0].Value.Messages[0].Text.Body,
                        CreatedUtc = DateTimeOffset.FromUnixTimeSeconds(wpEntry.Body.Entry[0].Changes[0].Value.Messages[0].TimeStamp)
                            .ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss")
                    }
                );
                await installation.SendAdaptiveCard(JsonConvert.DeserializeObject(cardContent), req.HttpContext.RequestAborted);
            }

            return new OkResult();
        }
    }
}
