using HallOfFame.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace HallOfFame.IntegrTest
{

        public class BasicTests
    : IClassFixture<WebApplicationFactory<HallOfFame.Web.Startup>>
        {
            private readonly WebApplicationFactory<HallOfFame.Web.Startup> _factory;

            public BasicTests(WebApplicationFactory<HallOfFame.Web.Startup> factory)
            {
                _factory = factory;
            }

            [Theory]
            [InlineData("/")]
            [InlineData("/Index")]
            [InlineData("/About")]
            [InlineData("/Privacy")]
            [InlineData("/Contact")]
            public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
            {
                // Arrange
                var client = _factory.CreateClient();

                // Act
                var response = await client.GetAsync(url);

                // Assert
                response.EnsureSuccessStatusCode(); // Status Code 200-299
                Assert.Equal("text/html; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }
       
}
