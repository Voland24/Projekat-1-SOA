using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGateway.Models
{
	public class OpenLibraryBookInfo
	{
		public string Title { get; set; }
		public List<string> Authors { get; set; }
		public string Status { get; set; }
		public string Uri { get; set; }

		public OpenLibraryBookInfo()
		{
			Authors = new List<string>();
		}
	}
}
