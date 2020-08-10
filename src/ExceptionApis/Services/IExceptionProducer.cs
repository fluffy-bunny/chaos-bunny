using BazorAuth.Shared;
using System.Threading.Tasks;

namespace ExceptionApis
{
    public interface IExceptionProducer
    {
        Task ProductException(ExceptionType exceptionType);
    }
}
