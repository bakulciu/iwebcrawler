using System;
using System.Collections.Generic;
using System.Text;
using CrawlerNameSpace.Utilities;
using System.Threading;

namespace CrawlerNameSpace.Utilities
{
    /**
     * This class supply methods which can access the shared resources in thread safe
     * way
     */
    public class SyncAccessor
    {
        /**
         * puts the elemnt in the queue, this method is thread safe so it can be invoked 
         *  via more than one thread which want access to the shared resource
         */
        static public void putInQueue<T>(Queue<T> queue, T elemnt)
        {
            lock (queue)
            {
                queue.Enqueue(elemnt);
            }
        }

        /**
         * puts the elemnt in the queue, this method is thread safe so it can be invoked 
         *  via more than one thread which want access to the shared resource
         */
        static public int queueSize<T>(Queue<T> queue)
        {
            lock (queue)
            {
                return queue.Count;
            }
        }

        /**
         * returns the elemnt in the queue, this method is thread safe so it can be invoked 
         *  via more than one thread which want access to the shared resource
         * NOTE: if the shared resource is empty it will wait the time and retry.
         */
        static public T getFromQueue<T>(Queue<T> queue, int time)
        {
            bool toSleep = false;
            while (true)
            {
                T elemnt;
                Random randomizer = new Random();
                if (toSleep) Thread.Sleep(time + randomizer.Next(time / 2));
                toSleep = false;

                lock (queue)
                {
                    if (queue.Count == 0)
                    {
                        toSleep = true;
                        continue;
                    }
                    elemnt = queue.Dequeue();
                }
                return elemnt;
            }
        }
    }
}
