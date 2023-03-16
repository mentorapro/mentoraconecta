using teams.app.mpwti.Models;
using AdaptiveCards.Templating;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;

namespace teams.app.mpwti.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ConversationBot _conversation;
        private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "NotificationDefault.json");

        public NotificationController(ConversationBot conversation)
        {
            this._conversation = conversation;
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(CancellationToken cancellationToken = default)
        {
            // Read adaptive card template
            var cardTemplate = await System.IO.File.ReadAllTextAsync(_adaptiveCardFilePath, cancellationToken);

            var installations = await this._conversation.Notification.GetInstallationsAsync(cancellationToken);
            var whatsAppCallBack = "";

            using (StreamReader reader = new StreamReader(Request.Body))
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
                        ClientName = wpEntry.Entry[0].Changes[0].Value.Contacts[0].Profile.Name,
                        Message = wpEntry.Entry[0].Changes[0].Value.Messages[0].Text.Body,
                        CreatedUtc = DateTimeOffset.FromUnixTimeSeconds(wpEntry.Entry[0].Changes[0].Value.Messages[0].TimeStamp)
                            .ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss")
                    }
                );
                await installation.SendAdaptiveCard(JsonConvert.DeserializeObject(cardContent), cancellationToken);
            }
            return Ok();
        }
    }
}
