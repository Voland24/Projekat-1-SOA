using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Alert;

namespace GrpcTestApp
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var channel = GrpcChannel.ForAddress("https://localhost:5001");
			var client = new AlertService.AlertServiceClient(channel);
			var reply = await client.QueryFluxAlertAsync(new QueryFluxInfo
			{
				Info = "average two high"
			});
			Console.WriteLine($"Got reply {reply}");
		}
	}
}
