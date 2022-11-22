using System.Collections.Generic;

namespace KursachBotRegata.Models
{
    public static class Variables
    {
        public static Dictionary<long, State> StateList = new Dictionary<long, State>();
        public static Dictionary<long, InputInfo> InputDataList = new Dictionary<long, InputInfo>();
        public enum State
        {
            None,
            GetLogin,
            GetPassword,
            GetFioInsert,
            GetPontInsert,
            GetDetailInsert,
            GetSheckInfo
        }
    }
}