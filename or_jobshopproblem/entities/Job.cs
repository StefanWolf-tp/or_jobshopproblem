namespace or_jobshopproblem.entities
{
    internal class Job
    {
        public int JobId { get; set; }
        public List<JobTask> JobTaskList { get; set; } = new List<JobTask>();

        public int UnitAmount { get; set; }

        public Job(int id, int wc)
        {
            this.JobId = id;
            this.UnitAmount = wc;
        }

        public Job(int id, int wc, List<JobTask> jobTaskList)
        {
            this.JobId = id;
            this.UnitAmount = wc;
            this.JobTaskList = jobTaskList;
        }

        public void AddJobTask(JobTask jt)
        {
            this.JobTaskList.Add(jt);
        }

        public JobTask? getJobTaskById(int id)
        {
            return this.JobTaskList?.Find(jt => jt.TaskId == id);
        }
    }
}
