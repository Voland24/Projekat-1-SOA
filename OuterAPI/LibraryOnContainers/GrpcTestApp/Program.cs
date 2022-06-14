using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using NotificationsMicroservice;

namespace GrpcTestApp
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var channel = GrpcChannel.ForAddress("https://localhost:5001");
			var client = new Alerter.AlerterClient(channel);
			var reply = await client.AlertAsync(new AlertInfo
			{
				QueryCount = 4
			});
			Console.WriteLine("Got reply");
		}
	}
}
