using System.Reflection.Metadata.Ecma335;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using KursachBotRegata.Models;
using Telegram.Bot.Types.Enums;
using System;
using KursachBotRegata.Models.CallBacks;

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
            Console.WriteLine("Сообщение дошло!");
            var upd = JsonConvert.DeserializeObject<Update>(update.ToString());
            if (upd == null) 
                return Ok();

            var botClient = await Bot.GetBotClientAsync();
            
            switch (upd.Type)
            {
                case UpdateType.Message:
                    if(!Variables.StateList.ContainsKey(upd.Message.Chat.Id))
                        Variables.StateList[upd.Message.Chat.Id] = Variables.State.None;

                    Console.WriteLine(upd.Message.Text);
                    foreach (var command in Bot.Commands)
                    {
                        if (command.Contains(upd.Message))
                        {
                            await command.Execute(upd.Message, botClient);
                            System.Console.WriteLine(command.Name);
                            return Ok();
                        }
                    }

                    await Bot.Commands.Last().Execute(upd.Message, botClient);
                    return Ok(); 

                case UpdateType.CallbackQuery:
                    if(!Variables.StateList.ContainsKey(upd.CallbackQuery.Message.Chat.Id))
                        Variables.StateList[upd.Message.Chat.Id] = Variables.State.None;
                    var message = upd.CallbackQuery;
                    var commands = new CallBackQueryCommand();
                    await commands.Execute(message, botClient);
                    return Ok(); 

                default:
                    return Ok();
            }
        }
    }
}