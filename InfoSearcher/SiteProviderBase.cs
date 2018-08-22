using System;
using System.Text.RegularExpressions;

namespace InfoSearcher
{
    public abstract class SiteProviderBase
    {
        private Regex _configFileMatcher;
        protected abstract string ConfigNamePattern { get; }

        public bool ConfigMatch(string fileName)
        {
            if (_configFileMatcher == null)
            {
                _configFileMatcher = new Regex(ConfigNamePattern);
            }

            return _configFileMatcher.IsMatch(fileName);
        }

        public SiteGrabberBase CreateGrabber(MainSettings settings, string sf)
        {
            return new SiteGrabberBase();
        }
    }

    public class SiteGrabberBase
    {
        public void Grab()
        {
            Console.WriteLine("GRabber");
        }
    }
}