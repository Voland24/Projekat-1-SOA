using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceWebAPI.Services
{
	public interface IPowerSavingService
	{
		public bool SavingModeOn { get; set; }
	}
	public class PowerSavingService : IPowerSavingService
	{
		public bool SavingModeOn { get; set; }

		public PowerSavingService()
		{
			SavingModeOn = false;
		}

	}
}
