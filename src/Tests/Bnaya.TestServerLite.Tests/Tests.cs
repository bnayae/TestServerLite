using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;

// link: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-2.2

namespace Bnaya.TestServerLite.Tests
{
    public sealed class Tests
    {
        private const string URL = "http://localhost:4445";
        private HttpClient _unitUnderTest;

        public Tests()
        {
            _unitUnderTest = new HttpClient();
        }


        [Fact]
        public async Task HappyPathDtoTest()
        {
            var content =  new Dto { Id = 2, Name = "Bnaya" };
            using (TestServer.CreateOK(content))
            {
                var response = await _unitUnderTest.GetAsync(URL).ConfigureAwait(false);
                var result = await response.Content.ReadAsAsync<Dto>().ConfigureAwait(false);
                Assert.Equal(content, result);
            }
        }

        [Fact]
        public async Task HappyPathStringTest()
        {
            var content = "Hello world";
            using (TestServer.CreateOK(content))
            {
                var result = await _unitUnderTest.GetStringAsync(URL).ConfigureAwait(false);
                Assert.Equal(content, result);
            }
        }

        [Theory]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.Conflict)]
        public async Task FaultPathTest(HttpStatusCode status)
        {
            using (TestServer.CreateWithStatus(status))
            {
                var response = await _unitUnderTest.GetAsync(URL)
                                             .ConfigureAwait(false);
                Assert.False(response.IsSuccessStatusCode);
                Assert.Equal(status, response.StatusCode);
            }
        }
    }
}
