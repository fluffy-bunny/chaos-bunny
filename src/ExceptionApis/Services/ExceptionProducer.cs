using BazorAuth.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ExceptionApis
{
    public class ExceptionProducer : IExceptionProducer
    {
        private ILogger<ExceptionProducer> _logger;

        public ExceptionProducer(ILogger<ExceptionProducer> logger)
        {
            _logger = logger;
        }
        public Task ProductException(ExceptionType exceptionType)
        {
            try
            {
                switch (exceptionType)
                {
                    case ExceptionType.ArgumentNullException:
                        throw new ArgumentNullException();
                    case ExceptionType.NotImplementedException:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return Task.CompletedTask;
        }
    }
}
