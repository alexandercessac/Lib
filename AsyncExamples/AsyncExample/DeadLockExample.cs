using System;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncExample
{
    public class DeadLockExample
    {

        public async Task<string> DeadLockUnstartedTask()
        {
            var i = 0;

            Console.WriteLine("Calling GetOneAsync");
            i += await HelperFunctions.GetOneAsync();
            Console.WriteLine("i = {0}", i);
            Console.WriteLine("Calling GetOneAsyncTaskFactory");
            i += await HelperFunctions.GetOneTaskFactory();
            Console.WriteLine("i = {0}", i);
            Console.WriteLine("Calling GetOneAsyncTask");
            i += await HelperFunctions.GetOneTask();
            Console.WriteLine("i = {0} you will never get this..", i);
            return i.ToString();
        }

        public async Task<string> DeadLock()
        {
            var i = 0;

            Console.WriteLine("Calling GetOneAsync");
            i += await HelperFunctions.GetOneAsync();
            Console.WriteLine("i = {0}", i);
            i += HelperFunctions.GetOneAsync().Result;

            Console.WriteLine("i = {0} you will never get in UI app..", i);
            return i.ToString();
        }
        
        private static void HandleResponse(object x) { }

        public static object GetResource()
        {
            var tmp = new Task(() => GetResourceAsync());
            
            var resource = Task.Run(GetResourceAsync).Result;

            HandleResponse(resource);

            return resource;
        }

        public static async Task<object> GetResourceAsync()
        {
            var resource = await LoadResourceAsync().ConfigureAwait(true);

            HandleResponse(resource);

            return resource;
        }
        
        public static Task<object> LoadResourceAsync() => Task.Run(() =>
        {
            Thread.Sleep(3000);
            return new object();
        });
    }
}

