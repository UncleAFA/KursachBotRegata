using System.Collections.Generic;

namespace KursachBotRegata.Models
{
    public static class AppSettings
    {
        public static string Url { get; set; } = "https://42fb-90-154-71-80.eu.ngrok.io" + "/{0}";
        public static string Name { get; set; } = "RegtaBotHellper";
        public static string Key { get; set; } = "5710419666:AAFwvJNvTu5qpJ4_1C2_6MZxHBbO4tZPRsE";

        // public static Dictionary<long, State> StateList = new Dictionary<long, State>();
        // public static Dictionary<long, InserLine> InsertList = new Dictionary<long, InserLine>();
        // public enum State
        // {
        //     None,
        //     Fio,
        //     Points,
        //     Details,
        //     Confirm,
        //     ShowNamePoint,
        //     GetDates
        // }
    }
}