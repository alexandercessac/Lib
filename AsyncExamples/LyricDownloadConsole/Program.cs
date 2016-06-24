using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LyricDownloadConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var repeat = true;

            //while (repeat)
            //{
            //    Console.WriteLine("----------------------------------press 'y' to run application again upon completion----------------------------------");
            //    repeat = Console.ReadKey().KeyChar == 'y';
            //    Console.WriteLine();
            //    new DownloadWorker().DoWork();
            //}


            var myList = new List<int?>
            {
                null,
                1,
                2,
                null,
                3,
                4,
                null
            };


            var filtered = myList.Where(maybeNull => maybeNull.HasValue)
                .Select(hasVal => hasVal.GetValueOrDefault(0)).ToList();

            filtered.ForEach(Console.WriteLine);

            Console.ReadLine();
        }

       
    }
}
