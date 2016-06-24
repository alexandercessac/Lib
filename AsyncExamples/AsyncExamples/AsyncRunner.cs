using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncExamples
{
    class AsyncRunner
    {
        private static readonly char[] ValidMenuChoices = { 'q', 'Q', '0', '1' };

        static void Main()
        {
            while (true)
            {
                var x = MainMenu();

                switch (x)
                {
                    case '0':
                        DoDeadLockExample();
                        break;
                    case '1':
                        DoMonitorQueueExample();
                        break;
                    default:
                        break;
                }
            }
        }

        private static void DoMonitorQueueExample()
        {
            var Q = new QueueManager.Queue();


            var itemsToQ = new List<int>() { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 };

            var work = new[]
            {
                Q.AddToQueue(itemsToQ),
                Q.AddToQueue(itemsToQ),
                Q.AddToQueue(itemsToQ)
            };
            //added a bunch to the queue, lets pull it out

            List<int> dQ;
            do
            {
                dQ = Q.GetFromQueue<int>().Result;
                if (dQ.Count > 0)
                    dQ.ForEach(Console.WriteLine);
                else
                    break;
            } while (dQ.Count > 0);

            Console.WriteLine("Done");

        }

        private static void DoDeadLockExample()
        {
            //var myTask = new AsyncExample.DeadLockExample().DeadLockUnstartedTask();
            var myTask = new AsyncExample.DeadLockExample().DeadLock();
            //myTask.Wait();

            var x = myTask.Result;

            Console.WriteLine("## RESULT OF TASK ##");

            Console.WriteLine(x);

            Console.ReadKey();
        }

        private static char MainMenu()
        {
            char userInput;

            do
            {
                Console.WriteLine("Which example would you like to run?");
                Console.WriteLine("[0] : Deadlock Example");
                Console.WriteLine("[1] : MonitorExample");
                Console.WriteLine("Please enter [1-9] or [q] to quit\n");
                userInput = Console.ReadKey().KeyChar;

            } while (!ValidMenuChoices.Contains(userInput));

            return userInput;

        }
    }
}
