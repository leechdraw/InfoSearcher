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

        public SimpleSiteGrabberBase CreateGrabber(MainSettings settings, string sf)
        {
            return new SimpleSiteGrabberBase(settings, sf);
        }
    }
}