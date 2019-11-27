using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TextsAPI.Services;
using TextsAPI.Models;
using AlternativeProcesses;
namespace TextsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessagesService _messagesService;

        public MessagesController(MessagesService messagesService)
        {
            _messagesService = messagesService;
        }

        [HttpGet("{SenderReceptor}")]
        public ActionResult<List<MessagesModel>> Get(string SenderReceptor)
        {
            var messages = _messagesService.Get(SenderReceptor);

            if (messages.Count == 0)
            {
                return StatusCode(404, "No existen mensajes");
            }
            // messages.Sort(); sortear por fecha
            return messages;
        }

        [HttpPost]
        public ActionResult<MessagesModel> Create(MessagesModel message)
        {
            try
            {
                _messagesService.Create(message);
                return StatusCode(200, "Su mensaje se envió con exito");
            }
            catch (Exception)
            {

                return StatusCode(500, "Contacte un desarrollador D:");
            }

        }
    }
}