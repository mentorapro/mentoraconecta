using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace teams.app.mpwti.Models
{
    public class WhatsAppMessageModel
    {
        public string Object { get; set; }
        public List<Entry> Entry { get; set; }

        public Body Body { get; set; }
    }
    public class Body
    {
        public string Object { get; set; }
        public List<Entry> Entry { get; set; }
    }
    public class Entry
    {
        public long Id { get; set; }
        public List<Change> Changes { get; set; }
    }
    public class Change
    {
        public Value Value { get; set; }
        public string Field { get; set; }
    }

    public class Value
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; }
        public Metadata Metadata { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<Message> Messages { get; set; }
    }

    public class Metadata
    {
        [JsonPropertyName("display_phone_number")]
        public string DisplayPhoneNumber { get; set; }
        [JsonPropertyName("phone_number_id")]
        public string PhoneNumberId { get; set; }
    }
    public class Contact
    {
        [JsonPropertyName("wa_id")]
        public string WaId { get; set; }
        public Profile Profile { get; set; }
    }
    public class Profile
    {
        public string Name { get; set; }
    }
    public class Message
    {
        public string From { get; set; }
        public string Id { get; set; }
        public long TimeStamp { get; set; }
        public Text Text { get; set; }
        public Types Type { get; set; }
        
    }
    public class Text
    {
        public string Body { get; set; }
    }

    public enum Types
    {
        Text = 1
    }
}
