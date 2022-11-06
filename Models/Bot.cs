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

        public static async Task<TelegramBotClient> GetBotClientAsync(string Token, string Url)
        {
            System.Console.WriteLine(Token);
            System.Console.WriteLine(Url);
            if (botClient != null)
            {
                return botClient;
            }

            commandsList = new List<Command>();
            // commandsList.Add(new StartCommand());
            // commandsList.Add(new HelpCommand());
            // commandsList.Add(new AddCommand());
            // commandsList.Add(new ShowAllCommand());
            // commandsList.Add(new ShowOnePersoneCommand());
            // commandsList.Add(new ShowAllDatseCommand());

            //TODO: Add more commands

            botClient = new TelegramBotClient(Token);
            string hook = Url + "/api/message/update";
            System.Console.WriteLine(hook);
            await botClient.SetWebhookAsync(hook);
            return botClient;
        }
    }
}