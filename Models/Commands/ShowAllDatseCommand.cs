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
    public class ShowAllDatseCommand : Command
    {
        public override string Name => @"/showwithdates";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            if(Variables.StateList[message.Chat.Id] == Variables.State.None)
            {
                if(Variables.InputDataList[message.Chat.Id].Authorization)  
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: "Введите две даты в формате(ММ.ДД.ГГГГ-ММ.ДД.ГГГГ)", 
                        parseMode: ParseMode.Markdown
                    );

                    Variables.StateList[message.Chat.Id] = Variables.State.GetDatePeriods;
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
            else
            {
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

            InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[dt.Rows.Count+2][];

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
                InlineKeyboardButton.WithCallbackData("Выход в главное меню","Cancel"),
            };

            return keyboard;
        }
    }
}