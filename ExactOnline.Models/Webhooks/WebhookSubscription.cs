using System;

namespace ExactOnline.Models.Webhooks
{
    [SupportedActionsSDK(true, true, false, true)]
    [DataServiceKey("ID")]
    public class WebhookSubscription
    {
        public Guid ID { get; set; }
        public string CallbackURL { get; set; }
        public string Topic { get; set; }
    }
}
