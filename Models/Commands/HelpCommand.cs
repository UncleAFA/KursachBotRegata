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
                                            "/createnewuser - добавление нового пользоателя";

                switch (Variables.InputDataList[message.Chat.Id].Post)
                {
                    case null:
                        await botClient.SendTextMessageAsync(
                            chatId:message.Chat.Id,
                            text: "Доступные вам команды:\n/start - авторизация \n/help - помощь", 
                            parseMode: ParseMode.Markdown
                        );
                        break;
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