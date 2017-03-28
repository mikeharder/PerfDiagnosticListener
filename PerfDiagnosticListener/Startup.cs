using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PerfDiagnosticListener
{
    public class Startup
    {
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            DiagnosticListener diagnosticListener)
        {
            diagnosticListener.SubscribeWithAdapter(new PerfDiagnosticListener());

            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                // Limit throughput for testing current requests
                await _semaphoreSlim.WaitAsync();
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                finally
                {
                    _semaphoreSlim.Release();
                }

                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
