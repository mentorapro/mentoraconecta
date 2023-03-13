using Microsoft.Graph;

namespace MentoraConecta.Models
{
    public class NotificationDefaultModel
    {
        public string Title { get; set; }

        public string AppName { get; set; }

        public string NotificationUrl { get; set; }

        public string Message { get; set; }

        public string ClientName { get; set; }

        public string CreatedUtc { get; set; }

    }

    
}
