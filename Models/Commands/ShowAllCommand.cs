using System.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace KursachBotRegata.Models.Commands
{
    public class ShowAllCommand : Command
    {
        public override string Name =>@"/showall";

        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            if(Variables.StateList[message.Chat.Id] == Variables.State.None)
            {
                if(Variables.InputDataList[message.Chat.Id].Authorization)  
                {
                    DataTable dtPoints = new DataTable();
			        dtPoints = DBWorker.SelectCommand("fio, points", "listrecords", $"WHERE listrecords.group = '{Variables.InputDataList[message.Chat.Id].Group}'");

                    DataTable dtUsers = new DataTable();
			        dtUsers = DBWorker.SelectCommand("fio", "users", $"WHERE users.group = '{Variables.InputDataList[message.Chat.Id].Group}' AND post = '3'");

                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: EditTextMessage( dtPoints, dtUsers), 
                        parseMode: ParseMode.Markdown
                    );
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: "Для этой команды нужно авторизаваться в боте \nДля этого нужно прописать команды /start", 
                        parseMode: ParseMode.Markdown
                    );
                }
            }
            else{
                var keyboard = new InlineKeyboardMarkup
                (
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Выход в главное меню","Cancel"),
                    }
                );
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id, 
                    text: "Вы находитесь не в главном меню", 
                    parseMode: ParseMode.Markdown,
                    replyMarkup: keyboard
                );
            }
        }

        private string EditTextMessage(DataTable dtPoints, DataTable dtUsers)
        {
            string OutPut = "";

            if (dtPoints.Rows.Count <= 0)
               return "Данных нет";

            if (dtUsers.Rows.Count <= 0)
               return "Данных нет";

            List<UsersListWithPoints> UsersList = new List<UsersListWithPoints>();
            foreach (DataRow row in dtUsers.Rows)
            {
                UsersListWithPoints User = new UsersListWithPoints();
                User.Name = row["fio"].ToString();
                UsersList.Add(User);
            }
            UsersList.Add(new UsersListWithPoints(){Name = "Весь класс"});
            UsersList.Add(new UsersListWithPoints(){Name = "Общий счет"});

            foreach (DataRow row in dtPoints.Rows)
            {
                for (int i = 0; i < UsersList.Count; i++)
                {
                    if(row["fio"].ToString() == UsersList[i].Name)
                    {
                        UsersList[i].Point += float.Parse(row["points"].ToString());
                        UsersList[UsersList.Count-1].Point += float.Parse(row["points"].ToString()); 
                    }
                }
            }

            

            foreach (var item in UsersList)
            {
                OutPut += $"{item.Name} \t = \t {item.Point.ToString()}\n";
            }

            return OutPut;
        }
    }

    public class UsersListWithPoints
    {
        public string Name;
        public float Point;
    }
}