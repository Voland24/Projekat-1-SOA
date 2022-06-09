using APIGateway.Clients;
using APIGateway.DTOs;
using APIGateway.Models;
using APIGateway.MQTT;
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
		private IBookService _bookService;
		private IMQTTService _mqttService;
		public BooksController(IBookService bookService, IMQTTService mqttService)
		{
			_bookService = bookService;
			_mqttService = mqttService;
		}

		[HttpGet]
		[Route("searchTitle")]
		public async Task<BooksResponse> SearchByTitle([FromQuery(Name = "title")] string title)
		{
			var result = await _bookService.GetBookByTitle(title);
			await _mqttService.PublishEvent(new MQTTEvent
			{
				Title = title,
				InLibrary = result.InLibrary.Count
			});
			return result;
		}

		[HttpGet]
		[Route("searchAuthor")]
		public async Task<BooksResponse> SearchByAuthor([FromQuery(Name = "author")] string author)
		{
			var result = await _bookService.GetBookByAuthor(author);
			await _mqttService.PublishEvent(new MQTTEvent
			{
				Author = author,
				InLibrary = result.InLibrary.Count
			});
			return result;
		}

		[HttpGet]
		[Route("")]
		public async Task<BooksResponse> SearchByISBN([FromQuery(Name = "isbn")] string isbn)
		{
			var result = await _bookService.GetBookByISBN(isbn);
			await _mqttService.PublishEvent(new MQTTEvent
			{
				ISBN = isbn,
				InLibrary = result.InLibrary.Count
			});
			return result;
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
