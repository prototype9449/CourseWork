﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace httpServer
{
    public class Server
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly ASCIIEncoding _encoding = new ASCIIEncoding();
        private readonly string _directory;
        private readonly int _maxFileNumber;
        private readonly bool _isLoadingIncluded;

        public Server(string address, string directory, int maxFileNumber, bool isLoadingIncluded)
        {
            _directory = directory;
            _maxFileNumber = maxFileNumber;
            _isLoadingIncluded = isLoadingIncluded;

            _listener.Prefixes.Add(address);
            _listener.Start();
        }

        public async Task Start()
        {
            while (true)
            {
                var context = await _listener.GetContextAsync();
                ProcessRequest(context);
            }
        }

        private string GetFileName(string url)
        {
            var file = string.IsNullOrEmpty(url) ? 1 : int.Parse(url) % _maxFileNumber;

            return string.Format("file{0}.txt", file.ToString().PadLeft(3, '0'));
        }

        private async void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                var filename = GetFileName(context.Request.Url.PathAndQuery.Substring(1));
                string receivedData;

                var path = Path.Combine(_directory, filename);
                using (var reader = new StreamReader(path))
                {
                    receivedData = await reader.ReadToEndAsync();
                }

                byte[] response;
                if (_isLoadingIncluded)
                {
                    var tuple = await SortAsync(receivedData);
                    response =
                        _encoding.GetBytes(string.Format("file: {0},  length: {1}, time: {2}", filename,
                            tuple.Item1.Length, tuple.Item2));
                }
                else
                {
                    response = _encoding.GetBytes(string.Format("file: {0}; without loading", filename));
                }

                await context.Response.OutputStream.WriteAsync(response, 0, response.Length);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Console.WriteLine(e.Message);
            }
            finally
            {
                context.Response.Close();
            }
        }

        private async Task<Tuple<string[], long>> SortAsync(string rawData)
        {
            return await Task.Factory.StartNew(() =>
            {
                var array = rawData.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var timer = new Stopwatch();
                timer.Start();
                QuickSort(array,0, array.Length - 1);
                timer.Stop();

                return new Tuple<string[], long>(array, timer.ElapsedMilliseconds);
            });
        }

        static void QuickSort(string[] array, int left, int right)
        {
            string temp;
            string x = array[left + (right - left) / 2];
            int i = left;
            int j = right;
            while (i <= j)
            {
                while (array[i].CompareTo(x) == -1) i++;
                while (array[j].CompareTo(x) == 1) j--;
                if (i <= j)
                {
                    temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                    i++;
                    j--;
                }
            }
            if (i < right)
                QuickSort(array, i, right);

            if (left < j)
                QuickSort(array, left, j);
        }
    }
    static class Program
    {
        public static void Main(string[] args)
        {
            string address = args[0];
            int maxFileNumber = int.Parse(args[1]);
            bool isLoadingIncluded = bool.Parse(args[2]);
            string directory = args[3];
            Console.WriteLine("http-сервер запущен по адресу {0}", address);
            Console.WriteLine("Сервер ожидает подключений...");
            var program = new Server(address, directory, maxFileNumber, isLoadingIncluded);
            
            program.Start().Wait();
        }
    }
}
