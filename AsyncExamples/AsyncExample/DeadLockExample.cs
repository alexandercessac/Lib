using System;
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
       
    }
}

