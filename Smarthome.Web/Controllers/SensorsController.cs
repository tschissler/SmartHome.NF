using Keba;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smarthome.Web.Components;

namespace Smarthome.Web.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly SensorsConnector sensorsConnector;

        public SensorsController(SensorsConnector sensorsConnector)
        {
            this.sensorsConnector = sensorsConnector;
        }

        [HttpPost("[action]")]
        public IActionResult RemoteDisplaySensorData([FromBody] RemoteDisplayData sensorData)
        {
            sensorsConnector.RemoteDisplayChanged(sensorData);
            return Ok();
        }
    }
}
