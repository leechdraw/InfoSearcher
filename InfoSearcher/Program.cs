using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InfoSearcher
{
    class Program
    {
        private static readonly SiteProviderBase[] Providers =
        {
            new SiteWithNews(),
        };

        static void Main(string[] args)
        {
            var settingsFileName = args.Length > 0 ? args[0] : "settings.json";
            var settings = MainSettings.Load(settingsFileName);
            var siteSettingsFiles = CollectSiteConfigs(settings.SiteConfigDir);
            var siteGrabbers = siteSettingsFiles
                .Select(sf => Providers.FirstOrDefault(pr => pr.ConfigMatch(sf))?.CreateGrabber(settings, sf))
                .Where(gr => gr != null)
                .ToList();
            var grabTasks = siteGrabbers.Select(gr => Task.Run(() => gr.Grab())).ToArray();
            Task.WaitAll(grabTasks);
        }

        private static IEnumerable<string> CollectSiteConfigs(string siteConfigDir)
        {
            return Directory.GetFiles(siteConfigDir);
        }
    }
}