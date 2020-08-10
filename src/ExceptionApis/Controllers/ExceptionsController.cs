using BazorAuth.Shared;
using ExceptionApis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InMemoryIdentityApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ExceptionsController : ControllerBase
    {
        private IExceptionProducer _exceptionProducer;
        private ILogger<ExceptionsController> _logger;

        public ExceptionsController(
            IExceptionProducer exceptionProducer,
            ILogger<ExceptionsController> logger)
        {
            _exceptionProducer = exceptionProducer;
            _logger = logger;
        }
        [HttpGet("{et}")]
        public async Task<ActionResult> ProduceExceptionAsync(ExceptionType et)
        {
            await _exceptionProducer.ProductException(et);
            return NoContent();
        }

    }
}
