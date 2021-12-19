using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace SmartHome.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        [HttpPost]
        public void Log([FromBody] string logData)
        {
            Debug.WriteLine(logData);
            LogMessagesManager.Enqueue(logData);
        }

        [HttpGet]
        public string Status()
        {
            return "Logging Service is up and running.";
        }

        [HttpGet]
        [Route("/api/[controller]/ReadLogMessages") ]
        public string[] ReadLogMessages()
        {
            return LogMessagesManager.ReadLogMessages();
        }
    }
}
