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

        public async Task<Book> BookByOLID(string OLID)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://openlibrary.org/");
                var response = await client.GetAsync($"api/books?bibkeys=OLID:{OLID}&jscmd=data&format=json"); // 0451526538
                string json = await response.Content.ReadAsStringAsync();
                int idx = json.IndexOf(':', json.IndexOf(':') + 1) + 1;
                string kesz = json.Substring(idx, (json.Length - idx - 1));
                Book konyv = JsonConvert.DeserializeObject<Book>(kesz);
                return konyv;
            }

            
            Debug.WriteLine("kesz");
            Console.WriteLine("gee");
            //Console.ReadLine();
        }

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
            //author helyett lehet meg title=,subject=,publisher=, es ezeket kombinalva & taggel
            
        }

        public async void GetCover(string cover)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddr;
                //TODO: ez nemi skell

            }
        }


    }
}
