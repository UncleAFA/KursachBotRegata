﻿using System;
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

            //User command
            commandsList.Add(new ShowAllCommand());
            commandsList.Add(new AddCommand());
            commandsList.Add(new ShowAllDatseCommand());
            commandsList.Add(new ShowOnePersoneCommand());
            commandsList.Add(new DeletOneLine());
			commandsList.Add(new AutoInputCommand());

            //AdminCommand
			commandsList.Add(new ClearListRecords());
            commandsList.Add(new DeletUser());
            commandsList.Add(new SwohInfoUsers());
            commandsList.Add(new AddNewUser());

			commandsList.Add(new InputDataCommand());
            //TODO: Add more commands
            
            botClient = new TelegramBotClient(AppSettings.Key);
            string hook = string.Format(AppSettings.Url, "api/message/update");
            Console.WriteLine("LOG: WEBHook  - " + hook);
            await botClient.SetWebhookAsync(hook);

            return botClient;
        }
    }
}