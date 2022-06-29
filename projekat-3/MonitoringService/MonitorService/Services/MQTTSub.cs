using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace MonitorService.Services
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
			_monitoringService.InitState();
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

				_mqttClient.ApplicationMessageReceivedAsync += async e =>
				{
					//_monitoringService.HandleMessage();
					//Console.WriteLine(e.ApplicationMessage.ConvertPayloadToString());
					await HandleAppMessage(e.ApplicationMessage);
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

		private async Task HandleAppMessage(MqttApplicationMessage mqttApplicationMessage)
		{
			var jsonRoot = JsonDocument.Parse(mqttApplicationMessage.ConvertPayloadToString()).RootElement;

			foreach (var reading in jsonRoot.GetProperty("readings").EnumerateArray())
			{
				var powerConsumer = reading.GetProperty("name").GetString();

				long miliseconds = reading.GetProperty("origin").GetInt64() / 1000000;
				var time = DateTimeOffset.FromUnixTimeMilliseconds(miliseconds).UtcDateTime;

				int power = int.Parse(reading.GetProperty("value").GetString());

				await _monitoringService.HandleMessage(time, powerConsumer, power);
			}
		}
	}
}
