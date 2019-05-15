using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Bnaya
{
    /// <summary>
    /// Test-server will start listening on creation and stop at disposal.
    /// I will retuen whatever it set to return for each request.
    /// </summary>
    public sealed class TestServer : IDisposable
    {
        private readonly IWebHost _host;

        #region CreateOK

        /// <summary>
        /// Start listening and return specific result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="port"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static IDisposable CreateOK<T>(T result, int port = 4445, string contentType = "application/json")
        {
            RequestDelegate requestDelegate = ctx =>
            {
                ctx.Response.ContentType = contentType;
                if(result is string content)
                    return ctx.Response.WriteAsync(content);

                var json = JsonConvert.SerializeObject(result);
                return ctx.Response.WriteAsync(json);
            };

            return new TestServer(requestDelegate, port);
        }

        #endregion // CreateOK

        #region CreateWithStatus
        
        /// <summary>
        /// tart listening and return specific status code.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="port"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static IDisposable CreateWithStatus(HttpStatusCode status, int port = 4445, string contentType = "application/json")
        {
            RequestDelegate requestDelegate = ctx =>
            {
                ctx.Response.ContentType = contentType;
                ctx.Response.StatusCode = (int)status;
                return Task.CompletedTask;
            };

            return new TestServer(requestDelegate, port);
        }

        #endregion // CreateWithStatus

        #region Ctor

        private TestServer(RequestDelegate requestDelegate, int port)
        {
            _host = new WebHostBuilder()
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseKestrel()
                   .UseIISIntegration()
                    .ConfigureServices(servicesCollection =>
                    {
                        servicesCollection.AddSingleton<RequestDelegate>(requestDelegate);
                    })
                   .UseStartup<Startup>()
                   .ConfigureKestrel((context, options) =>
                   {
                       options.Listen(IPAddress.Loopback, port);
                   })
                   .Build();

            _host.Start();
        }

        #endregion // Ctor

        #region Dispose

        public void Dispose()
        {
            _host.StopAsync().GetAwaiter().GetResult();
        }

        #endregion // Dispose
    }
}
