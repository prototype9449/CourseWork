﻿using System;
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
                QuickSort(array, 0, array.Length -1);
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
}
