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

				case Variables.State.GetPontInsert:
					if (float.TryParse(message.Text.ToString(), out float res) )
					{
						await botClient.SendTextMessageAsync(
							chatId: message.Chat.Id,
							text: $"\tФИО: {Variables.InputDataList[message.Chat.Id].InsertFio} \nБаллы: {float.Parse(message.Text.ToString()).ToString()} \n\nВведите детали:",
							parseMode: ParseMode.Markdown
						);
						Variables.InputDataList[message.Chat.Id].InsertPoint = float.Parse(message.Text.ToString()).ToString();
						Variables.StateList[message.Chat.Id] = Variables.State.GetDetailInsert;
					}
					else
					{
						await botClient.SendTextMessageAsync(
							chatId: message.Chat.Id,
							text: "Некорректный формат \nПовторите ввод количества баллов учитывая формат(формат: -5,0 | + 5,9)",
							parseMode: ParseMode.Markdown
						);
					}
					break;

				case Variables.State.GetDetailInsert:
					var keyboard = new InlineKeyboardMarkup
					(
						new[]
						{
							InlineKeyboardButton.WithCallbackData("Да","YesForNewInsert"),
							InlineKeyboardButton.WithCallbackData("Нет","NoForNewInsert"),
						}
					);
					Variables.InputDataList[message.Chat.Id].InsertDetail = message.Text.ToString();
					await botClient.SendTextMessageAsync(
						chatId: message.Chat.Id,
						text: $"\tФИО: {Variables.InputDataList[message.Chat.Id].InsertFio} \nБаллы: {Variables.InputDataList[message.Chat.Id].InsertPoint} \nДетали: {message.Text.ToString()} \n\nВсе введено правильно?",
						parseMode: ParseMode.Markdown,
						replyMarkup: keyboard
					);
					Variables.InputDataList[message.Chat.Id].InsertDetail = message.Text.ToString();
					Variables.StateList[message.Chat.Id] = Variables.State.GetSheckInfo;
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
			Variables.InputDataList[message.Chat.Id].Login = message.Text.ToString();
			
		}

		private async Task GetPassword(Message message, TelegramBotClient botClient)
		{
			bool Flag = false;
			DataTable dt = new DataTable();
			dt = DBWorker.SelectCommand("password", "users", $"WHERE login = '{Variables.InputDataList[message.Chat.Id].Login.ToString()}'");
			foreach (DataRow row in dt.Rows)
			{
				if (row[0].ToString() == message.Text.ToString())
				{
					Flag = true;
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

			Variables.StateList[message.Chat.Id] = Variables.State.None;
			Variables.InputDataList[message.Chat.Id].Authorization = true;

			dt = DBWorker.SelectCommand("fio, post, users.group", "users", $"WHERE password = '{message.Text.ToString()}' AND login = '{Variables.InputDataList[message.Chat.Id].Login.ToString()}'");

			foreach (DataRow row in dt.Rows)
			{
				Variables.InputDataList[message.Chat.Id].FioUser = row[0].ToString();
				Variables.InputDataList[message.Chat.Id].Post = row[1].ToString();
				Variables.InputDataList[message.Chat.Id].Group = row[2].ToString();
			}
			
			await botClient.SendTextMessageAsync(
				chatId: message.Chat.Id,
				text: $"Здравствуйте {Variables.InputDataList[message.Chat.Id].FioUser} \n\n\n\nВы в главном меню \nСписок доступных команд:",//TODO: добавить список доступных команд И информция о USERE
				parseMode: ParseMode.Markdown
			);
		}
	}
}