using APIGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Web;
using System.Collections.Specialized;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace APIGateway.Clients
{
	public interface IOpenLibraryClient
	{
		Task<List<OpenLibraryBookInfo>> Search(string query);
		Task<List<OpenLibraryBookInfo>> BooksByAuthor(string author);
		Task<List<OpenLibraryBookInfo>> BooksByTitle(string title);
		Task<OpenLibraryBookInfo> BookByISBN(string isbn);
	}
	public class OpenLibraryClient : IOpenLibraryClient
	{
		private HttpClient _client;

		public OpenLibraryClient(HttpClient client, IConfiguration configuration)
		{
			_client = client;
			// TODO load from configuration
			_client.BaseAddress = new Uri("https://openlibrary.org/");
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<List<OpenLibraryBookInfo>> BooksByAuthor(string author)
		{
			return await SearchApiQuery("author", author);
		}

		public async Task<List<OpenLibraryBookInfo>> BooksByTitle(string title)
		{
			return await SearchApiQuery("title", title);
		}

		public async Task<List<OpenLibraryBookInfo>> Search(string query)
		{
			return await SearchApiQuery("q", query);
		}

		public async Task<OpenLibraryBookInfo> BookByISBN(string isbn)
		{
			var requestUrl = BuildISBNQueryString(isbn);

			var response = await _client.GetAsync(requestUrl);

			if (!response.IsSuccessStatusCode)
				return null;

			return await ParseBookResponse(response, isbn);
		}

		#region Private Methods
		private async Task<List<OpenLibraryBookInfo>> SearchApiQuery(string searchField, string searchValue)
		{
			var requestUrl = BuildSearchQueryString(searchField, searchValue);

			var response = await _client.GetAsync(requestUrl);

			if (!response.IsSuccessStatusCode)
				return null;

			return await ParseSearchResponse(response);
		}
		private string BuildSearchQueryString(string searchField, string searchValue)
		{
			var queryParams = HttpUtility.ParseQueryString(string.Empty);
			queryParams.Add(searchField, searchValue);
			queryParams.Add("fields", "title");
			queryParams.Add("fields", "author_name");
			queryParams.Add("limit", "3");
			return $"search.json?{queryParams}&fields=title,author_name,ia,availability";
		}
		private string BuildISBNQueryString(string isbn)
		{
			var queryParams = HttpUtility.ParseQueryString(string.Empty);
			queryParams.Add("format", "json");
			queryParams.Add("jscmd", "data");
			return $"api/books?bibkeys=ISBN:{isbn}&{queryParams}";
		}
		private async Task<List<OpenLibraryBookInfo>> ParseSearchResponse(HttpResponseMessage response)
		{
			var booksInfo = new List<OpenLibraryBookInfo>();
			using var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
			var docs = jsonDoc.RootElement.GetProperty("docs");
			foreach (var doc in docs.EnumerateArray())
			{
				var bookInfo = new OpenLibraryBookInfo();
				if (doc.TryGetProperty("title", out JsonElement title))
					bookInfo.Title = title.GetString();
				if (doc.TryGetProperty("author_name", out JsonElement authors))
				{
					foreach (var author in authors.EnumerateArray())
					{
						bookInfo.Authors.Add(author.GetString());
					}
				}
				if (doc.TryGetProperty("availability", out JsonElement availability))
				{
					if (availability.TryGetProperty("status", out JsonElement status))
						bookInfo.Status = status.GetString();
					if (availability.TryGetProperty("identifier", out JsonElement ia))
						bookInfo.Uri = $"https://archive.org/details/{ia.GetString()}";
				}
				booksInfo.Add(bookInfo);
			}
			return booksInfo;
		}
		private async Task<OpenLibraryBookInfo> ParseBookResponse(HttpResponseMessage response, string isbn)
		{
			var booksInfo = new List<OpenLibraryBookInfo>();
			using var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
			var doc = jsonDoc.RootElement.GetProperty($"ISBN:{isbn}");
			var bookInfo = new OpenLibraryBookInfo();
			if (doc.TryGetProperty("title", out JsonElement title))
				bookInfo.Title = title.GetString();
			if (doc.TryGetProperty("authors", out JsonElement authors))
			{
				foreach (var author in authors.EnumerateArray())
				{
					if (author.TryGetProperty("name", out JsonElement name))
					{
						bookInfo.Authors.Add(name.GetString());
					}
				}
			}
			if (doc.TryGetProperty("ebooks", out JsonElement ebooks))
			{
				foreach (var ebook in ebooks.EnumerateArray())
				{
					if (ebook.TryGetProperty("preview_url", out JsonElement uri))
					{
						bookInfo.Uri = uri.GetString();
					}
					if (ebook.TryGetProperty("availability", out JsonElement status))
					{
						bookInfo.Status = status.GetString();
					}
					break;
				}
			}
			return bookInfo;
		}

		#endregion
	}
}
