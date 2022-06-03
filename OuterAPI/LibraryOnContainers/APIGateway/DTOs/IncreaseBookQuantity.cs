using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIGateway.DTOs
{
	public class IncreaseBookQuantity
	{
		[JsonPropertyName("ISBN")]
		public string ISBN { get; set; }
		public int Operation { get; set; } // either +1 or -1
	}
}
