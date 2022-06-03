using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIGateway.Models
{
	public class Book
	{
		[JsonPropertyName("ISBN")]
		public string ISBN { get; set; }
		public string BookTitle { get; set; }
		public string BookAuthor { get; set; }
		public string YearOfPublication { get; set; }
		public string Publisher { get; set; }
		public int Quantity { get; set; }
	}
}
