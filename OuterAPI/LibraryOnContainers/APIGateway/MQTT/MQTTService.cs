using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace APIGateway.MQTT
{
	public interface IMQTTService
	{
		Task PublishEvent(MQTTEvent mqttEvent);
	}
	public class MQTTService : IMQTTService, IDisposable
	{
		private IMqttClient _mqttClient;
		private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		public MQTTService()
		{
			var mqttFactory = new MqttFactory();
			_mqttClient = mqttFactory.CreateMqttClient();
		}

		public void Dispose()
		{
			_mqttClient.Dispose();
		}

		public async Task PublishEvent(MQTTEvent mqttEvent)
		{
			if (!_mqttClient.IsConnected)
				await Connect();
			var applicationMessage = new MqttApplicationMessageBuilder()
					.WithTopic("soa/bookQueries")
					.WithPayload(JsonSerializer.Serialize(mqttEvent, _jsonSerializerOptions ))
					.Build();

			await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
		}

		private async Task Connect()
		{
			var mqttClientOptions = new MqttClientOptionsBuilder()
					.WithTcpServer("mosquitto", 1883)
					.Build();
			await _mqttClient.ConnectAsync(mqttClientOptions);
		}

	}
}
