namespace InfoSearcher
{
    public class GrabberConfig
    {
        public string Title { get; set; }

        public string MainLink { get; set; }
        
        public string OutName { get; set; }

        public string NewsListSelector { get; set; }

        public string NewsContentSelector { get; set; }

        public string[] SearchFor { get; set; }
    }
}