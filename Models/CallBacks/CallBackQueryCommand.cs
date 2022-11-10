using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KursachBotRegata.Models.CallBacks
{
    public class CallBackQueryCommand
    {
        public async Task Execute(CallbackQuery message, TelegramBotClient botClient)
        {
            if (message.Data.ToString() == "Cancel")
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Message.Chat.Id, 
                    text: "Выход в главное меню", 
                    parseMode: ParseMode.Markdown
                );

                Variables.StateList[message.Message.Chat.Id] = Variables.State.None;
            }
        }
    }
}