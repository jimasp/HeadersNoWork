using Shouldly;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using WebApplication4;
using Xunit;

namespace Test
{
    public class Integration
    {
        [Fact]
        public void TestHomePageSingleThreaded()
        {
            using (var server = CreateServer())
            {
                for (int i = 0; i <= 20; i++) {
                    GetAsync(server, "/");
                }
            }
        }


        [Fact]
        public async Task TestHomePageParallelAsnyc()
        {
            using (var server = CreateServer())
            {
                var tasks = Enumerable.Range(1, 10).Select(i => 
                    GetAsync(server, "/" + i));

                await Task.WhenAll(tasks);
            }
        }


        private TestServer CreateServer()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var directory = Directory.GetCurrentDirectory();
            var setDir = Path.GetFullPath(
                Path.Combine(directory, @"..\..\..\..\..\WebApplication4")
            );

            var builder = new WebHostBuilder()
                .UseContentRoot(setDir)
                .UseStartup<Startup>();

            return new TestServer(builder);
        }


        private async Task GetAsync(TestServer server, string url)
        {
            using (var client = server.CreateClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000");
                var req = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url, UriKind.Relative)
                };
                Debug.WriteLine("### Getting:" + url);
                var resp = await client.SendAsync(req);
                resp.Dispose();
                Debug.WriteLine("### Done Getting:" + url);
            }
        }
    }
}


