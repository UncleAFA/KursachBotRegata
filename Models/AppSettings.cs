using System.Collections.Generic;

namespace KursachBotRegata.Models
{
    public static class AppSettings
    {
        public static string Url { get; set; } = "https://31aa-2a02-2168-b1f6-f300-151b-7461-124d-c77f.eu.ngrok.io" + "/{0}";
        public static string Key { get; set; } = "5710419666:AAFwvJNvTu5qpJ4_1C2_6MZxHBbO4tZPRsE";
        public static string ConnString { get; set; } = "Server=host.docker.internal;Username=postgres;Database=postgres;Port=49153;Password=postgrespw;SSLMode=Prefer";
        
    }
}