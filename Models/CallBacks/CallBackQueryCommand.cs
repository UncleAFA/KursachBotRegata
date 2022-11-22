using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
				
			}
		}
	}
}