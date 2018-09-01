using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using CsQuery;
using InfoSearcher.Tools;
using Newtonsoft.Json;

namespace InfoSearcher
{
    public class SimpleSiteGrabberBase
    {
        private readonly MainSettings _mainConfig;
        private readonly GrabberConfig _config;

        public SimpleSiteGrabberBase(MainSettings mainConfig, string configFileName)
        {
            _mainConfig = mainConfig ?? throw new ArgumentNullException(nameof(mainConfig));
            _config = JsonConvert.DeserializeObject<GrabberConfig>(File.ReadAllText(configFileName));
        }

        public void Grab()
        {
            var data = GetData(_config.MainLink);
            var outputDir = Path.Combine(_mainConfig.OutputDir, _config.OutName + ".out");
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var cq = CQ.Create(data);
            var newsLink = cq.Find(_config.NewsListSelector).Select(x => x.GetAttribute("href")).Where(IsNewLink);

            foreach (var link in newsLink)
            {
                var newsData = GetData(link);
                var newsCq = CQ.Create(newsData);
                var content = newsCq.Select(_config.NewsContentSelector);
                var normalizedSearchList = _config.SearchFor.Select(x => x.ToLowerInvariant()).ToHashSet();
                var normalizedContent = string.Join("\r\n", content.Select(x=>x.InnerText)).ToLowerInvariant();
                var isMatched = normalizedSearchList.Any(x => normalizedContent.Contains(x));
                if (isMatched)
                {
                    var outData = new OutPutData
                    {
                        Content = content.Html(),
                        Url = link,
                        MainLink = _config.MainLink
                    };
                    SaveDataToFile(outData, outputDir);
                }
            }
        }

        private static void SaveDataToFile(OutPutData outData, string outputDir)
        {
            var fileName = Path.Combine(outputDir, outData.Url.GetHashString() + ".dat");
            if (File.Exists(fileName))
            {
                return;
            }
            File.WriteAllText(fileName, JsonConvert.SerializeObject(outData));
        }

        private bool IsNewLink(string arg)
        {
            // todo create storage for links and use it here
            return true;
        }

        private static string GetData(string url)
        {
            using (var wc = new WebClient())
            {
                return wc.DownloadString(url);
            }
        }
    }
}