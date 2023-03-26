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
    public class ShowOnePersoneCommand : Command
    {
        public override string Name =>@"/showonepersone";
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
                    var keyboard = new InlineKeyboardMarkup
                    (
                        GetListFio(Variables.InputDataList[message.Chat.Id].Group)
                    );

                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: "Выберите ученика", 
                        parseMode: ParseMode.Markdown,
                        replyMarkup: keyboard
                    );

                    Variables.StateList[message.Chat.Id] = Variables.State.GetPointsOnePersone;
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
        private InlineKeyboardButton[][] GetListFio(string Group)
        {
            DataTable dt = new DataTable();
            dt = DBWorker.SelectCommand("fio", "users", $"WHERE post = '3' AND users.group = '{Group}'");

            InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[dt.Rows.Count+3][];

            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                keyboard[i] = new[]
                {
                    InlineKeyboardButton.WithCallbackData(row[0].ToString(),row[0].ToString()),
                };
                i++;
            }

            keyboard[i] = new[]
            {
                InlineKeyboardButton.WithCallbackData("Весь класс","Весь класс"),
            };
            i++;
			keyboard[i] = new[]
		    {
				InlineKeyboardButton.WithCallbackData("Все записи","Все записи"),
			};
			i++;
			keyboard[i] = new[]
            {
                InlineKeyboardButton.WithCallbackData("Выход в главное меню","Cancel"),
            };

            return keyboard;
        }
    }
}