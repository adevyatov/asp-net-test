using System;
using System.Threading.Tasks;
using Moq;
using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Models.Dto;
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
    }
}
