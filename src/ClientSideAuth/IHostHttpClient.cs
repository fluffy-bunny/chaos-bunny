using System.Net.Http;

namespace ClientSideAuth
{
    public interface IHostHttpClient
    {
        HttpClient CreateHttpClient();
    }
}
