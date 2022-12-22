using GrafanaTest.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;

namespace GrafanaTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly ILogger<PersonsController> logger;
        private readonly ApplicationContext applicationContext;

        public PersonsController(ILogger<PersonsController> logger, ApplicationContext applicationContext)
        {
            this.logger = logger;
            this.applicationContext = applicationContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            logger.LogInformation($"Getting Weather Forecast traceID={Tracer.CurrentSpan.Context.TraceId.ToHexString()}");
            var result = await applicationContext.Persons.Include(x => x.Dependents).ToListAsync();
            var result2 = await applicationContext.Persons.FirstAsync(x => x.Id == 1);
            return Ok(result);
        }
    }
}