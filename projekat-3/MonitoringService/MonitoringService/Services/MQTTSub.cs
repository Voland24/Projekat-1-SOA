using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace MonitoringService.Services
{
	public class MQTTSub
	{
		private IMqttClient _mqttClient;
		private MqttFactory _mqttFactory;
		private IMonitoringService _monitoringService;

		public MQTTSub(IMonitoringService monitoringService)
		{
			_mqttFactory = new MqttFactory();
			_mqttClient = _mqttFactory.CreateMqttClient();
			_monitoringService = monitoringService;
		}

		public void Dispose()
		{
			_mqttClient.Dispose();
		}

		public async Task Subscribe()
		{
			if (!_mqttClient.IsConnected)
			{
				var mqttClientOptions = new MqttClientOptionsBuilder()
				.WithTcpServer("mosquitto", 1883)
				.Build();

				_mqttClient.ApplicationMessageReceivedAsync += e =>
				{
					Console.WriteLine("Received message:");
					//_monitoringService.HandleMessage();
					Console.WriteLine(e.ApplicationMessage.ConvertPayloadToString());
					return Task.CompletedTask;
				};

				_mqttClient.ConnectedAsync += e =>
				{
					Console.WriteLine("Connected to Mqtt!");
					return Task.CompletedTask;
				};

				await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
			}
			
			var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
				.WithTopicFilter(f => { f.WithTopic("edgeX/power_consumption"); })
				.Build();

			await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

			Console.WriteLine("MQTT client subscribed to topic.");
		}
	}
}
