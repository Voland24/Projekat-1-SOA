using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using APIGateway.DTOs;
using APIGateway.Models;
using Microsoft.Extensions.Configuration;

namespace APIGateway.Clients
{
	public interface IExpressApiClient
	{
		Task<List<Book>> GetBookByTitle(string title);
		Task<List<Book>> GetBookByAuthor(string author);
		Task<Book> GetBookByISBN(string isbn);
		Task<Book> AddBook(Book book);
		Task<Book> UpdateQuantity(string isbn, int increase);
		Task<bool> DeleteBook(string isbn);
	}
	public class ExpressApiClient : IExpressApiClient
	{
		private HttpClient _client;

		public ExpressApiClient(HttpClient client, IConfiguration configuration)
		{
			_client = client;
			// TODO: load from configuration
			_client.BaseAddress = new Uri("http://localhost:8080/");
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		#region Public methods
		public async Task<Book> AddBook(Book book)
		{
			var serialized = JsonSerializer.Serialize(
				book,
				typeof(Book),
				new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});
			var response = await _client.PostAsync(
				"insertNewBook",
				new StringContent(serialized, System.Text.Encoding.UTF8, "application/json")
			);
			return await BookFromResponse (response);
		}

		public async Task<List<Book>> GetBookByAuthor(string author)
		{
			var response = await _client.GetAsync($"getBookByAuthor?bookAuthor={author}");
			return await BooksFromResponse(response);
		}

		public async Task<Book> GetBookByISBN(string isbn)
		{
			var response = await _client.GetAsync($"getBookByISBN?ISBN={isbn}");
			return await BookFromResponse(response);
		}

		public async Task<List<Book>> GetBookByTitle(string title)
		{
			var response = await _client.GetAsync($"getBookByTitle?bookTitle={title}");
			return await BooksFromResponse(response);
		}

		public async Task<Book> UpdateQuantity(string isbn, int increase)
		{
			var update = new IncreaseBookQuantity
			{
				ISBN = isbn,
				Operation = increase
			};
			var serialized = JsonSerializer.Serialize(
				update,
				typeof(IncreaseBookQuantity),
				new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});
			var response = await _client.PutAsync(
				"updateBookQuantity",
				new StringContent(serialized, System.Text.Encoding.UTF8, "application/json")
			);
			return await BookFromResponse(response);
		}

		public async Task<bool> DeleteBook(string isbn)
		{
			var response = await _client.DeleteAsync($"deleteABook?ISBN={isbn}");
			return response.IsSuccessStatusCode;
		}
		#endregion

		#region Private methods
		private async Task<Book> BookFromResponse(HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode)
			{
				// TODO: add logging (get logger through DI)
				return null;
			}
			var book = JsonSerializer.Deserialize<Book>(
				await response.Content.ReadAsStringAsync(),
				new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});
			return book;
		}

		private async Task<List<Book>> BooksFromResponse(HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode)
			{
				// TODO: add logging (get logger through DI)
				return null;
			}
			var jsonString = await response.Content.ReadAsStringAsync();
			var books = JsonSerializer.Deserialize<List<Book>>(
				jsonString,
				new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});
			return books;
		}
		#endregion
	}
}
