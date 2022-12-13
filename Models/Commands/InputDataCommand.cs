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
				
				case Variables.State.GetIdForDelete:
					DataTable dt = new DataTable();
					dt = DBWorker.SelectCommand("id", "listrecords", $"WHERE listrecords.group = '{Variables.InputDataList[message.Chat.Id].Group}'");
					if(dt.Rows.Count<=0)
					{
						await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: $"Записей еще нет", 
                        parseMode: ParseMode.Markdown
                    	);
						
						Variables.StateList[message.Chat.Id] = Variables.State.None;
						break;
					}
					if (!int.TryParse(message.Text.ToString(),out int inres))
					{
						await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: $"Повторите число повторно (Введите число)", 
                        parseMode: ParseMode.Markdown
                    	);
						break;
					}
					if (int.Parse(message.Text.ToString()) <= 0)
					{
						await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: $"Повторите число повторно (Оно должно быть больше 0)", 
                        parseMode: ParseMode.Markdown
                    	);
						break;
					}
					bool FlagDelete = false;
					foreach (DataRow row in dt.Rows)
					{
						if (row[0].ToString() == message.Text.ToString())
						{
							DataTable dt1 = new DataTable();
							dt1 = DBWorker.SelectCommand("fio,listrecords.group,points,details", "listrecords", $"WHERE id = {int.Parse(message.Text.ToString())}");
							foreach (DataRow row1 in dt1.Rows)
							{
								await botClient.SendTextMessageAsync(
									chatId: message.Chat.Id, 
									text: $"Удаленная запись:\n {row1[0].ToString()}\n{row1[1].ToString()}\n{row1[2].ToString()}\n{row1[3].ToString()}\n", 
									parseMode: ParseMode.Markdown
								);
								DBWorker.DeleteCommand($"listrecords",$"id = {int.Parse(message.Text.ToString())}");
								FlagDelete = true;
							}
						}
					}
					if (!FlagDelete)
					{
						await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: $"Такой записи нет", 
                        parseMode: ParseMode.Markdown
                    	);
					}
					Variables.StateList[message.Chat.Id] = Variables.State.None;
					break;

				case Variables.State.GetIdUserDelete:
					DataTable dtUsersDel = new DataTable();
					dtUsersDel = DBWorker.SelectCommand("id", "users","");
					if(dtUsersDel.Rows.Count<=0)
					{
						await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: $"Пользователей нет", 
                        parseMode: ParseMode.Markdown
                    	);
						
						Variables.StateList[message.Chat.Id] = Variables.State.None;
						break;
					}
					if (!int.TryParse(message.Text.ToString(),out int inress))
					{
						await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: $"Повторите число повторно (Введите число)", 
                        parseMode: ParseMode.Markdown
                    	);
						break;
					}
					if (int.Parse(message.Text.ToString()) <= 0)
					{
						await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: $"Повторите число повторно (Оно должно быть больше 0)", 
                        parseMode: ParseMode.Markdown
                    	);
						break;
					}
					bool FlagDeleteUs = false;
					foreach (DataRow row in dtUsersDel.Rows)
					{
						if (row[0].ToString() == message.Text.ToString())
						{
							DataTable dt1 = new DataTable();
							dt1 = DBWorker.SelectCommand("fio,login,password,post,users.group", "users", $"WHERE id = {int.Parse(message.Text.ToString())}");
							foreach (DataRow row1 in dt1.Rows)
							{
								await botClient.SendTextMessageAsync(
									chatId: message.Chat.Id, 
									text: $"Удаленная запись:\n {row1[0].ToString()}\n{row1[1].ToString()}\n{row1[2].ToString()}\n{row1[3].ToString()}\n{row1[4].ToString()}\n", 
									parseMode: ParseMode.Markdown
								);
								DBWorker.DeleteCommand($"users",$"id = {int.Parse(message.Text.ToString())}");
								FlagDeleteUs = true;
							}
						}
					}
					if (!FlagDeleteUs)
					{
						await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id, 
                        text: $"Такой записи нет", 
                        parseMode: ParseMode.Markdown
                    	);
					}
					Variables.StateList[message.Chat.Id] = Variables.State.None;
					break;

				case Variables.State.GetFioNewUser:
					Variables.InputDataList[message.Chat.Id].InsertNewFio = message.Text.ToString();
					await botClient.SendTextMessageAsync(
						chatId: message.Chat.Id, 
						text: $"Новый пользователь \nФИО:{Variables.InputDataList[message.Chat.Id].InsertNewFio}\n\n\nВедите Login для нового пользователя(null - если логин не нужен)", 
						parseMode: ParseMode.Markdown
					);
					Variables.StateList[message.Chat.Id] = Variables.State.GetLoginNewUser;
					break;

				case Variables.State.GetLoginNewUser:
					if(message.Text.ToString() == "null")
					{
						Variables.InputDataList[message.Chat.Id].InsertNewLogin = null;
					}
					if(message.Text.ToString() != "null")
					{
						Variables.InputDataList[message.Chat.Id].InsertNewLogin = message.Text.ToString();
					}

					await botClient.SendTextMessageAsync(
						chatId: message.Chat.Id, 
						text: $"Новый пользователь \nФИО:{Variables.InputDataList[message.Chat.Id].InsertNewFio}\nLogin:{Variables.InputDataList[message.Chat.Id].InsertNewLogin}\n\n\nВедите Password для нового пользователя(null - если пароль не нужен)", 
						parseMode: ParseMode.Markdown
					);
					Variables.StateList[message.Chat.Id] = Variables.State.GetPassNewUser;
					break;

				case Variables.State.GetPassNewUser:
					if(message.Text.ToString() == "null")
					{
						Variables.InputDataList[message.Chat.Id].InsertNewPassword = null;
					}
					if(message.Text.ToString() != "null")
					{
						Variables.InputDataList[message.Chat.Id].InsertNewPassword = message.Text.ToString();
					}

					await botClient.SendTextMessageAsync(
						chatId: message.Chat.Id, 
						text: $"Новый пользователь \nФИО:{Variables.InputDataList[message.Chat.Id].InsertNewFio}\nLogin:{Variables.InputDataList[message.Chat.Id].InsertNewLogin}\nPassword:{Variables.InputDataList[message.Chat.Id].InsertNewPassword}\n\n\nВедите роль для нового пользователя(1 - администратор, 2-преподаватель,3-ученик)", 
						parseMode: ParseMode.Markdown
					);
					Variables.StateList[message.Chat.Id] = Variables.State.GetPostNewUser;
					break;

				case Variables.State.GetPostNewUser:
					if(message.Text.ToString() == "1")
					{
						Variables.InputDataList[message.Chat.Id].InsertNewPost = message.Text.ToString();
					}
					else if(message.Text.ToString() == "2")
					{
						Variables.InputDataList[message.Chat.Id].InsertNewPost = message.Text.ToString();
					}
					else if(message.Text.ToString() == "3")
					{
						Variables.InputDataList[message.Chat.Id].InsertNewPost = message.Text.ToString();
					}
					else
					{
						await botClient.SendTextMessageAsync(
						chatId: message.Chat.Id, 
						text: $"Ведите роль для нового пользователя в соответствии с примером (1 - администратор, 2-преподаватель,3-ученик)", 
						parseMode: ParseMode.Markdown
						);
						break;
					}

					await botClient.SendTextMessageAsync(
						chatId: message.Chat.Id, 
						text: $"Новый пользователь \nФИО:{Variables.InputDataList[message.Chat.Id].InsertNewFio}\nLogin:{Variables.InputDataList[message.Chat.Id].InsertNewLogin}\nPassword:{Variables.InputDataList[message.Chat.Id].InsertNewPassword}\nРоль:{Variables.InputDataList[message.Chat.Id].InsertNewPost}\n\n\nВвеите класс к которому он относится(null - если класс не нужен)", 
						parseMode: ParseMode.Markdown
					);
					Variables.StateList[message.Chat.Id] = Variables.State.GetGroupNewUser;
					break;

				case Variables.State.GetGroupNewUser:
					if(message.Text.ToString() == "null")
					{
						Variables.InputDataList[message.Chat.Id].InsertNewGroup = null;
					}
					if(message.Text.ToString() != "null")
					{
						Variables.InputDataList[message.Chat.Id].InsertNewGroup = message.Text.ToString();
					}

					await botClient.SendTextMessageAsync(
						chatId: message.Chat.Id, 
						text: $"Новый пользователь \nФИО:{Variables.InputDataList[message.Chat.Id].InsertNewFio}\nLogin:{Variables.InputDataList[message.Chat.Id].InsertNewLogin}\nPassword:{Variables.InputDataList[message.Chat.Id].InsertNewPassword}\nРоль:{Variables.InputDataList[message.Chat.Id].InsertNewPost}\nКласс:{Variables.InputDataList[message.Chat.Id].InsertNewGroup}", 
						parseMode: ParseMode.Markdown
					);
					Variables.StateList[message.Chat.Id] = Variables.State.None;
					DBWorker.InsertCommand("(fio,login,password,post,\"group\")","users",$@"'{Variables.InputDataList[message.Chat.Id].InsertNewFio}','{Variables.InputDataList[message.Chat.Id].InsertNewLogin}','{Variables.InputDataList[message.Chat.Id].InsertNewPassword}','{Variables.InputDataList[message.Chat.Id].InsertNewPost}','{Variables.InputDataList[message.Chat.Id].InsertNewGroup}'");
					await botClient.SendTextMessageAsync(
						chatId: message.Chat.Id, 
						text: $"Пользователь создан", 
						parseMode: ParseMode.Markdown
					);
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
				text: $"Здравствуйте {Variables.InputDataList[message.Chat.Id].FioUser} \n\n\n\nВы находитесь в главном меню",
				parseMode: ParseMode.Markdown
			);
			string ListComandForUsers=  "Доступные вам команды:\n"+
                                            "/start - Авторизация\n" +
                                            "/help - список системных команд\n" +
                                            "/showall - баллы всего коллектива\n"+
                                            "/add - добавление новой заметки\n"+
                                            "/showwithdates - показать баллы за промежуток времени\n"+
                                            "/showonepersone - показать баллы одного человека из коллектива\n"+
                                            "/deleteline - удалить заметку\n";

            string ListComandForAdmin=  "Доступные вам команды:\n"+
                                            "/start - Авторизация\n" +
                                            "/help - список системных команд\n" +
                                            "/clearlistrecords -очистка таблицы с заметками\n"+
                                            "/deleteuser - удаление пользователя\n"+
                                            "/swohinfousers - получение информации о пользователях\n"+
                                            "/createnewuser - добавление нового пользователя";

			switch (Variables.InputDataList[message.Chat.Id].Post)
                {                        
                    case "1":
                        await botClient.SendTextMessageAsync(
                            chatId:message.Chat.Id,
                            text: ListComandForAdmin, 
                            parseMode: ParseMode.Markdown
                        );
                        break;
                    case "2":
                        await botClient.SendTextMessageAsync(
                            chatId:message.Chat.Id,
                            text: ListComandForUsers, 
                            parseMode: ParseMode.Markdown
                        );
                        break;
                }
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