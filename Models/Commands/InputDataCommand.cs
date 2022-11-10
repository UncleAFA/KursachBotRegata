using System.Data;
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
    public class InputDataCommand : Command
    {
        public override string Name => string.Empty;

        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            switch (Variables.StateList[key: message.Chat.Id])
            {
                case Variables.State.None:
                    break;

                case Variables.State.GetLogin:
                    await GetLogin(message, botClient);
                    break;

                case Variables.State.GetPassword:
                    await GetPassword(message, botClient);
                    break;
            }
        }

        private async Task GetLogin(Message message, TelegramBotClient botClient)
        {
            bool Flag = false;
            DataTable dt = new DataTable();
            dt = DBWorker.SelectCommand("login", "users", $"WHERE login = '{message.Text.ToString()}'");
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() == message.Text.ToString())
                    Flag = true;
            }

            if(!Flag)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Введенный логин не существует или некорректен попробуйте снова",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Логин введен корректно \nВвеите пароль:",
                parseMode: ParseMode.Markdown
            );
            Variables.StateList[message.Chat.Id] = Variables.State.GetPassword;
        }

        private async Task GetPassword(Message message, TelegramBotClient botClient)
        {
            bool Flag = false;
            string NameUser = "";
            DataTable dt = new DataTable();
            dt = DBWorker.SelectCommand("password, fio", "users", $"WHERE password = '{message.Text.ToString()}'");
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() == message.Text.ToString())
                {
                    Flag = true;
                    NameUser = row[1].ToString();
                }
            }

            if(!Flag)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Введенный пароль некорректен попробуйте снова",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Пароль введен корректно",
                parseMode: ParseMode.Markdown
            );

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Здравствуйте {NameUser} \n\n\n\nВы в главном меню \nСписок доступных команд:",//TODO: добавить список доступных команд И информция о USERE
                parseMode: ParseMode.Markdown
            );

            Variables.StateList[message.Chat.Id] = Variables.State.None;
        }
    }
}