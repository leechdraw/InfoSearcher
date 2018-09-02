using System.IO;
using Newtonsoft.Json;

namespace InfoSearcher
{
    public sealed class MainSettings
    {
        public string OutputDir { get; set; }
        
        public string SiteConfigDir { get; set; }

        public string BodyHtmlTemplate { get; set; }

        public string From { get; set; }

        public string[] To { get; set; }

        public string SmtpServer { get; set; }

        public int? SmtpPort { get; set; }

        public string SmtpLogin { get; set; }

        public string SmtpPass { get; set; }
        
        public bool? SmtpUseSsl { get; set; }

        public static MainSettings Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            var subResult = JsonConvert.DeserializeObject<MainSettings>(File.ReadAllText(fileName));

            if (subResult.SmtpPort == null)
            {
                subResult.SmtpPort = 25;
            }

            if (subResult.SmtpUseSsl == null)
            {
                subResult.SmtpUseSsl = false;
            }

            return subResult;
        }
    }
}