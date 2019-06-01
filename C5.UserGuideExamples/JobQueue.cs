// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: job queue 2004-11-22

// Compile with
//   csc /r:netstandard.dll /r:C5.dll JobQueue.cs

using System;

namespace C5.UserGuideExamples
{
    internal class JobQueueProgram
    {
        public static void Main()
        {
            var jq = new JobQueue();

            // One user submits three jobs at time=27
            var rid1 = jq.Submit(new Ip("62.150.83.11"), 27);
            _ = jq.Submit(new Ip("62.150.83.11"), 27);

            // One user submits three jobs at time=27
            var rid3 = jq.Submit(new Ip("62.150.83.11"), 27);

            // One job is executed
            jq.ExecuteOne();

            // Another user submits two jobs at time=55
            _ = jq.Submit(new Ip("130.225.17.5"), 55);
            _ = jq.Submit(new Ip("130.225.17.5"), 55);

            // One more job is executed
            jq.ExecuteOne();

            // The first user tries to cancel his first and last job
            jq.Cancel(rid1);
            jq.Cancel(rid3);
            // The remaining jobs are executed
            while (jq.ExecuteOne() != null)
            {
            }
        }
    }

    internal class JobQueue
    {
        private readonly IPriorityQueue<Job> _jobQueue;
        private readonly IDictionary<Rid, IPriorityQueueHandle<Job>> _jobs;
        private readonly HashBag<Ip> _userJobs;

        public JobQueue()
        {
            _jobQueue = new IntervalHeap<Job>();
            _jobs = new HashDictionary<Rid, IPriorityQueueHandle<Job>>();
            _userJobs = new HashBag<Ip>();
        }

        public Rid Submit(Ip ip, int time)
        {
            var jobCount = _userJobs.ContainsCount(ip);
            var rid = new Rid();
            var job = new Job(rid, ip, time + 60 * jobCount);
            IPriorityQueueHandle<Job> h = default;

            _jobQueue.Add(ref h, job);
            _userJobs.Add(ip);
            _jobs.Add(rid, h);

            Console.WriteLine($"Submitted {job}");

            return rid;
        }

        public Job ExecuteOne()
        {
            if (!_jobQueue.IsEmpty)
            {
                var job = _jobQueue.DeleteMin();
                _userJobs.Remove(job.Ip);
                _jobs.Remove(job.Rid);
                Console.WriteLine($"Executed {job}");
                return job;
            }
            else
            {
                return null;
            }
        }

        public void Cancel(Rid rid)
        {
            if (_jobs.Remove(rid, out IPriorityQueueHandle<Job> h))
            {
                var job = _jobQueue.Delete(h);
                _userJobs.Remove(job.Ip);
                Console.WriteLine($"Cancelled {job}");
            }
        }
    }

    internal class Job : IComparable<Job>
    {
        public Rid Rid { get; }
        public Ip Ip { get; }
        public int Time { get; }

        public Job(Rid rid, Ip ip, int time)
        {
            Rid = rid;
            Ip = ip;
            Time = time;
        }

        public int CompareTo(Job that)
        {
            return Time - that.Time;
        }

        public override string ToString()
        {
            return Rid.ToString();
        }
    }

    internal class Rid
    {
        private readonly int _ridNumber;
        private static int _nextRid = 1;

        public Rid()
        {
            _ridNumber = _nextRid++;
        }

        public override string ToString()
        {
            return $"rid={_ridNumber}";
        }
    }

    internal class Ip
    {
        public string IpString { get; }

        public Ip(string ipString)
        {
            IpString = ipString;
        }

        public override int GetHashCode()
        {
            return IpString.GetHashCode();
        }

        public override bool Equals(object that)
        {
            return IpString.Equals(that);
        }
    }
}
