using BookService;
using BookService.Model;
using System;
using System.Threading.Tasks;

namespace Librarian_UI.ViewModel
{
    public class BookItemViewModel : BaseViewModel
    {
        private Book book;

        public Book Book
        {
            get { return book; }
            set { book = value; OnPropertyChanged(); }
        }

        public Doc Doc { get; set; }

        public CaroItem CaroItem { get; set; }

        public BookItemViewModel(Book book)
        {
            Book = book;
        }

        public BookItemViewModel(Doc doc)
        {
            Book = ConvertToBook(doc);
            CaroItem = new CaroItem();
        }

        /// <summary>
        /// Converts an incomplete Book that has been returned from the API object into a Book with image
        /// </summary>
        /// <param name="doc">Incomplete book object</param>
        /// <returns>Converted book</returns>
        private Book ConvertToBook(Doc doc)
        {
            try
            {
                Book tmp = new Book { OLID_key = doc.cover_edition_key };
                tmp.cover.medium = "http://covers.openlibrary.org/b/id/" + doc.cover_i + "-M.jpg";
                tmp.title = doc.title;
                return tmp;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Loads the details of the selected book
        /// </summary>
        /// <returns>Detailed book</returns>
        public async Task LoadDetails()
        {
            BookManager bm = new BookManager();
            var keytmp = Book.OLID_key;
            Book = await bm.BookByOLID(Book.OLID_key);
            Book.OLID_key = keytmp;
        }
    }
}
