using BookService;
using Librarian_UI.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;

namespace Librarian_UI.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private string filter;
        public string Filter
        {
            get { return filter; }
            set { filter = value; ResultPage = 1; OnPropertyChanged(); }
        }

        private string keyword;

        public string Keyword
        {
            get { return keyword; }
            set { keyword = value; ResultPage = 1; OnPropertyChanged(); }
        }

        public int ResultPage { get; set; }

        private ObservableCollection<BookItemViewModel> books;
        public ObservableCollection<BookItemViewModel> Books { get { return books; } set { books = value; OnPropertyChanged(); } }
        private BookItemViewModel selectedBook;

        public BookItemViewModel SelectedBook
        {
            get { return selectedBook; }
            set { selectedBook = value; OnPropertyChanged(); }
        }


        //TODO: remove items
        private ObservableCollection<CaroItem> items;
        public ObservableCollection<CaroItem> Items { get { return items; } set { items = value; OnPropertyChanged(); } }

        private bool largeImageShown;

        public bool LargeImageShown
        {
            get { return largeImageShown; }
            set
            {
                largeImageShown = value;
                OnPropertyChanged();
            }
        }

        private bool isHyperLink;
        public bool IsHyperLink
        {
            get
            {
                if (SelectedBook == null)
                    return false;
                else
                    return !String.IsNullOrEmpty(SelectedBook.Book.authors.First().url);
            }
            set { isHyperLink = value; OnPropertyChanged(); }
        }

        private bool isLoading;

        public bool IsLoading
        {
            get { return isLoading;; }
            set { isLoading = value; OnPropertyChanged(); }
        }
        private ICommand loadDetailsCommand;

        public ICommand LoadDetailsCommand
        {
            get
            {
                if (loadDetailsCommand == null)
                {
                    loadDetailsCommand = new DelegateCommand
                        (
                          x => LoadDetails(),
                          null
                        );
                }
                return loadDetailsCommand;
            }
            set { loadDetailsCommand = value; }
        }

        private async Task LoadDetails()
        {
                IsLoading = true;
                await SelectedBook.LoadDetails();
                IsHyperLink = IsHyperLink;
                IsLoading = false;
        }

        private ICommand searchCommand;

        public ICommand SearchCommand
        {
            get
            {
                if (searchCommand == null)
                {
                    searchCommand = new DelegateCommand
                        (
                            x => Search(x),
                            null
                        );
                }
                return searchCommand;
            }
            set { searchCommand = value; }
        }

        private async Task Search(object x)
        {
            IsLoading = true;
            string query = x as string;
            if (!String.IsNullOrEmpty(query))
            {
                BookManager bm = new BookManager();
                var result = await bm.SearchAsync(Filter, query, ResultPage);
                if (result.docs.Count == 0)
                {
                    //nincs eredmeny
                    IsLoading = false;
                    return;
                }
                Books.Clear();
                foreach (var bookResult in result.docs.Where(d=>d.cover_i !=0))
                {
                        Books.Add(new BookItemViewModel(bookResult));
                }
            }
            SelectedBook = Books.First();
            IsLoading = false;
        }

        private ICommand showLargeImageCommand;
        /// <summary>
        /// Command property for enlarging cover image
        /// </summary>
        public ICommand ShowLargeImageCommand
        {
            get
            {
                if (showLargeImageCommand == null)
                {
                    showLargeImageCommand = new DelegateCommand
                        (
                            x => ShowLargeImage(),
                            x => LargeImageExist()
                        );
                }
                return showLargeImageCommand;
            }
            set
            {
                showLargeImageCommand = value;
            }
        }

        private bool LargeImageExist()
        {
            return !string.IsNullOrEmpty(SelectedBook.Book.cover.large);
        }

        /// <summary>
        /// Async method that changes the enlarged cover visibility
        /// </summary>
        /// <returns></returns>
        private async Task ShowLargeImage()
        {
            LargeImageShown = !LargeImageShown;
        }

        private ICommand filterChange;

        public ICommand FilterChange
        {
            get
            {
                if (filterChange == null)
                {
                    filterChange = new DelegateCommand
                        (
                            x => ChangeFilter(x),
                            null
                        );
                }
                return filterChange;
            }
            set { filterChange = value; }
        }

        private async Task ChangeFilter(object param)
        {
            Filter = param as string;
        }

        private ICommand openAuthorPageCommand;

        public ICommand OpenAuthorPageCommand
        {
            get
            {
                if (openAuthorPageCommand == null)
                {
                    openAuthorPageCommand = new DelegateCommand
                        (
                            x => OpenAuthorPageCommandMethod(),
                            null
                        );
                }
                return openAuthorPageCommand;
            }
            set { openAuthorPageCommand = value; }
        }

        private async Task OpenAuthorPageCommandMethod()
        {
                await Launcher.LaunchUriAsync(new Uri(SelectedBook.Book.authors[0].url));
        }

        private ICommand pagingCommand;

        public ICommand PagingCommand
        {
            get
            {
                if (pagingCommand == null)
                {
                    pagingCommand = new DelegateCommand
                        (
                        x => Paging(x),
                        null //ide a canlogin
                        );
                }
                return pagingCommand;
            }

            set { pagingCommand = value; }
        }

        private async Task Paging(object x)
        {
            int direction = int.Parse((x as string)??"0");
            if (ResultPage + direction > 0)
            {
                ResultPage += direction;
                await Search(Keyword);
            }
        }

        public MainViewModel()
        {
            Items = new ObservableCollection<CaroItem>();
            Books = new ObservableCollection<BookItemViewModel>();
            loadDataAsync();
        }



        private async void loadDataAsync()
        {
            IsHyperLink = false;
            LargeImageShown = false;
            ResultPage = 1;
        }
    }
}
