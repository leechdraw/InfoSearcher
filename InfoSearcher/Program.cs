using System;
using System.Collections.Generic;
using System.IO;

namespace InfoSearcher
{
    class Program
    {
        private static readonly SiteProviderBase[] _providers = new[]
        {
            new SiteWithNews(),
        };

        static void Main(string[] args)
        {
            /*
             * 1. инициировать провайдеры сайтов их файлами / возможно прочитав конфиг
             * 2. пробежаться по каждому / в несколько потоков - чтобы те сходили по своим делам
             * 3. выгрузить результаты в оутпут папку
             */
            var settings = MainSettings.Load("settings.json");
            var siteSettingsFiles = CollectSiteConfigs(settings.SiteConfigDir);
        }

        private static List<string> CollectSiteConfigs(string siteConfigDir)
        {
            var files = Directory.GetFiles(siteConfigDir);
        }
    }
}