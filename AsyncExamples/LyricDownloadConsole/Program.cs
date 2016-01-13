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
                new DownloadWorker().DoWork();
                Console.WriteLine("press 'y' to run application again.");
                repeat = Console.ReadKey().KeyChar == 'y';
                Console.WriteLine();
            }

        }
      
    }
}
