namespace InfoSearcher
{
    public class MailSettings
    {
        public string BodyHtmlTemplate { get; set; }

        public string Subject { get; set; }

        public string From { get; set; }

        public string[] To { get; set; }

        public string SmtpServer { get; set; }

        public int? SmtpPort { get; set; }

        public string SmtpLogin { get; set; }

        public string SmtpPass { get; set; }

        public bool? SmtpUseSsl { get; set; }
    }
}