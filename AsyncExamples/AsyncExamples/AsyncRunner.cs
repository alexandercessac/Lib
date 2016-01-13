using System;

namespace AsyncExamples
{
    class AsyncRunner
    { 
        static void Main()
        {
            Console.WriteLine("Which example would you like to run?");
            Console.WriteLine("Please enter [1-9]\n");
            //var userInput = Console.ReadKey();

            //var myTask = new AsyncExample.DeadLockExample().DeadLockUnstartedTask();
            var myTask = new AsyncExample.DeadLockExample().DeadLock();
            //myTask.Wait();

            var x = myTask.Result;

            Console.WriteLine("## RESULT OF TASK ##");
            
            Console.WriteLine(x);

            Console.ReadKey();

        }
    }
}
