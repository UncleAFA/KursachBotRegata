using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.IO;
using System;
using KursachBotRegata.Models;

namespace KursachBotRegata.Models.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";

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
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id, 
                    text: "Здравствуйте", 
                    parseMode: ParseMode.Markdown
                );
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id, 
                    text: "Введите ваш логин:", 
                    parseMode: ParseMode.Markdown
                );
                Variables.StateList[message.Chat.Id] = Variables.State.GetLogin;
                Variables.InputDataList[message.Chat.Id] = new InputInfo();
                // readfile();
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

        public static void readfile()
        {
            string fileName = @"C:\GitHubProjects\KursachBotRegata\Все данные для БД RegataBot.txt";

            List<string> lines = System.IO.File.ReadLines(fileName).ToList();
            // Console.WriteLine(String.Join(Environment.NewLine, lines));
            foreach (var item1 in lines)
            {
                
                string[] stringssss = item1.ToString().Split('|');
                // if(stringssss.Length!= 4)
                //     System.Console.WriteLine(item1);
                string [] datatostr = stringssss[3].ToString().Split('/');
                // System.Console.WriteLine(stringssss.Length);
                // if(datatostr.Length!= 3)
                //     System.Console.WriteLine(stringssss[3]);

                // foreach (var item in datatostr)
                // {
                //     System.Console.WriteLine(item);
                // }
                DateTime test = new DateTime(int.Parse(datatostr[2].ToString()),int.Parse(datatostr[0].ToString()),int.Parse(datatostr[1].ToString()));
                // System.Console.WriteLine(test);
                DBWorker.InsertCommand("","listrecords",$@"'{stringssss[0].ToString().Trim(' ')}','3','{stringssss[1].ToString().Trim(' ')}','{stringssss[2].ToString().Trim(' ')}','{test.Year}-{test.Month}-{test.Day}'");

                    
            
            }
            System.Console.WriteLine("Всеееееееееееееееее");
            // DBWorker.InsertCommand("","listrecords",$@"'{Variables.InputDataList[message.Message.Chat.Id].InsertFio}',+
            //                                            '{Variables.InputDataList[message.Message.Chat.Id].Group}',
            //                                            '{Variables.InputDataList[message.Message.Chat.Id].InsertPoint}',
            //                                            '{Variables.InputDataList[message.Message.Chat.Id].InsertDetail}',
            //                                            '{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}'");
            
        }
    }
}