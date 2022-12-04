using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace KursachBotRegata.Models.Commands
{
    public class ClearListRecords : Command
    {
        public override string Name =>@"/clearlistrecords";

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
                    if (Variables.InputDataList[message.Chat.Id].Post != "1")
                    {
                        return;
                    }
                    
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: "Вы очистили всю базу данных с записями", 
                        parseMode: ParseMode.Markdown
                    );
                    DBWorker.DeleteCommand($"listrecords",$"id <> 0");

                    Variables.StateList[message.Chat.Id] = Variables.State.None;
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
    }
}