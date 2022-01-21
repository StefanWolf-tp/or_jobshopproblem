using Google.OrTools.Sat;
using or_jobshopproblem.entities;

namespace or_jobshopproblem
{
    internal class CpModelPreparer
    {
        private CpModel CpModel;
        private readonly int horizon;
        private List<Job> Jobs;

        Dictionary<Tool, List<IntervalVar>> machineToIntervals = new();
        private bool variablesInit = false;

        public CpModelPreparer(CpModel cpm, List<Job> jobs, int hor)
        {
            this.CpModel = cpm;
            this.horizon = hor;
            this.Jobs = jobs;
        }

        public void PrepareModelWithVariablesAndConstraints()
        {
            foreach (Job job in this.Jobs)
            {
                foreach (JobTask jobTask in job.JobTaskList.ToList())
                {
                    string suffix = $"_{job.JobId}_{jobTask.TaskId}";
                    IntVar start = this.CpModel.NewIntVar(0, horizon, "start" + suffix);
                    IntVar end = this.CpModel.NewIntVar(0, horizon, "end" + suffix);
                    IntVar durration = this.CpModel.NewIntVar(jobTask.TaskDurration, jobTask.TaskDurration+1, "end" + suffix);
                    IntervalVar interval = this.CpModel.NewIntervalVar(start, durration, end, "interval" + suffix);
                    //IntervalVar interval = this.CpModel.NewIntervalVar(start, jobTask.TaskDurration, end, "interval" + suffix);
                    var timings = Tuple.Create(start, end, interval, durration);
                    jobTask.TimingVars = timings;
                    if (!this.machineToIntervals.ContainsKey(jobTask.RequiredTool))
                    {
                        this.machineToIntervals.Add(jobTask.RequiredTool, new List<IntervalVar>());
                    }
                    this.machineToIntervals[jobTask.RequiredTool].Add(interval);
                }
            }
            this.variablesInit = true;

            // Create and add disjunctive constraints.
            foreach (Tool tool in ExtractUsedTools(this.Jobs))
            {
                this.CpModel.AddNoOverlap(machineToIntervals[tool]);

                //this.CpModel.Add
            }

            // Create contraints for the right task order for each job
            foreach (Job job in this.Jobs)
            {
                foreach (JobTask jt in job.JobTaskList)
                {
                    if(jt == job.JobTaskList.Last()) { break; }
                    this.CpModel.Add(
                        job.getJobTaskById(jt.TaskId + 1)?.TimingVars?.Item1
                        >=
                        jt.TimingVars?.Item2
                    );
                }
            }
        }

        internal void AddObjectives()
        {
            if (this.variablesInit) {
                IntVar objVar = this.CpModel.NewIntVar(0, this.horizon, "makespan");
                IntVar sumIntervalsVar = this.CpModel.NewIntVar(0, this.horizon, "intervalSum");
                List<IntVar> ends = new List<IntVar>();
                List<IntVar> intervals = new();
                foreach (Job job in this.Jobs) //get for each last task the end time variable
                {
                    try
                    {
                        JobTask jt = job.JobTaskList.Last();
                        if (jt != null && jt.TimingVars != null)
                        {
                            ends.Add(jt.TimingVars.Item2);
                            intervals.Add(jt.TimingVars.Item4);

                        }
                    }catch (Exception ex) {
                        Console.WriteLine($"The current job with id: {job.JobId} does not contain any jobtasks", ex);
                    }
                }

                /*CpModel.AddMaxEquality(sumIntervalsVar, intervals);
                CpModel.Maximize(sumIntervalsVar);*/
                //IntVar durrs = CpModel.NewIntVar(0, this.horizon, "sumDurrations");
                //CpModel.Add(durrs == LinearExpr.Sum(intervals));
                //CpModel.Maximize(durrs);
                //CpModel.Minimize(LinearExpr.Sum(intervals));

                CpModel.AddMaxEquality(objVar, ends); // Adds objVar == Max(variables)
                CpModel.Minimize(objVar); //so with line before -> minimize the greatest end time
            }
        }

        private static List<Tool> ExtractUsedTools(List<Job> jobs)
        {
            List<Tool> toolList = new();
            foreach (var job in jobs)
            {
                foreach (var task in job.JobTaskList.ToList())
                {
                    if (toolList.Contains(task.RequiredTool))
                    {
                        continue;
                    }
                    toolList.Add(task.RequiredTool);
                }
            }

            return toolList;
        }

    }
}
