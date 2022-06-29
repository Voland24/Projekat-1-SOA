using APIGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGateway.DTOs
{
	public class BooksResponse
	{
		public List<Book> InLibrary { get; set; }
		public List<OpenLibraryBookInfo> OnInternet { get; set; }
	}
}
