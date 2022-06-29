using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MonitoringService.Services
{

	public interface IMonitoringService
	{
		void HandleMessage();
	}
	public class MonitorService : IMonitoringService
	{
		private HttpClient _commandClient;

		public MonitorService(HttpClient commandClient)
		{
			_commandClient = commandClient;
			_commandClient.BaseAddress = new Uri("http://edgex-core-command:48082/api/v1/device/20558a1e-9c51-4ad0-be63-3fc100fed8dc/command/27da2d31-d4a0-44a3-add0-4e1abe5b7309");
			_commandClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public void HandleMessage()
		{
			throw new NotImplementedException();
		}
	}
}
