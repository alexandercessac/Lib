using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AsyncExample;
using QueueManager;

namespace AsyncExamples
{
    class AsyncRunner
    {
        private static readonly char[] ValidMenuChoices = { 'q', 'Q', '0', '1', '2', '3' };

        static void Main()
        {
            var quit = false;
            while (!quit)
            {
                var x = MainMenu();

                switch (x)
                {
                    case '0': DoDeadLockExample(); break;
                    case '1': DoMonitorQueueExample(); break;
                    case '2': DoMonitorQueueListenerExample(); break;
                    case '3': RunTest(); break;
                    default:
                        quit = true;
                        break;

                }
            }
        }

        private static void RunTest()
        {
            var path = "C:/code/enterprise-apps/Component/AlderOrderProcessorConsumer/bin/Debug/AlderOrderProcessorConsumer.log";
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
            {
                
            }
        }

        private static void DoMonitorQueueListenerExample()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"[{DateTime.Now}]------------Queueing using Monitor Listeners----------\n");

            MsgQ.OnReport += Console.WriteLine;
            MsgQ.AddQueueListener(GetVowelsFromString);

            do
            {
                var input = WriteLineWithResponse("Enter text [Q to quit]:\n");
                if (input == "Q") break;
                MsgQ.AddToQueue(new List<string> { input });
            } while (true);

            MsgQ.StopQueueListeners();//cleanup
            MsgQ.OnReport -= Console.WriteLine;
        }

        private static readonly Action<object> GetVowelsFromString = msg =>
        {
            try
            {
                var tmp = (string)msg;

                var cap = new Regex("a|e|i|o|u", RegexOptions.IgnoreCase).Match(tmp).Captures;

                var ret = new string[cap.Count];

                cap.CopyTo(ret, 0);

                Console.WriteLine($"string {tmp} contains the following vowels:\n {string.Join(", ", ret).Trim().Trim(',')}\n\n");

            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not cast message to a string: {e.Message}");
            }

            //write all vowels
        };

        private static void DoMonitorQueueExample()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"[{DateTime.Now}] Queueing using Monitor");


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
                MsgQ.AddToQueue(itemsToQ),
                //q.AddToQueue(itemsToQ),
                //q.AddToQueue(itemsToQ),
                WriteToConsoleFromQ(),
                WriteToConsoleFromQ(),
                WriteToConsoleFromQ(),
                WriteToConsoleFromQ(),
                WriteToConsoleFromQ()
            };
            //added a bunch to the queue, lets pull it out



            Task.WaitAll(work);

            Console.WriteLine($"Execution time took: [{DateTime.Now.Subtract(startTime).TotalSeconds} seconds] to enqueue and dequeue {numCount} items.");
            Console.WriteLine("--------------------------------------------------------------------------");
            Console.WriteLine();

        }

        private static async Task WriteToConsoleFromQ()
        {
            //StringHelper.WriteListToConsole(async () =>
            //{
            // return await MsgQ.GetFromQueue<int>(25);
            //});

            var dQ = new List<int>(); string val;

            do
            {
                dQ.Clear(); val = "";
                dQ = await MsgQ.GetFromQueue<int>(25);

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
            DeadLockExample.GetResource();
            ////var myTask = new AsyncExample.DeadLockExample().DeadLockUnstartedTask();
            //var myTask = new AsyncExample.DeadLockExample().DeadLock();
            ////myTask.Wait();

            //var x = myTask.Result;

            //Console.WriteLine("## RESULT OF TASK ##");

            //Console.WriteLine(x);

            Console.ReadKey();
        }

        #region UI
        
        private static char MainMenu()
        {
            char userInput;

            do
            {
                Console.WriteLine("             ------------------------------Main Menu--------------------------------------");
                Console.WriteLine("             |                Which example would you like to run?                       |");
                Console.WriteLine("             |                [0] : Deadlock Example                                     |");
                Console.WriteLine("             |                [1] : Monitor Example                                      |");
                Console.WriteLine("             |                [2] : Monitor Listener Example                             |");
                Console.WriteLine("             |                [3] : Do a test thing                                      |");
                Console.WriteLine("             |                Please enter [1-9] or [q] to quit                          |");
                Console.WriteLine("             -----------------------------------------------------------------------------");
                userInput = Console.ReadKey().KeyChar;

            } while (!ValidMenuChoices.Contains(userInput));

            return userInput;

        }

        private static string WriteLineWithResponse(string msg)
        {
            Console.WriteLine(msg);
            return Console.ReadLine();
        }

        #endregion
    }
}
