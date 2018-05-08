using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ConsoleTestProject
{
    public class Program
    {
        static void Main(string[] args)
        {

            using (var client = new HttpClient())
            {
                var prog = new Program();
                client.BaseAddress = new Uri("https://openlibrary.org/");
                
                prog.Faszom(client);
                //     var response = client.GetAsync("books?bibkeys=ISBN:0451526538&jscmd=data");
                //       var content = response.Result;
                //       var obj = JsonConvert.DeserializeObject<object>( content.Content.ReadAsStringAsync().Result);
                prog.Search(client, "patrick rothfuss");
                Debug.WriteLine("esz");
                Console.WriteLine("sssgee");
                Console.ReadLine();
            }

        }

        public async void Faszom(HttpClient client)
        {
            var response =await client.GetAsync("api/books?bibkeys=ISBN:0451526538&jscmd=data&format=json");
            string json = await response.Content.ReadAsStringAsync();
            int idx = json.IndexOf(':', json.IndexOf(':') + 1) + 1;
            string kesz = json.Substring(idx, (json.Length - idx - 1));
            Book konyv = JsonConvert.DeserializeObject<Book>(kesz);
            Debug.WriteLine("kesz");
            Console.WriteLine("gee");
//Console.ReadLine();
        }

        public async void Search(HttpClient client, string keyword)
        {
            //author helyett lehet meg title=,subject=,publisher=, es ezeket kombinalva & taggel
            var response = await client.GetAsync("search.json?author="+keyword+"&page=1");
            string json = await response.Content.ReadAsStringAsync();
            SearchResult sr = JsonConvert.DeserializeObject<SearchResult>(json);
            Console.WriteLine("search kesz");
        }

    }
}
