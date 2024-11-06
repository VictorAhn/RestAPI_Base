using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Security.Authentication;
using System.Text;

namespace RestAPI_Base
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.Limits.MinRequestBodyDataRate = new MinDataRate(100, TimeSpan.FromSeconds(10));
                        serverOptions.Limits.MinResponseDataRate = new MinDataRate(100, TimeSpan.FromSeconds(10));
                        serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(3);
                        serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(3);
                        serverOptions.Limits.MaxResponseBufferSize = 4120100000;
                        serverOptions.ConfigureHttpsDefaults(listenOptions =>
                        {
                            listenOptions.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
                        });
                    })
                    .UseStartup<Startup>()
                    .UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = 4120100000;
                    });
                });
    }
}
