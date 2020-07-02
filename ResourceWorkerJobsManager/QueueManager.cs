using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ResourceWorkerJobsManager
{
    class QueueManager
    {
       private static BlockingCollection<WorkerJob> highPriorityQueue, lowPriorityQueue;
       static QueueManager()
        {
            highPriorityQueue = new BlockingCollection<WorkerJob>();
            lowPriorityQueue = new BlockingCollection<WorkerJob>();
        }

        public static bool IsCompleted() => highPriorityQueue.IsCompleted && lowPriorityQueue.IsCompleted;
        public static void SetCompleted()
        {
            highPriorityQueue.CompleteAdding();
            lowPriorityQueue.CompleteAdding();
        }

        public static void AddWorkItem(WorkerJob workerItem)
        {
            bool success;
            do
            {
                success = (workerItem.HighPriority) ? highPriorityQueue.TryAdd(workerItem) : lowPriorityQueue.TryAdd(workerItem);
            } while (!success);
        }

        public static WorkerJob GetJobItem()
        {
            WorkerJob workerItem = null;
            bool success;
            do
            {
                if (highPriorityQueue.Count == 0 && lowPriorityQueue.Count == 0)  
                {
                    break; // in case of empty queues
                }
                success = (highPriorityQueue.Count > 0) ? // <-- always prefer the high priority job queue
                    highPriorityQueue.TryTake(out workerItem) : 
                    lowPriorityQueue.TryTake(out workerItem);

            } while (!success);
            return workerItem;
        }
    }
}
