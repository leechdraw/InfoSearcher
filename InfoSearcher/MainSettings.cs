using System.IO;
using Newtonsoft.Json;

namespace InfoSearcher
{
    public sealed class MainSettings
    {
        public string OutputDir { get; set; }

        public string SiteConfigDir { get; set; }

        public MailSettings Mail { get; set; }

        public static MainSettings Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            var subResult = JsonConvert.DeserializeObject<MainSettings>(File.ReadAllText(fileName));

            return subResult;
        }
    }
}