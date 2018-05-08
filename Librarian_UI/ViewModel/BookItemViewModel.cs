using BookService;
using BookService.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        private Book ConvertToBook(Doc doc)
        {
            try
            {
                Book tmp = new Book();
                tmp.OLID_key = doc.cover_edition_key;
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

        public BookItemViewModel(CaroItem it)
        {
            CaroItem = it;
        }

        public async Task LoadDetails()
        {
            BookManager bm = new BookManager();
            var keytmp = Book.OLID_key;
            Book = await bm.BookByOLID(Book.OLID_key);
            Book.OLID_key = keytmp;
        }
    }
}
