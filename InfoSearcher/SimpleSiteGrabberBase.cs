using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace InfoSearcher
{
    public class SimpleSiteGrabberBase
    {
        private readonly MainSettings _mainConfig;
        private readonly string _configFileName;
        private readonly GrabberConfig _config;

        public SimpleSiteGrabberBase(MainSettings mainConfig, string configFileName)
        {
            _mainConfig = mainConfig ?? throw new ArgumentNullException(nameof(mainConfig));
            _configFileName = configFileName;
            _config = JsonConvert.DeserializeObject<GrabberConfig>(File.ReadAllText(configFileName));
        }

        public void Grab()
        {
            using (var wc = new WebClient())
            {
                var data = wc.DownloadString(_config.MainLink);
                var outPutFileName = Path.Combine(_mainConfig.OutputDir, _config.OutName + ".out");
                if (!Directory.Exists(_mainConfig.OutputDir))
                {
                    Directory.CreateDirectory(_mainConfig.OutputDir);
                }

                File.WriteAllText(outPutFileName, data);
            }
        }
    }
}