using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIGateway.DTOs
{
	public class DeleteBook
	{
		[JsonPropertyName("ISBN")]
		public string ISBN { get; set; }
	}
}
