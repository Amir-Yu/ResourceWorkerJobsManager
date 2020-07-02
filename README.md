The “Resource worker Jobs” Manager

The goal of this project is to implement automatic resource worker jobs manager.
Please use C# .net language. 
To make it simple, the solution can be run under .net Console application. 

Below are the requirements of the system:

The main goal is to develop a system that manages incoming jobs and assigns them to workers.
Each worker in the system is a small unit that is responsible to execute job action for a specific duration.
Please pay attention to performance and scalability.
  

Requirements :
The resource worker is responsible to do some work for a given job for a specific time (duration). 
Each resource worker needs to implement the following Interface:   
                DoWork (Action a,  JobDuration t)

Each worker has a property (MaxHandleTime) which indicates the maximum time it can handle a job. Worker can’t handle jobs with duration larger than its MaxHandleTime.

The system has large number of available workers.
The system receives in parallel many job requests to execute.
The system must implement API to get new jobs. 
Each job has the following properties: ID (number), Action (method), Duration (time) , HighPriority (bool)  

The system needs to find the best available resource worker (based on Worker MaxHandleTime) and send the job to the worker. 
If there are no available worker, a job will wait in queue until some worker can take it.
Jobs in the queue should be handled by the time they received by the system.

Priority Jobs
Jobs have HighPriority flag (bool). Jobs  with high priority must be handled ASAP before any other jobs.

Advanced:
The resource workers are running in different process than the main program that receives jobs to handle.
During the lifetime of the system workers can be added or deleted to the system.
