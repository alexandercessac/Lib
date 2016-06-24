using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManager
{
    public class Queue
    {
        internal static System.Collections.Queue MSmplQueue;

        public Task AddToQueue<T>(List<T> queueItems) => Task.Run(() => Enqueue(queueItems));
        public Task<List<T>> GetFromQueue<T>(int count = 1) => Task.Run(() => Dequeue<T>(count));

        private static void Enqueue<T>(IReadOnlyList<T> queueItems)
        {
            var itemsQueued = 0;
            Monitor.TryEnter(MSmplQueue);
            while (itemsQueued < queueItems.Count)
            {
                if (!Monitor.IsEntered(MSmplQueue))
                    Monitor.Wait(MSmplQueue);

                MSmplQueue.Enqueue(queueItems[itemsQueued]);
                Monitor.Pulse(MSmplQueue);

                itemsQueued++;
            }
            Monitor.Exit(MSmplQueue);
        }

        private static List<T> Dequeue<T>(int count)
        {
            var ret = new List<T>();

            while (Monitor.Wait(MSmplQueue, 3000) || ret.Count < count)
            {
                ret.Add((T)MSmplQueue.Dequeue());
                Monitor.Pulse(MSmplQueue);
            }

            return ret;
        }
    }
}
