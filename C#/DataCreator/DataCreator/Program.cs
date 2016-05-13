using System;
using System.IO;
using System.Text;

namespace DataCreator
{
    public class DataCreator
    {
        public void CreateFiles(int fileNumber, string directory)
        {
            for (int i = 0; i < fileNumber; ++i)
            {
                byte[] data = Generate();
                var fileName = string.Format("file{0}.txt", i.ToString().PadLeft(3, '0'));
                var path = Path.Combine(directory, fileName);
                using (var stream = File.Open(path, FileMode.OpenOrCreate))
                {
                    stream.Write(data, 0, data.Length);
                }
            }
        }

        private byte[] Generate()
        {
            var encoding = new ASCIIEncoding();
            var random = new Random((int)(DateTime.UtcNow.Ticks % Int32.MaxValue) + 1);
            var data = new StringBuilder();

            for (long i = 0; i < 30000; ++i)
            {
                data.AppendLine(random.NextDouble().ToString("F7"));
            }
            data.Append(random.NextDouble().ToString("F7"));

            return encoding.GetBytes(data.ToString());
        }
    }

    public static class Program
    {
        static void Main(string[] args)
        {
            int fileNumber = Int32.Parse(args[0]);
            string directory = args[1];
            Console.WriteLine("You want to create {0} files in {1}", fileNumber, directory);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
            Console.WriteLine();
            Console.WriteLine("Creating");
            new DataCreator().CreateFiles(fileNumber, directory);
            Console.WriteLine("All files were created. Press any key to exit");
            Console.ReadKey(true);
        }
    }
}
