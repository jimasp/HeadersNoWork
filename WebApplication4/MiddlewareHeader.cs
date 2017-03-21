using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;


namespace WebApplication4
{
    public class MiddlewareHeader
    {
        private readonly RequestDelegate _next;

        public MiddlewareHeader(RequestDelegate next) {
            _next = next;
        }

        public static readonly object obj = new object();

        public async Task Invoke(HttpContext context)
        {
            Debug.WriteLine("###Invoked:url:" + context.Request.GetDisplayUrl());
            context.Response.OnStarting(() =>
            {

                    Debug.WriteLine($"###CTX:{context.GetHashCode()}, url:{context.Request.GetDisplayUrl()}");
                    context.Response.Headers.Add("MyHeader", "header");
                    return Task.CompletedTask;

            });

            await _next.Invoke(context);

            Debug.WriteLine("next.Invoke called for url:" + context.Request.GetDisplayUrl());
        }

    }
}
