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
                    Get(server, "/");
                }
            }
        }


        [Fact]
        public void TestHomePageParallel()
        {
            int i=0;
            using (var server = CreateServer())
            {
                Parallel.For((long) 0, 10, index =>
                {
                    i++;
                    Get(server, "/" + i);
                });
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


        public void Get(TestServer server, string url)
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
                var resp = client.SendAsync(req).Result;
                resp.Dispose();
                Debug.WriteLine("### Done Getting:" + url);
            }
        }
    }
}


