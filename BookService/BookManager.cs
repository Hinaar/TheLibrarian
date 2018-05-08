using BookService.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BookService
{
    public class BookManager
    {
        private Uri baseAddr = new Uri("https://openlibrary.org/");

        /// <summary>
        /// Searches a book by its OLID number.
        /// </summary>
        /// <param name="OLID">OLID Identifier of the Book</param>
        /// <returns>Book details</returns>
        public async Task<Book> BookByOLID(string OLID)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("https://openlibrary.org/");
                    var response = await client.GetAsync($"api/books?bibkeys=OLID:{OLID}&jscmd=data&format=json");
                    string json = await response.Content.ReadAsStringAsync();

                    //format the result into a valid JSON form
                    int idx = json.IndexOf(':', json.IndexOf(':') + 1) + 1;
                    string kesz = json.Substring(idx, (json.Length - idx - 1));

                    Book konyv = JsonConvert.DeserializeObject<Book>(kesz);
                    return konyv;
                }
                catch (Exception)
                {
                    return new Book();
                }

            }
        }

        /// <summary>
        /// Searches for books matching the conditions in the online library
        /// </summary>
        /// <param name="filter">Property of the books which we are seaerching in</param>
        /// <param name="keyword">The keyword which we are looking for in the selected property</param>
        /// <param name="page">Used for pagination</param>
        /// <returns></returns>
        public async Task<SearchResult> SearchAsync(string filter,string keyword, int page)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddr;
                var response = await client.GetAsync($"search.json?{filter??"title"}={keyword}&page={page}");
                string json = await response.Content.ReadAsStringAsync();
                SearchResult sr = JsonConvert.DeserializeObject<SearchResult>(json);
                return sr;
            }
        }
    }
}
