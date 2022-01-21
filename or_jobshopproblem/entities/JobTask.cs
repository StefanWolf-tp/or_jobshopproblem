using Google.OrTools.Sat;

namespace or_jobshopproblem.entities
{
    internal class JobTask : IComparable
    {
        internal Tuple<IntVar, IntVar, IntervalVar, IntVar>? TimingVars { get; set; }

        public int TaskId { get; set; }
        public int TaskDurration { get; set; }
        public int Start { get; set; }
        public Tool RequiredTool { get; set; }
        public Job RootJob { get; set; }

        public JobTask(int id, int durration, Tool rt, Job rj)
        {
            this.TaskId = id;
            this.TaskDurration = durration;
            this.RequiredTool = rt;
            this.RootJob = rj;
        }

        public int CompareTo(object? obj)
        {
            if (obj == null)
                return 1;

            if (obj is JobTask otherTask)
            {
                if (this.Start != otherTask.Start)
                    return this.Start.CompareTo(otherTask.Start);
                else
                    return this.TaskDurration.CompareTo(otherTask.TaskDurration);
            }
            else
                throw new ArgumentException("Object to compare with is not a JobTask");
        }
    }
}
