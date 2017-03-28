using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PerfDiagnosticListener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var writePerfCountersTask = WritePerfCounters(cts.Token);

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();

            cts.Cancel();
            writePerfCountersTask.Wait();
        }

        private static async Task WritePerfCounters(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                PerfDiagnosticListener.Write(Console.Out);
            }
        }
    }
}
