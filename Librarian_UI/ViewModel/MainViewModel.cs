using BookService;
using BookService.Model;
using Librarian_UI.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Librarian_UI.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties
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
        /// <summary>
        /// Property to help decide if the author points to a page
        /// </summary>
        public bool IsHyperLink
        {
            get
            {
                if (SelectedBook == null)
                    return false;
                else
                    return !String.IsNullOrEmpty(SelectedBook.Book?.authors?.FirstOrDefault().url);
            }
            set { isHyperLink = value; OnPropertyChanged(); }
        }

        private bool isLoading;

        public bool IsLoading
        {
            get { return isLoading; ; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        #endregion
        #region Commands
        private ICommand loadDetailsCommand;

        public ICommand LoadDetailsCommand
        {
            get
            {
                if (loadDetailsCommand == null)
                {
                    loadDetailsCommand = new DelegateCommand
                        (
                          async x => await LoadDetails(),
                          null
                        );
                }
                return loadDetailsCommand;
            }
            set { loadDetailsCommand = value; }
        }

        /// <summary>
        /// Loads the details of the book from the service into thier properties
        /// </summary>
        /// <returns>Runnable task</returns>
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
                            async x => await Search(x),
                            null
                        );
                }
                return searchCommand;
            }
            set { searchCommand = value; }
        }

        /// <summary>
        /// Searches the service for the occurence of the passed keyword in the choosen filter property of the book and loads the results
        /// </summary>
        /// <param name="x">Keyword through binding</param>
        /// <returns></returns>
        private async Task Search(object x)
        {
            //to avoid casting exceptions we use as 
            string query = x as string;

            //if the usre actually typed in a keyword
            if (!String.IsNullOrEmpty(query))
            {
                IsLoading = true;
                BookManager bm = new BookManager();
                SearchResult result = new SearchResult();

                try
                {
                    result = await bm.SearchAsync(Filter, query, ResultPage);
                }
                //Respond to the user in case of exception
                catch (Exception)
                {
                    ContentDialog noResulDialog = new ContentDialog
                    {
                        Title = "Uups",
                        Content = "Failed to reach the library. Check your internet connection or try again later",
                        CloseButtonText = "Ok"
                    };

                    await noResulDialog.ShowAsync();
                    IsLoading = false;
                    return;
                }

                Books.Clear();
                
                //empty resultset
                if (result.docs?.Count == 0)
                {
                    IsLoading = false;
                    ContentDialog noResulDialog = new ContentDialog
                    {
                        Title = "No results",
                        Content = "Try a different expression",
                        CloseButtonText = "Ok"
                    };

                    await noResulDialog.ShowAsync();
                    return;
                }
                //show only books with cover image for more spectacular UI
                foreach (var bookResult in result.docs.Where(d => d.cover_i != 0))
                    Books.Add(new BookItemViewModel(bookResult));

                SelectedBook = Books.FirstOrDefault();
                IsLoading = false;
            }
            else
            {
                ContentDialog noResulDialog = new ContentDialog
                {
                    Title = "No keyword",
                    Content = "Try filling in the searchbox first",
                    CloseButtonText = "Ok"
                };

                await noResulDialog.ShowAsync();
                SelectedBook = null;
            }
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
                            async x => await ShowLargeImage(),
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

        /// <summary>
        /// Evaluates if our selected book has a larger image to show or not
        /// </summary>
        /// <returns></returns>
        private bool LargeImageExist()
        {
            return !string.IsNullOrEmpty(SelectedBook.Book.cover.large);
        }

        /// <summary>
        /// Flips the flag that turns the enlarged image visible
        /// </summary>
        /// <returns>Runnable task</returns>
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
                            async x => await ChangeFilter(x),
                            null
                        );
                }
                return filterChange;
            }
            set { filterChange = value; }
        }

        /// <summary>
        /// Sets the filter that the books are searched based on
        /// </summary>
        /// <param name="param">The new filter value</param>
        /// <returns>Runnable task</returns>
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
                            async x => await OpenAuthorPageCommandMethod()
                        );
                }
                return openAuthorPageCommand;
            }
            set { openAuthorPageCommand = value; }
        }

        /// <summary>
        /// Starts a web browser opening the clicked authors page
        /// </summary>
        /// <returns>Runnable task</returns>
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
                        async x => await Paging(x),
                        null //canexcecute
                        );
                }
                return pagingCommand;
            }

            set { pagingCommand = value; }
        }

        /// <summary>
        /// Search the next/previous page of the results in a single method
        /// </summary>
        /// <param name="x">The direction of the paging {-1, 1}</param>
        /// <returns>Runnable task</returns>
        private async Task Paging(object x)
        {
            //avoid negativ pages and stay at parse error
            int direction = int.Parse((x as string) ?? "0");
            if (ResultPage + direction > 0)
            {
                ResultPage += direction;
                LargeImageShown = false;
                await Search(Keyword);
            }
        }
        #endregion
        public MainViewModel()
        {
            Books = new ObservableCollection<BookItemViewModel>();
            Filter = "title";
            Task.Run(() => LoadDataAsync());
        }

        /// <summary>
        /// Since async actions are permitted in constructors, this handles all Data loading
        /// </summary>
        /// <returns>Runnable task</returns>
        private async Task LoadDataAsync()
        {
            IsHyperLink = false;
            LargeImageShown = false;
            ResultPage = 1;
        }
    }
}
