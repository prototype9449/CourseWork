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
            Task.WaitAll(tasks);
            timer.Stop();

            _result.ToList().ForEach(Console.WriteLine);
            Console.WriteLine();
            Console.WriteLine("total: {0}",timer.ElapsedMilliseconds);
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
            var address = args[0];
            int fileNumber = int.Parse(args[1]);
            var client = new Client(address, fileNumber);
            client.Start();
            Console.ReadLine();
        }
    }
}
