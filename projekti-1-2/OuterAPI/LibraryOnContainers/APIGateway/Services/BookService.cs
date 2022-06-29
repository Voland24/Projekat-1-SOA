using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIGateway.Clients;
using APIGateway.DTOs;
using APIGateway.Models;

namespace APIGateway.Services
{
	public interface IBookService
	{
		Task<BooksResponse> GetBookByTitle(string title);
		Task<BooksResponse> GetBookByAuthor(string author);
		Task<BooksResponse> GetBookByISBN(string isbn);
		Task<Book> AddBook(Book book);
		Task<Book> UpdateQuantity(string isbn, int increase);
		Task<bool> DeleteBook(string isbn);
	}
	public class BookService : IBookService
	{
		private IOpenLibraryClient _openLibraryClient;
		private IExpressApiClient _expressApiClient;

		public BookService(IOpenLibraryClient openLibraryClient, IExpressApiClient expressApiClient)
		{
			_openLibraryClient = openLibraryClient;
			_expressApiClient = expressApiClient;
		}

		public async Task<Book> AddBook(Book book)
		{
			return await _expressApiClient.AddBook(book);
		}

		public async Task<bool> DeleteBook(string isbn)
		{
			return await _expressApiClient.DeleteBook(isbn);
		}

		public async Task<BooksResponse> GetBookByAuthor(string author)
		{
			return new BooksResponse
			{
				InLibrary = await _expressApiClient.GetBookByAuthor(author),
				OnInternet = await _openLibraryClient.BooksByAuthor(author)
			};
		}

		public async Task<BooksResponse> GetBookByISBN(string isbn)
		{
			return new BooksResponse
			{
				InLibrary = new List<Book>() { await _expressApiClient.GetBookByISBN(isbn) },
				OnInternet = new List<OpenLibraryBookInfo>() { await _openLibraryClient.BookByISBN(isbn) }
			};
		}

		public async Task<BooksResponse> GetBookByTitle(string title)
		{
			return new BooksResponse
			{
				InLibrary = await _expressApiClient.GetBookByTitle(title),
				OnInternet = await _openLibraryClient.BooksByTitle(title)
			};
		}

		public async Task<Book> UpdateQuantity(string isbn, int increase)
		{
			return await _expressApiClient.UpdateQuantity(isbn, increase);
		}
	}
}
