using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace httpClient
{
    public class Client
    {
        private readonly string _baseUrl;
        private readonly int _tasks;
        private readonly ConcurrentQueue<string> _result;

        public Client(string baseUrl, int tasks)
        {
            _baseUrl = baseUrl;
            _tasks = tasks;
            _result = new ConcurrentQueue<string>();
        }

        public void Start()
        {
            Console.WriteLine();
            Console.WriteLine("Start sending");

            var timer = new Stopwatch();
            timer.Start();
            var tasks = new Task[_tasks];
            for (int i = 0; i < _tasks; ++i)
            {
                tasks[i] = Perform(i);
            }
            Task.WaitAll(tasks, -1);
            timer.Stop();

            //_result.ToList().ForEach(Console.WriteLine);
            Console.WriteLine();
            Console.WriteLine("tasks: {0} time: {1}", _tasks, timer.ElapsedMilliseconds);
        }

        private async Task Perform(int state)
        {
            string url = String.Format("{0}{1}", _baseUrl, state.ToString().PadLeft(3, '0'));
            var client = new HttpClient();
            var timer = new Stopwatch();

            timer.Start();
            var stringResult = await client.GetStringAsync(url);
            timer.Stop();

            _result.Enqueue(string.Format("{0,4}\t{1,5}\t{2}", url, timer.ElapsedMilliseconds, stringResult));
        }
    }

    static class Program
    {
        public static void Main(string[] args)
        {
            var address = "http://localhost:8077/Home/Index/";

            for (var i = 100; i <= 500; i+=100)
            {
                var client = new Client(address, i);
                client.Start();
            }
            
            Console.ReadLine();
        }
    }
}
