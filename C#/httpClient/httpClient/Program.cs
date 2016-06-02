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
            var timer = new Stopwatch();
            timer.Start();
            var tasks = new Task[_tasks];
            for (int i = 0; i < _tasks; ++i)
            {
                tasks[i] = Perform(i);
            }
            try
            {
                Task.WaitAll(tasks, -1);
            }
            catch (AggregateException exception)
            {
                throw exception;
            }
            timer.Stop();
            
            Console.WriteLine("файлов обработано: {0}, время работы: {1} ms", _tasks, timer.ElapsedMilliseconds);
        }

        private async Task Perform(int state)
        {
            string url = String.Format("{0}{1}", _baseUrl, state.ToString().PadLeft(3, '0'));
            var client = new HttpClient();
            var timer = new Stopwatch();
            var shouldRecconect = true;

            string stringResult = "";
            while (shouldRecconect)
            {
                try
                {
                    timer.Start();
                    stringResult = await client.GetStringAsync(url);
                    timer.Stop();
                    shouldRecconect = false;
                }
                catch (Exception ex)
                {
                    shouldRecconect = true;
                }
            }

            _result.Enqueue(string.Format("{0,4}\t{1,5}\t{2}", url, timer.ElapsedMilliseconds, stringResult));
        }
    }

    static class Program
    {
        public static void Main(string[] args)
        {
            var address = args[0];
            Console.WriteLine("Адрес для отправки сообщений: {0}", address);
            Console.WriteLine();

            for (var i = 100; i <= 500; i+=100)
            {
                Console.WriteLine("Количество сообщений: {0}", i);
                var client = new Client(address, i);
                client.Start();
                Console.WriteLine();
            }
            
            Console.ReadLine();
        }
    }
}
