using System;

namespace KursachBotRegata.Models
{
    public class InputInfo
    {
        public string FioUser { get; set; }

        public string Login { get; set; }

        public bool Authorization { get; set; }

        public string Post { get; set; }

        public long ChatId { get; set; }

        public string Group { get; set; }

        public string InsertFio { get; set; }
        public string InsertPoint { get; set; }
        public string InsertDetail { get; set; }
        public string InsertNewFio { get; set; }
        public string InsertNewLogin { get; set; }
        public string InsertNewPassword { get; set; }
        public string InsertNewPost { get; set; }
        public string InsertNewGroup { get; set; }
    }
}
