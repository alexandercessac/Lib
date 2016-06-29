using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManager
{
    public class Queue
    {

        private static System.Collections.Queue _q = new System.Collections.Queue();

        public Task AddToQueue<T>(List<T> queueItems) => Task.Run(() => Enqueue(queueItems));
        public Task<List<T>> GetFromQueue<T>(int count = 1) => Task.Run(() => Dequeue<T>(count));

        private static void Enqueue<T>(IReadOnlyList<T> queueItems)
        {
            var itemsQueued = 0;

            while (itemsQueued < queueItems.Count)
            {
                if (!TryEnterMonitor(3)) continue; //Could not obtain lock after 3 tries waiting 5 seconds each

                _q.Enqueue(queueItems[itemsQueued]);
                Monitor.Pulse(_q);
                Monitor.Exit(_q);
                itemsQueued++;
            }

        }

        private static List<T> Dequeue<T>(int count)
        {
            var ret = new List<T>();

            while (ret.Count < count)
            {
                if (!TryEnterMonitor(3)) continue; //Could not obtain lock after 3 tries waiting 5 seconds each

                try
                {
                    ret.Add((T)_q.Dequeue());
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine($"[###Thread: {Thread.CurrentThread.ManagedThreadId}###] queue is empty");
                    count = 0;//don't try to get any more from empty queue
                }
                finally
                {
                    Monitor.Pulse(_q);
                    Monitor.Exit(_q);
                }
                
            }

            return ret;
        }

        private static bool TryEnterMonitor(int retries = 3)
        {
            var retrycount = 0;
            do
                if (Monitor.TryEnter(_q, TimeSpan.FromSeconds(5)))
                    return true;
            while (++retrycount < retries);

            return false;
        }
    }
}
