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

				case Variables.State.GetDatePeriods:
					string[] DatesArr= message.Text.ToString().Split('-');
					
					if(DatesArr.Count() != 2 | !DateTime.TryParse(DatesArr[0],out DateTime start) | !DateTime.TryParse(DatesArr[1],out DateTime end))
					{
						await botClient.SendTextMessageAsync(
							chatId: message.Chat.Id,
							text: "Введите повторно две даты обращая внимание на формат(ДД.ММ.ГГГГ-ДД.ММ.ГГГГ)",
							parseMode: ParseMode.Markdown
						);
						break;
					}
					if(start > end)
					{
						await botClient.SendTextMessageAsync(
							chatId: message.Chat.Id,
							text: "Введите повторно две даты обращая внимание на формат(ДД.ММ.ГГГГ-ДД.ММ.ГГГГ) \n Первая дата должна быть меньше второй",
							parseMode: ParseMode.Markdown
						);
						break;
					}

					DataTable dtPoints = new DataTable();
			        dtPoints = DBWorker.SelectCommand("fio, points", "listrecords", $"WHERE listrecords.group = '{Variables.InputDataList[message.Chat.Id].Group}' AND listrecords.date BETWEEN '{start.Year}-{start.Month}-{start.Day}' AND '{end.Year}-{end.Month}-{end.Day}'");

                    DataTable dtUsers = new DataTable();
			        dtUsers = DBWorker.SelectCommand("fio", "users", $"WHERE users.group = '{Variables.InputDataList[message.Chat.Id].Group}' AND post = '3'");

                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: $"Данные на период {start.ToShortDateString()} - {end.ToShortDateString()}\n" + EditTextMessage( dtPoints, dtUsers), 
                        parseMode: ParseMode.Markdown
                    );
					Variables.StateList[message.Chat.Id] = Variables.State.None;
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

		private string EditTextMessage(DataTable dtPoints, DataTable dtUsers)
        {
            if (dtPoints.Rows.Count <= 0)
               return "Данных нет";

            if (dtUsers.Rows.Count <= 0)
               return "Данных нет";

            List<UsersListWithPoints> UsersList = new List<UsersListWithPoints>();
            foreach (DataRow row in dtUsers.Rows)
            {
                UsersListWithPoints User = new UsersListWithPoints();
                User.Name = row["fio"].ToString();
                UsersList.Add(User);
            }
            UsersList.Add(new UsersListWithPoints(){Name = "Весь класс"});
            UsersList.Add(new UsersListWithPoints(){Name = "Общий счет"});

            foreach (DataRow row in dtPoints.Rows)
            {
                for (int i = 0; i < UsersList.Count; i++)
                {
                    if(row["fio"].ToString() == UsersList[i].Name)
                    {
                        System.Console.WriteLine(float.Parse(row["points"].ToString()));
                        UsersList[i].Point += float.Parse(row["points"].ToString());
                        UsersList[UsersList.Count-1].Point += float.Parse(row["points"].ToString()); 
                    }
                }
            }

            string OutPut = "";

            foreach (var item in UsersList)
            {
                OutPut += $"{item.Name} \t = \t {item.Point.ToString()}\n";
            }

            return OutPut;
        }
	}
}