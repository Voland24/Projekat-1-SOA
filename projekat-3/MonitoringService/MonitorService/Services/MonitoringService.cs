using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonitorService.Services
{

	public interface IMonitoringService
	{
		Task HandleMessage(DateTime time, string powerConsumer, int powerValue);
		Task InitState();
	}
	public class MonitoringService : IMonitoringService
	{
		class ExponentialWeightingAverage
		{
			public int Value { get; set; }
			public int Count { get; set; }
		};

		private HttpClient _commandClient;
		private Dictionary<string, ExponentialWeightingAverage> _runningAverages;
		private readonly double _beta = 0.9;
		private readonly int _threshold = 600;
		private bool _savingModeOn = false;

		public MonitoringService(HttpClient commandClient)
		{
			_commandClient = commandClient;
			_commandClient.BaseAddress = new Uri("http://edgex-core-command:48082/");
			_commandClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			_runningAverages = new Dictionary<string, ExponentialWeightingAverage>();

			_runningAverages["fridge"] = new ExponentialWeightingAverage { Value = 0, Count = 0 };
			_runningAverages["furnace"] = new ExponentialWeightingAverage { Value = 0, Count = 0 };
			_runningAverages["office"] = new ExponentialWeightingAverage { Value = 0, Count = 0 };
			_runningAverages["dishwasher"] = new ExponentialWeightingAverage { Value = 0, Count = 0 };
		}

		public async Task HandleMessage(DateTime time, string powerConsumer, int powerValue)
		{
			if (!_runningAverages.TryGetValue(powerConsumer, out ExponentialWeightingAverage average))
				return;
			
			++average.Count;
			average.Value = (int)(_beta * average.Value + (1 - _beta) * powerValue); // / (1 - Math.Pow(_beta, average.Count))) ;

			int sumOfAverages = 0;
			foreach(var (device, avg) in _runningAverages)
			{
				if (avg.Count % 25 == 24)
				{
					Console.WriteLine($"After {avg.Count} measurements, EWA({device}) = {avg.Value}");
				}
				sumOfAverages += avg.Value;
			}

			if (sumOfAverages > _threshold && !_savingModeOn)
			{
				await SendCommand(_savingModeOn = true);
			}
			else if (sumOfAverages < _threshold && _savingModeOn)
			{
				await SendCommand(_savingModeOn = false);
			}
		}

		private async Task SendCommand(bool on)
		{
			var response = await _commandClient.PutAsync(
				"api/v1/device/20558a1e-9c51-4ad0-be63-3fc100fed8dc/command/27da2d31-d4a0-44a3-add0-4e1abe5b7309",
				new StringContent(
					on ? "\"ON\"" : "\"OFF\"",
					System.Text.Encoding.UTF8, "application/json"));
			
			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine("Error encountered during communication with EdgeX");
			}
			else
			{
				Console.WriteLine($"Saving mode on: {on}");
			}
		}

		public async Task InitState()
		{
			var response = await _commandClient.GetAsync("api/v1/device/20558a1e-9c51-4ad0-be63-3fc100fed8dc/command/27da2d31-d4a0-44a3-add0-4e1abe5b7309");
			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("State initialized!");
				var current = await response.Content.ReadAsStringAsync();
				if (current.Equals("ON"))
					_savingModeOn = true;
				else if (current.Equals("OFF"))
					_savingModeOn = false;
			}
			else
			{
				Console.WriteLine("Error encountered during communication with EdgeX");
			}
		}
	}
}
