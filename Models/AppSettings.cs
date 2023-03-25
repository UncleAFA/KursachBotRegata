namespace KursachBotRegata.Models
{
    public static class AppSettings
    {
        public static string Url { get; set; } = "https://87a3-90-154-71-80.eu.ngrok.io" + "/{0}";
        public static string Key { get; set; } = "5710419666:AAFwvJNvTu5qpJ4_1C2_6MZxHBbO4tZPRsE";
        public static string ConnString { get; set; } = "Server=host.docker.internal;Username=postgres;Database=postgres;Port=32768;Password=postgrespw;SSLMode=Prefer";
        
    }
}
