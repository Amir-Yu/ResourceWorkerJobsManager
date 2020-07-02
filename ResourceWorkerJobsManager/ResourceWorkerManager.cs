﻿using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceWorkerJobsManager
{
    class ResourceWorkerManager
    {
        private object objSyncer = new object();
        private int workersCapacity;
        private ConcurrentDictionary<int, ResourceWorker> workers;
        private ConcurrentDictionary<int, double> workersMaxHandleTime;
        
        public ResourceWorkerManager(int workersCapacity = 100)
        {
            this.workersCapacity = workersCapacity;
            workers = new ConcurrentDictionary<int, ResourceWorker>(1, workersCapacity);
            workersMaxHandleTime = new ConcurrentDictionary<int, double>(1, workersCapacity);
        }

        public bool AddWorker(int id, TimeSpan maxHandleTime)
        {
            bool result = false;
            if (workers.Count < workersCapacity)
            {
                lock (objSyncer)
                {
                    result = workers.TryAdd(id, new ResourceWorker(id, maxHandleTime)) &&
                        workersMaxHandleTime.TryAdd(id, maxHandleTime.TotalMilliseconds);
                }
            }
            var msg = result ? $"worker id: {id} created, maxHandleTime: {maxHandleTime.TotalMilliseconds} ms" : $"worker {id} not created";
            Console.WriteLine(msg);
            return result;
        }

        public bool RemoveWorker(int id)
        {
            lock (objSyncer)
            {
                return workers.TryRemove(id, out _) && workersMaxHandleTime.TryRemove(id, out _);
            }
        }

        public void ExecuteJobs()
        {
            while (!QueueManager.IsCompleted()) // as long as getting work items
            {
                WorkerJob workerJob = QueueManager.GetJobItem();
                if (workerJob != null)
                {
                    var duration = workerJob.Duration.TotalMilliseconds;
                    var closestWorkerId = workersMaxHandleTime
                        .Select(kvp => new { kvp, distance = Math.Abs(kvp.Value - duration) })
                        .OrderBy(o => o.distance).First().kvp.Key;
                    Task.Run(() => workers[closestWorkerId].DoWork(workerJob.Method, workerJob.Duration));
                }
            }
        }
    }
}
