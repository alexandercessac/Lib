using System;

namespace LyricDownloadConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var repeat = true;

            while (repeat)
            {
                Console.WriteLine("----------------------------------press 'y' to run application again upon completion----------------------------------");
                repeat = Console.ReadKey().KeyChar == 'y';
                Console.WriteLine();
                new DownloadWorker().DoWork();
            }

        }
      
    }
}
