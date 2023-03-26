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
	public class AutoInputCommand : Command
	{
		public override string Name => @"/autoinput";

		public override bool Contains(Message message)
		{
			if (message.Type != MessageType.Text)
				return false;

			return message.Text.Contains(this.Name);
		}

		public override async Task Execute(Message message, TelegramBotClient botClient)
		{
			if (Variables.StateList[message.Chat.Id] == Variables.State.None)
			{
				if (Variables.InputDataList[message.Chat.Id].Authorization)
				{
					var keyboard = new InlineKeyboardMarkup
					(
						new[]
						{
							InlineKeyboardButton.WithCallbackData("Выход в главное меню","Cancel"),
						}
					);
					await botClient.SendTextMessageAsync(
						chatId: message.Chat.Id,
						text: "Введите ваши данные для автоматического ввода\n Формат Федя| +1|test| 03/15/2023(ММ/ДД/ГГГГ)",
						parseMode: ParseMode.Markdown,
						replyMarkup: keyboard
					);

					Variables.StateList[message.Chat.Id] = Variables.State.GetAutoInputData;
				}
				else
				{
					await botClient.SendTextMessageAsync(
						chatId: message.Chat.Id,
						text: "Для этой команды нужно авторизаваться в боте \nДля этого нужно прописать команды /start",
						parseMode: ParseMode.Markdown
					);
				}

			}
			else
			{
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
	}
}