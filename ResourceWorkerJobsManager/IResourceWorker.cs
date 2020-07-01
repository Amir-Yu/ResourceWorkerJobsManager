using System;
using System.Collections.Generic;
using System.Text;

namespace ResourceWorkerJobsManager
{
    interface IResourceWorker
    {
        void DoWork(Action action, TimeSpan duration);
        TimeSpan MaxHandleTime { get; }
    }
}
