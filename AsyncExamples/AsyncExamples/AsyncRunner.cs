using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QueueManager;

namespace AsyncExamples
{
    class AsyncRunner
    {
        private static readonly char[] ValidMenuChoices = { 'q', 'Q', '0', '1' };

        static void Main()
        {
            var quit = false;
            while (!quit)
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
                        quit = true;
                        break;

                }
            }
        }

        private static void DoMonitorQueueExample()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"[{DateTime.Now}] Queueing using Monitor");

            var q = new QueueManager.Queue();



            var itemsToQ = new List<int>();

            Console.WriteLine("How many numbers to add to queue? ");
            var readLine = Console.ReadLine();
            var numCount = long.Parse(readLine ?? "0");
            //add specified count of random single digit numbers
            var rnd = new Random();
            for (var i = 0; i < numCount; i++)
                itemsToQ.Add(rnd.Next(0, 9));

            Console.WriteLine($"\nQueueing {itemsToQ.Count} items");

            var startTime = DateTime.Now;
            var work = new[]
            {
                q.AddToQueue(itemsToQ),
                //q.AddToQueue(itemsToQ),
                //q.AddToQueue(itemsToQ),
                WriteToConsoleFromQ(q),
                WriteToConsoleFromQ(q),
                WriteToConsoleFromQ(q),
                WriteToConsoleFromQ(q),
                WriteToConsoleFromQ(q)
            };
            //added a bunch to the queue, lets pull it out



            Task.WaitAll(work);

            Console.WriteLine($"Execution time took: [{DateTime.Now.Subtract(startTime).TotalSeconds} seconds] to enqueue and dequeue {numCount} items.");
            Console.WriteLine("--------------------------------------------------------------------------");
            Console.WriteLine();

        }

        private static async Task WriteToConsoleFromQ(Queue q)
        {
            var dQ = new List<int>(); string val;

            do
            {
                dQ.Clear(); val = ""; 
                dQ = await q.GetFromQueue<int>(25);

                if (dQ.Count <= 0) break;

                dQ.ForEach(x => val += $"{x}, ");
                Console.WriteLine(val.Trim().Trim(','));
                

            } while (dQ.Count > 0);

            if (!string.IsNullOrEmpty(val))
            {
                Console.WriteLine(val.Trim().Trim(','));
                Console.WriteLine();
            }

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
                Console.WriteLine("             ------------------------------Main Menu--------------------------------------");
                Console.WriteLine("             |                Which example would you like to run?                       |");
                Console.WriteLine("             |                [0] : Deadlock Example                                     |");
                Console.WriteLine("             |                [1] : MonitorExample                                       |");
                Console.WriteLine("             |                Please enter [1-9] or [q] to quit                          |");
                Console.WriteLine("             -----------------------------------------------------------------------------");
                userInput = Console.ReadKey().KeyChar;

            } while (!ValidMenuChoices.Contains(userInput));

            return userInput;

        }
    }
}
