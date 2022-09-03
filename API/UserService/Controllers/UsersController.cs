using AccountService.RPC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UserService.Models;
using UserService.RPC;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRPCProducer _producer;
        private readonly IExchangeProducer _exProducer;
        private readonly IDirectProducer _directProducer;
        private readonly IHttpClientFactory _httpClientFactory;

        public UsersController(IRPCProducer producer, IHttpClientFactory httpClientFactory, IExchangeProducer exProducer, IDirectProducer directProducer)
        {
            _producer = producer;
            _exProducer = exProducer;
            _directProducer = directProducer;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("")]
        public async Task<IActionResult> UserList()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:40487/weatherforecast");
            string content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);
            return Ok(json);
        }

        [HttpPost("add")]
        public IActionResult Add(User user)
        {
            _producer.SendProductMessage(user, "user");

            return Ok("User added successfully!");
        }

        [HttpPut("update")]
        public IActionResult Update(User user)
        {
            _exProducer.SendProductMessage(user, "user");

            return Ok("User updated successfully!");
        }

        [HttpPost("email")]
        public IActionResult SendEmail(User user)
        {
            _directProducer.SendProductMessage(user);

            return Ok("Email sent!");
        }
    }
}
