using AccountService.RPC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRPCProducer _producer;

        public UsersController(IRPCProducer producer)
        {
            _producer = producer;
        }

        public IActionResult UserList()
        {
            return Ok("Users");
        }

        [HttpPost("add")]
        public IActionResult Add(User user)
        {
            _producer.SendProductMessage(user, "user");

            return Ok("User added successfully!");
        }
    }
}
