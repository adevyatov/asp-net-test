using AutoMapper;
using Moq;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApiTests.Service
{
    public class BookServiceMock
    {
        public Mock<IBookRepository> BookRepository { get; }

        public Mock<ILibraryCardRepository> LibraryCardRepository { get; }

        public Mock<IGenreRepository> GenreRepository { get; }

        public Mock<IMapper> Mapper { get; }

        public IBookService Service { get; }

        public BookServiceMock()
        {
            BookRepository = new Mock<IBookRepository>();
            LibraryCardRepository = new Mock<ILibraryCardRepository>();
            GenreRepository = new Mock<IGenreRepository>();
            Mapper = new Mock<IMapper>();

            Service = new BookService(
                BookRepository.Object,
                LibraryCardRepository.Object,
                GenreRepository.Object,
                Mapper.Object
            );
        }
    }
}
