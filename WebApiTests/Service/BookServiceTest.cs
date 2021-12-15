using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.Dto;
using WebApi.Models.Dto.Request;
using Xunit;

namespace WebApiTests.Service
{
    public class BookServiceTest
    {
        [Fact]
        public async Task GetBook_Should_Returns_BookDto()
        {
            // Arrange
            var book = new Book();
            var bookDto = new BookDto();
            var mock = new BookServiceMock();

            mock.BookRepository.Setup(m => m.GetById(It.IsAny<int>())).Returns(Task.FromResult(book));
            mock.Mapper.Setup(m => m.Map<Book, BookDto>(book)).Returns(bookDto);

            // Act
            var actual = await mock.Service.GetBook(513);

            // Assert
            Assert.Same(bookDto, actual);
            mock.BookRepository.Verify(x => x.GetById(513), Times.Once());
        }

        [Fact]
        public async Task GetBook_Should_Throw_Exception_If_Book_Not_Exists()
        {
            // Arrange
            var mock = new BookServiceMock();

            mock.BookRepository
                .Setup(m => m.GetById(It.IsAny<int>()))
                .Returns(Task.FromResult<Book>(null));

            // Act
            Task Action() => mock.Service.GetBook(404);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpNotFoundException>(Action);
            Assert.Equal("Book with given id not found", exception.Message);
        }

        [Fact]
        public async void Check_Book_Exist()
        {
            // Arrange
            const int id = 4568;
            var mock = new BookServiceMock();
            var random = new Random();
            var expected = random.Next(0, 1) == 1;

            mock.BookRepository.Setup(x => x.Exist(id)).ReturnsAsync(expected);

            // Act
            var actual = await mock.Service.Exist(id);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void GetBooks()
        {
            // Arrange
            var books = new List<Book> {Capacity = 0};
            var booksDto = new List<BookDto>() {new BookDto(), new BookDto()};

            var mock = new BookServiceMock();
            var order = new OrderDto();

            mock.BookRepository.Setup(x => x.GetAll()).ReturnsAsync(books);
            mock.Mapper.Setup(x => x.Map<IEnumerable<Book>, IEnumerable<BookDto>>(books)).Returns(booksDto);

            // Act
            var result = await mock.Service.GetBooks(order);

            // Assert
            Assert.Same(booksDto, result);
        }

        [Fact]
        public async void GetBooks_Should_Sort_Items()
        {
            // Arrange
            var books = new List<Book> {Capacity = 0};

            var bookDto1 = new BookDto() {Title = "Aaa"};
            var bookDto2 = new BookDto() {Title = "Bbb"};
            var booksDto = new List<BookDto>() {bookDto1, bookDto2};
            var expected = new List<BookDto>() {bookDto2, bookDto1};

            var mock = new BookServiceMock();
            var order = new OrderDto() {OrderBy = "title", Direction = "desc"};

            mock.BookRepository.Setup(x => x.GetAll()).ReturnsAsync(books);
            mock.Mapper.Setup(x => x.Map<IEnumerable<Book>, IEnumerable<BookDto>>(books)).Returns(booksDto);

            // Act
            var result = await mock.Service.GetBooks(order);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async void GetBooks_By_Author_Id()
        {
            // Arrange
            var book = new Mock<Book>();
            var booksDto = new Mock<IEnumerable<BookDto>>();
            const int authorId = 79;
            var books = new List<Book>();
            var booksDto = new List<Book>();

            var mock = new BookServiceMock();

            mock.BookRepository.Setup(x => x.GetByAuthorId(authorId)).ReturnsAsync(books);
            mock.Mapper.Setup(x => x.Map<IEnumerable<Book>, IEnumerable<BookDto>>(books)).Returns(booksDto);

            // Act
            var result = await mock.Service.GetBooks(order);

            // Assert
            Assert.Equal(books, result);
        }
    }
}
