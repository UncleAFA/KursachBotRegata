using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using KursachBotRegata.Models;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
// using KursachBotRegata.Models.CallBack;
using static KursachBotRegata.Models.AppSettings;
using KursachBotRegata.Models.Commands;

namespace KursachBotRegata.Controllers
{
    [Route("api/message/update")]
    public class MessageController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return string.Empty;
        }

        [HttpPost]
        public async Task<OkResult> Post([FromBody] object update)
        {
            System.Console.WriteLine("Сообщение дошло!");
            var upd = JsonConvert.DeserializeObject<Update>(update.ToString());
            if (upd == null) 
                return Ok();

            switch (upd.Type)
            {
                case UpdateType.Message:
                    return Ok();

                case UpdateType.CallbackQuery:
                    return Ok(); 

                default:
                    return Ok();
            }

            // if (upd.Type == UpdateType.Message)
            // {
            //     var commands = Bot.Commands;
            //     var message = upd.Message;
            //     System.Console.WriteLine(message.Text);
            //     var botClient = await Bot.GetBotClientAsync();

            //     foreach (var command in commands)
            //     {

            //         try
            //         {
            //             if (StateList[message.Chat.Id] != State.None)
            //             {
            //                 var func = new AddingLine();
            //                 await func.Execute(message, botClient);
            //                 return Ok();
            //             }
            //         }
            //         catch (System.Exception)
            //         {
            //             System.Console.WriteLine("error");
            //         }
            //         if (command.Contains(message))
            //         {
            //             await command.Execute(message, botClient);
            //             break;
            //         }
            //     }
            // }
            // if (upd.Type == UpdateType.CallbackQuery)
            // {
            //     var message = upd.CallbackQuery;
            //     var botClient = await Bot.GetBotClientAsync();
            //     var commands = new CallBackQueryCommand();
            //     await commands.Execute(message, botClient);
            // }
        }
    }
}