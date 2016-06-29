using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManager
{
    public static class MsgQ
    {
        private const int MAX_LISTNER_COUNT = 5;

        private static readonly System.Collections.Queue Q = new System.Collections.Queue();

        private static readonly object ListenerLock = new object();
        private static CancellationTokenSource _cts = new CancellationTokenSource();
        private static List<Task> _currentListeners = new List<Task>();

        public static Task AddToQueue<T>(List<T> queueItems) => Task.Run(() => Enqueue(queueItems));
        public static Task AddToQueue<T>(T queueItems) => Task.Run(() => Enqueue(queueItems));
        public static Task<List<T>> GetFromQueue<T>(int count = 1) => Task.Run(() => Dequeue<T>(count));

        public static bool AddQueueListener(Action<object> msgAction)
        {
            lock (_currentListeners)
            {//obtain exclusive access of the CurrentListeners to prevent list from being cleared before we add a new one
                if (_currentListeners.Count == MAX_LISTNER_COUNT) return false;
                _currentListeners.Add(Task.Run(() => CreateQueueListener(msgAction, _cts.Token)));
            }
            return true;
        }

        //public static void StopQueueListeners(int timeout = -1) => StopQueueListenersAsync().Wait(timeout);

        public static void StopQueueListeners()
        {
            lock (ListenerLock)
            {//obtain exclusive access of the CurrentListeners to prevent listeners from being added before we complete/clear the previous listeners
                _cts.Cancel();

                lock (Q) Monitor.PulseAll(Q); //Pulse all Q listeners

                Task.WhenAll(_currentListeners).Wait();

                _cts = new CancellationTokenSource();
                _currentListeners = new List<Task>();//remove all completed listeners
            }

        }

        private static void CreateQueueListener(Action<object> msgAction, CancellationToken ctx)
        {
            lock (Q)
            {
                while (!ctx.IsCancellationRequested)
                {
                    if (Q.Count == 0)
                        Monitor.Wait(Q);
                    try
                    { msgAction(Q.Dequeue()); }
                    catch (InvalidOperationException)
                    { Console.WriteLine($"[###Thread: {Thread.CurrentThread.ManagedThreadId}###] queue is empty"); }

                }
            }
        }

        private static void Enqueue<T>(IReadOnlyList<T> queueItems)
        {
            var itemsQueued = 0;

            while (itemsQueued < queueItems.Count)
            {
                if (!TryEnterQMonitor(3)) continue; //Could not obtain lock after 3 tries waiting 5 seconds each

                Q.Enqueue(queueItems[itemsQueued]);
                Monitor.Pulse(Q);
                Monitor.Exit(Q);
                itemsQueued++;
            }
        }

        private static bool Enqueue<T>(T item)
        {
            if (!TryEnterQMonitor(3)) return false; //Could not obtain lock after 3 tries waiting 5 seconds each

            Q.Enqueue(item);
            Monitor.Pulse(Q);
            Monitor.Exit(Q);
            return true;
        }

        private static List<T> Dequeue<T>(int count)
        {
            var ret = new List<T>();

            while (ret.Count < count)
            {
                if (!TryEnterQMonitor(3)) continue; //Could not obtain lock after 3 tries waiting 5 seconds each

                try
                {
                    ret.Add((T)Q.Dequeue());
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine($"[###Thread: {Thread.CurrentThread.ManagedThreadId}###] queue is empty");
                    count = 0;//don't try to get any more from empty queue
                }
                finally
                {
                    Monitor.Pulse(Q);
                    Monitor.Exit(Q);
                }

            }

            return ret;
        }

        private static bool TryEnterQMonitor(int retries = 3)
        {
            var retrycount = 0;
            do
                if (Monitor.TryEnter(Q, TimeSpan.FromSeconds(5)))
                    return true;
            while (++retrycount < retries);

            return false;
        }

        private static bool TryEnterListenerMonitor(int retries = 3)
        {
            var retrycount = 0;
            do
                if (Monitor.TryEnter(ListenerLock, TimeSpan.FromSeconds(5)))
                    return true;
            while (++retrycount < retries);

            return false;
        }
    }
}
