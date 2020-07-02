using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ResourceWorkerJobsManager
{
    class Program
    {
        const int NUM_OF_WORKERS = 1000; // predefine the number of resource workers
        
        // API EndPoint
        public static void AddNewJob(int id, Action method, TimeSpan duration , bool highPriority)
        {
            var job = new WorkerJob(id, method, duration, highPriority);
            Task AddNewJobTask = Task.Run(() => QueueManager.AddWorkItem(job));
        }

        private static TimeSpan randomTime(int max)
        {
            Random rnd = new Random();
            return new TimeSpan(rnd.Next(max) * 10000); // 1 timespan tick = 0.0001 milliseconds
        }

        private static void PopulateWorkers(ResourceWorkerManager resourceWorkerManager)
        {
            Parallel.For(0, NUM_OF_WORKERS, (i) =>
            {
                resourceWorkerManager.AddWorker(i, randomTime(1000));
            });
        }

        private static void PopulateJobs()
        {
            Random rnd = new Random();
            int repetitions = rnd.Next(1000);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Adding {repetitions} new jobs....");
            Console.ForegroundColor = ConsoleColor.White;
            Parallel.For(0, repetitions, (i) => {
            Thread.Sleep(rnd.Next(1000));
            int id = rnd.Next(10000);
            TimeSpan ts = new TimeSpan(rnd.Next(1000) * 10000);
            bool priority = rnd.NextDouble() > 0.5;
            Action action = () => {
            var msg = ((priority) ? "High" : "Low") + $" priority job id {id} is done on {ts.TotalMilliseconds} ms";
                    Console.ForegroundColor = (priority) ? ConsoleColor.Green : ConsoleColor.DarkMagenta;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.White;
                };
                AddNewJob(id, action, ts, priority);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Adding job {id} to Queue");
                Console.ForegroundColor = ConsoleColor.White;
            });     
        }

        private static void UserActions()
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine("------- Actions Menu:  -------");
            Console.WriteLine("------------------------------");
            Console.WriteLine("g: (G)enerate random API calls");
            Console.WriteLine("x: (X)terminate service");
            Console.WriteLine("------------------------------");
            while (true)
            {
                var input = Console.ReadKey();
                Console.WriteLine("  <------ key pressed...");
                switch (input.KeyChar.ToString().ToLower())
                {
                    case "x":
                        QueueManager.SetCompleted();
                        break;
                    case "g":
                        Task.Run(PopulateJobs);
                        break;
                    default:
                        Console.WriteLine("Unsupported action key...");
                        break;
                }
            }
        }
        static void Main(string[] args)
        {
            // create & populate resource workers collection object
            ResourceWorkerManager resourceWorkerManager = new ResourceWorkerManager(NUM_OF_WORKERS);
            PopulateWorkers(resourceWorkerManager);

            // start user control interface
            Task.Run(() => UserActions());

            // start the job execution task
            resourceWorkerManager.ExecuteJobs();
        }
    }
}
