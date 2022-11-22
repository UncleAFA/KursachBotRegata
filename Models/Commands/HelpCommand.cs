using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace KursachBotRegata.Models.Commands
{
    public class HelpCommand : Command
    {
        public override string Name => @"/help";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            if(Variables.StateList[message.Chat.Id] == Variables.State.None)
            {
                // TODO: реализовать доступность команд от должности User
                await botClient.SendTextMessageAsync(
                    chatId:message.Chat.Id,
                    text: "Доступные вам команды:\n/start - Старт(приветствие) \n/help - помощь", 
                    parseMode: ParseMode.Markdown
                );
            }
            else{
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