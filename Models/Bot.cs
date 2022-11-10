using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using KursachBotRegata.Models.Commands;
using Telegram.Bot.Types.ReplyMarkups;

namespace KursachBotRegata.Models
{
    public class Bot
    {
        private static TelegramBotClient botClient;
        private static List<Command> commandsList;

        public static IReadOnlyList<Command> Commands => commandsList.AsReadOnly();

        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (botClient != null)
            {
                return botClient;
            }

            commandsList = new List<Command>();
            commandsList.Add(new StartCommand());
            commandsList.Add(new HelpCommand());

            commandsList.Add(new InputDataCommand());
            //TODO: Add more commands

            // botClient = new TelegramBotClient(Token);
            // string hook = Url + "/api/message/update";
            
            botClient = new TelegramBotClient(AppSettings.Key);
            string hook = string.Format(AppSettings.Url, "api/message/update");
            Console.WriteLine("LOG: WEBHook  - " + hook);
            await botClient.SetWebhookAsync(hook);

            return botClient;
        }
    }
}