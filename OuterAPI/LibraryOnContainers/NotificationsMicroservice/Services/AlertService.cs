using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationsMicroservice
{
	public class AlertService : Alert.AlertService.AlertServiceBase
	{
		private readonly ILogger<AlertService> _logger;
		public AlertService(ILogger<AlertService> logger)
		{
			_logger = logger;
		}

		public override Task<Alert.StatusMessage> QueryFluxAlert(Alert.QueryFluxInfo request, ServerCallContext context)
		{
			_logger.LogDebug("Alert method invoked!");
			return Task.FromResult(new Alert.StatusMessage
			{
				ResultInfo = "Success"
			});
		}
	}
}
