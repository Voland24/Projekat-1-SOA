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
			try
			{
				AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
				var channel = GrpcChannel.ForAddress("http://localhost:50051");
				var client = new AlertService.AlertServiceClient(channel);
				var reply = await client.QueryFluxAlertAsync(new QueryFluxInfo
				{
					Info = "average too high"
				});
				Console.WriteLine($"Got reply {reply.ResultInfo}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}
