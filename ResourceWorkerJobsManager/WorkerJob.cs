using System;
using System.Collections.Generic;
using System.Text;

namespace ResourceWorkerJobsManager
{
    class WorkerJob
    {
        public int Id { get; set; }
        public Action Method { get; set; }
        public TimeSpan Duration { get; set; }
        public bool HighPriority { get; set; }

        public WorkerJob(int id, Action method, TimeSpan duration, bool highPriority)
        {
            this.Id = id;
            this.Method = method;
            this.Duration = duration;
            this.HighPriority = highPriority;
        }
    }
}
