using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

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

			switch (Variables.StateList[message.Message.Chat.Id])
			{
				case Variables.State.GetFioInsert:
					await botClient.SendTextMessageAsync(
						chatId: message.Message.Chat.Id,
						text: $"\tВы выбрали: {message.Data.ToString()} \n\nВведите количество баллов (формат: -5,0 | + 5,9)",
						parseMode: ParseMode.Markdown
					);
					Variables.InputDataList[message.Message.Chat.Id].InsertFio = message.Data.ToString();
					Variables.StateList[message.Message.Chat.Id] = Variables.State.GetPontInsert;
					break;

				case Variables.State.GetSheckInfo:
					if (message.Data.ToString() == "NoForNewInsert")
					{
						await botClient.SendTextMessageAsync(
							chatId: message.Message.Chat.Id,
							text: "Выход в главное меню",
							parseMode: ParseMode.Markdown
						);

						Variables.StateList[message.Message.Chat.Id] = Variables.State.None;
					}

					if (message.Data.ToString() == "YesForNewInsert")
					{
						DBWorker.InsertCommand("","listrecords",$@"'{Variables.InputDataList[message.Message.Chat.Id].InsertFio}','{Variables.InputDataList[message.Message.Chat.Id].Group}','{Variables.InputDataList[message.Message.Chat.Id].InsertPoint}','{Variables.InputDataList[message.Message.Chat.Id].InsertDetail}','{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}'");
						await botClient.SendTextMessageAsync(
							chatId: message.Message.Chat.Id,
							text: "Данные занесены",
							parseMode: ParseMode.Markdown
						);

						await botClient.SendTextMessageAsync(
							chatId: message.Message.Chat.Id,
							text: "Выход в главное меню",
							parseMode: ParseMode.Markdown
						);

						Variables.StateList[message.Message.Chat.Id] = Variables.State.None;
					}
					break;

				case Variables.State.GetPointsOnePersone:					
					DataTable dtPoints = new DataTable();
			        dtPoints = DBWorker.SelectCommand("id, fio, points,date,details", "listrecords", $"WHERE listrecords.group = '{Variables.InputDataList[message.Message.Chat.Id].Group}' AND fio = '{message.Data.ToString()}'");

					if (dtPoints.Rows.Count <= 0)
					{
						await botClient.SendTextMessageAsync(
							chatId: message.Message.Chat.Id,
							text: "Данных нет",
							parseMode: ParseMode.Markdown
						);
					
						Variables.StateList[message.Message.Chat.Id] = Variables.State.None;
						break;
					}
					string OutText = "";
					foreach (DataRow row in dtPoints.Rows)
					{
								// UsersList[i].Point += float.Parse(row["points"].ToString());
								// UsersList[UsersList.Count-1].Point += float.Parse(row["points"].ToString());
						OutText +=$"(id = {row["id"].ToString()}) {row["date"].ToString()} - {row["points"].ToString()} - {row["details"].ToString()} \n";
					}

					System.IO.File.WriteAllText(Environment.CurrentDirectory + $@"\file{message.Message.Chat.Id}.txt", OutText);
					using (var stream = System.IO.File.OpenRead(Environment.CurrentDirectory + $@"\file{message.Message.Chat.Id}.txt"))
					{
						InputOnlineFile iof = new InputOnlineFile(stream);
						iof.FileName = $"{message.Data}{DateTime.Now.ToShortDateString()}.txt";
						await botClient.SendDocumentAsync(message.Message.Chat.Id, iof);
					}          
					
					
					System.IO.File.Delete(Environment.CurrentDirectory + $@"\file{message.Message.Chat.Id}.txt");
					Variables.StateList[message.Message.Chat.Id] = Variables.State.None;
					break;
			}
		}
	}
}