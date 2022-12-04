using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace KursachBotRegata.Models.Commands
{
    public class SwohInfoUsers : Command
    {
        public override string Name =>@"/swohinfousers";

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
                    
                    DataTable dtPoints = new DataTable();
			        dtPoints = DBWorker.SelectCommand("id, fio, login,password,post,users.group", "users", "");

					if (dtPoints.Rows.Count <= 0)
					{
						await botClient.SendTextMessageAsync(
							chatId: message.Chat.Id,
							text: "Данных нет",
							parseMode: ParseMode.Markdown
						);
					
						Variables.StateList[message.Chat.Id] = Variables.State.None;
						return;
					}
					string OutText = "";
					foreach (DataRow row in dtPoints.Rows)
					{
						OutText +=$"(id = {row["id"].ToString()}) {row["fio"].ToString()} - {row["login"].ToString()} - {row["password"].ToString()} - {row["post"].ToString()} - {row["group"].ToString()}\n";
					}

					System.IO.File.WriteAllText(Environment.CurrentDirectory + $@"\file{message.Chat.Id}.txt", OutText);
					using (var stream = System.IO.File.OpenRead(Environment.CurrentDirectory + $@"\file{message.Chat.Id}.txt"))
					{
						InputOnlineFile iof = new InputOnlineFile(stream);
						iof.FileName = $"InfoUsers.txt";
						await botClient.SendDocumentAsync(message.Chat.Id, iof);
					}          
					
					
					System.IO.File.Delete(Environment.CurrentDirectory + $@"\file{message.Chat.Id}.txt");
					Variables.StateList[message.Chat.Id] = Variables.State.None;
					return;
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