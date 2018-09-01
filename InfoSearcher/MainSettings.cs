using System.IO;
using Newtonsoft.Json;

namespace InfoSearcher
{
    public sealed class MainSettings
    {
        public string OutputDir { get; set; }
        
        public string SiteConfigDir { get; set; }

        public string BodyHtmlTemplate { get; set; }

        public static MainSettings Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            return JsonConvert.DeserializeObject<MainSettings>(File.ReadAllText(fileName));
        }
    }
}