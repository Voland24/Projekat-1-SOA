using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGateway.MQTT
{
	public class MQTTEvent
	{
		public string ISBN { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public int InLibrary { get; set; }
	}
}
