using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using MonitorService.Services;
using System.Threading.Tasks;

namespace MonitorService
{
	class Program
	{
		static async Task Main(string[] args)
		{
			IHost host = Host.CreateDefaultBuilder(args).
				ConfigureServices((_, services) =>
				{
					services.AddHttpClient<IMonitoringService, MonitoringService>();
					services.AddSingleton<MQTTSub>();
				})
				.Build();

			var mqttSubscriber = host.Services.GetRequiredService<MQTTSub>();
			await mqttSubscriber.Subscribe();
			await host.RunAsync();
		}
	}
}
