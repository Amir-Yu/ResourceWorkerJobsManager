using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ResourceWorkerJobsManager
{
    class ResourceWorker : IResourceWorker
    {
        private int id;
        private TimeSpan maxHandleTime;
        
        public TimeSpan MaxHandleTime => maxHandleTime;

        public ResourceWorker(int id, TimeSpan maxHandleTime)

        {
            this.id = id;
            this.maxHandleTime = maxHandleTime;
        }
        public void DoWork(Action action, TimeSpan duration)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(duration);
                action();
            });
        }
    }
}
