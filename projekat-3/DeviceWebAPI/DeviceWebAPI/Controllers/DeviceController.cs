using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceWebAPI.Services;
using System.Text.Json;

namespace DeviceWebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DeviceController : ControllerBase
	{
		private readonly ILogger<DeviceController> _logger;
		private IPowerSavingService _powerSavingService;

		public DeviceController(ILogger<DeviceController> logger, IPowerSavingService powerSavingService)
		{
			_logger = logger;
			_powerSavingService = powerSavingService;
		}

		[HttpPut]
		[Route("savingMode")]
		public IActionResult SetSavingMode([FromBody] string onoff)
		{
			_logger.LogInformation("Handling request...");
			switch (onoff)
			{
				case "ON":
					_powerSavingService.SavingModeOn = true;
					_logger.LogInformation("Saving mode turned ON!");
					return Ok("Saving mode turned ON!");
				case "OFF":
					_powerSavingService.SavingModeOn = false;
					_logger.LogInformation("Saving mode turned OFF");
					return Ok("Saving mode turned OFF!");
				default:
					_logger.LogInformation("Request hasn't change saving mode!");
					return BadRequest("Request hasn't changed the saving mode!");
			}
		}

		[HttpGet]
		[Route("savingMode")]
		public IActionResult GetSavingModeState()
		{
			return Ok(_powerSavingService.SavingModeOn ? "ON" : "OFF");
		}

		[HttpPost]
		[Route("proba")]
		public async Task<IActionResult> Proba()
		{
			var body = await JsonDocument.ParseAsync(Request.Body);
			var prop3 = body.RootElement.GetProperty("prop3");
			var str = prop3.GetRawText();
			object obj = JsonSerializer.Deserialize<object>(str);
			return Ok(obj);
		}
	}
}
