using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _directory = ConfigurationManager.AppSettings["dataDirectory"];
        private readonly int _maxFileNumber = int.Parse(ConfigurationManager.AppSettings["fileNumber"]);
        private readonly bool _isLoadingIncluded = bool.Parse(ConfigurationManager.AppSettings["isLoadingIncluded"]);

        public async Task<string> Index(string fileName)
        {
            var filename = GetFileName(fileName);
            string receivedData;

            var path = Path.Combine(_directory, filename);
            using (var reader = new StreamReader(path))
            {
                receivedData = await reader.ReadToEndAsync();
            }

           string response;
            if (_isLoadingIncluded)
            {
                var tuple = await SortAsync(receivedData);
                response =
                    string.Format("file: {0},  length: {1}, time: {2}", filename,
                        tuple.Item1.Length, tuple.Item2);
            }
            else
            {
                response = string.Format("file: {0}; without loading", filename);
            }
            return response;
        }

        private string GetFileName(string fileName)
        {
            var file = string.IsNullOrEmpty(fileName) ? 1 : int.Parse(fileName) % _maxFileNumber;

            return string.Format("file{0}.txt", file.ToString().PadLeft(3, '0'));
        }

        private async Task<Tuple<string[], long>> SortAsync(string rawData)
        {
            return await Task.Factory.StartNew(() =>
            {
                var array = rawData.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var timer = new Stopwatch();
                timer.Start();
                Array.Sort(array);
                timer.Stop();

                return new Tuple<string[], long>(array, timer.ElapsedMilliseconds);
            });
        }
    }
}
