using APIGateway.Clients;
using APIGateway.DTOs;
using APIGateway.Models;
using APIGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGateway.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BooksController : ControllerBase
	{
		private IOpenLibraryClient _client;
		private IBookService _bookService;

		public BooksController(IBookService bookService, IOpenLibraryClient client)
		{
			_bookService = bookService;
			_client = client;
		}

		[HttpGet]
		[Route("searchTitle")]
		public async Task<BooksResponse> SearchByTitle([FromQuery(Name = "title")] string title)
		{
			return await _bookService.GetBookByTitle(title);
		}

		[HttpGet]
		[Route("searchAuthor")]
		public async Task<BooksResponse> SearchByAuthor([FromQuery(Name = "author")] string author)
		{
			return await _bookService.GetBookByAuthor(author);
		}

		[HttpGet]
		[Route("")]
		public async Task<BooksResponse> SearchByISBN([FromQuery(Name = "isbn")] string isbn)
		{
			return await _bookService.GetBookByISBN(isbn);
		}

		[HttpPost]
		[Route("")]
		public async Task<IActionResult> InsertBook([FromBody] Book book)
		{
			var insertedBook = await _bookService.AddBook(book);
			if (insertedBook is null)
				return StatusCode(StatusCodes.Status500InternalServerError);
			else
				return Ok(insertedBook);
		}

		[HttpPut]
		[Route("{isbn}/increaseQuantity")]
		public async Task<IActionResult> IncreaseQuantity([FromRoute] string isbn, [FromQuery(Name = "inc")] int inc)
		{
			var book = await _bookService.UpdateQuantity(isbn, inc);
			if (book is null)
				return BadRequest();
			else
				return Ok(book);
		}

		[HttpDelete]
		[Route("")]
		public async Task<IActionResult> RemoveBook([FromQuery(Name = "isbn")] string isbn)
		{
			if (await _bookService.DeleteBook(isbn))
				return Ok();
			else
				return BadRequest();
		}
	}
}
