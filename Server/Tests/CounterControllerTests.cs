using Microsoft.AspNetCore.Mvc.Testing;
using Stromzaehler;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class CounterControllerTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public CounterControllerTests(WebApplicationFactory<Startup> factory) 
        {
            this.factory = factory;
        }

        [Theory]
        [InlineData(@"{""Count"":1,""SourceString"":""0""}")]
        [InlineData(@"{""Count"":1,""SourceString"":""Huhu""}")]
        public async Task Fails_without_source(string msg)
        {
            var client = factory.CreateClient();
            var c = new StringContent(msg, Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("/api/Counter", c);
            var content = await resp.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Theory]
        [InlineData(@"{""Count"":1,""SourceString"":""Power""}")]
        [InlineData(@"{""Count"":1,""SourceString"":""Water""}")]
        public async Task Can_add_counts(string msg)
        {
            var client = factory.CreateClient();
            var c = new StringContent(msg, Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("/api/Counter", c);
            var content = await resp.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }
    }
}
