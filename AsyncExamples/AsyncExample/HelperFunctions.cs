using System;
using System.Threading.Tasks;

namespace AsyncExample
{
    class HelperFunctions
    {
        public static async Task<int> GetOneAsync()
        {
            Console.WriteLine("--Getting one...");
            await Task.Delay(2000);
            Console.WriteLine("--Got one...");
            return 1;
        }

        public static Task<int> GetOneTaskFactory()
        {
            return new TaskFactory<int>().StartNew(() =>
            {
                Console.WriteLine("--Getting one...");
                Task.Delay(2000).Wait();
                Console.WriteLine("--Got one...");
                return 1;
            });
        }

        public static Task<int> GetOneTask()
        {
            return new Task<int>(() =>
            {
                Console.WriteLine("--Getting one...");
                Task.Delay(2000).Wait();
                Console.WriteLine("--Got one...");
                return 1;
            });
        }
    }
}
