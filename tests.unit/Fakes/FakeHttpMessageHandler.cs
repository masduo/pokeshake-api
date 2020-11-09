using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Unit.Fakes
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage _fakeResponse;

        public FakeHttpMessageHandler(HttpResponseMessage responseMessage)
        {
            _fakeResponse = responseMessage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_fakeResponse);
        }
    }
}
