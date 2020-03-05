using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpHttpLong.Middleware
{
    public class LongHttpMiddleware : IMiddleware
    {
        List<HttpContext> httpContexts = new List<HttpContext>();
        public void InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    httpContexts.ForEach(x =>
                    {
                        x.Response.WriteAsync("123");
                    });
                    Thread.Sleep(10000);
                }
            });
            httpContexts.Add(context);
        }

        Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}
