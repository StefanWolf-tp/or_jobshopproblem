using Google.OrTools.Sat;
using or_jobshopproblem;
using or_jobshopproblem.entities;

public class Program
{
    public static void Main(String[] args)
    {
        //Init variables
        Tool tool1 = new(1, 40, 200);
        Tool tool2 = new(2, 35, 200);
        Tool tool3 = new(3, 45, 200);

        //job1
        Job j1 = new(1, 25);
        j1.AddJobTask(new JobTask(1, 3, tool1, j1));
        j1.AddJobTask(new JobTask(2, 2, tool2, j1));
        j1.AddJobTask(new JobTask(3, 2, tool3, j1));

        //job2
        Job j2 = new(2, 5);
        j2.AddJobTask(new JobTask(1, 2, tool1, j2));
        j2.AddJobTask(new JobTask(2, 1, tool3, j2));
        j2.AddJobTask(new JobTask(3, 4, tool2, j2));

        //job3
        Job j3 = new(3, 25);
        j3.AddJobTask(new JobTask(1, 4, tool2, j3));
        j3.AddJobTask(new JobTask(2, 3, tool3, j3));
        j3.AddJobTask(new JobTask(3, 3, tool1, j3));
        j3.AddJobTask(new JobTask(4, 2, tool3, j3));

        //job4
        Job j4 = new(4, 20);
        j4.AddJobTask(new JobTask(1, 4, tool2, j4));
        j4.AddJobTask(new JobTask(2, 3, tool3, j4));
        j4.AddJobTask(new JobTask(3, 3, tool1, j4));
        j4.AddJobTask(new JobTask(4, 2, tool3, j4));

        //job5
        Job j5 = new(5, 30);
        j5.AddJobTask(new JobTask(1, 4, tool2, j5));
        j5.AddJobTask(new JobTask(2, 3, tool3, j5));
        j5.AddJobTask(new JobTask(3, 3, tool1, j5));
        j5.AddJobTask(new JobTask(4, 2, tool3, j5));

        List<Job> allJobs = new() { j1, j2, j3, j4, j5 };
        List<Tool> allTools = new() { tool1, tool2, tool3 };

        int horizon = 0;
        foreach (var job in allJobs)
        {
            foreach (var task in job.JobTaskList.ToList())
            {
                horizon += task.TaskDurration;
            }
        }

        CpModel model = new CpModel();
        CpModelPreparer mp = new(model, allJobs, horizon);
        mp.PrepareModelWithVariablesAndConstraints();
        mp.AddObjectives();

        // Solve
        CpSolver solver = new CpSolver();
        CpSolverStatus status = solver.Solve(model);
        Console.WriteLine($"Solve status: {status}");

        if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
        {
            Console.WriteLine("Solution:");
            Dictionary<Tool, List<JobTask>> assignedJobs = new();
            foreach (Job job in allJobs)
            {
                foreach (JobTask task in job.JobTaskList)
                {
                    if (!assignedJobs.ContainsKey(task.RequiredTool))
                    {
                        assignedJobs.Add(task.RequiredTool, new List<JobTask>());
                    }
                    task.Start = (int)solver.Value(task?.TimingVars?.Item1);
                    assignedJobs[task.RequiredTool].Add(task);
                }
            }

            // Create per output lines.
            String output = "";
            foreach (Tool tool in allTools)
            {
                // Sort by starting time.
                assignedJobs[tool].Sort();
                String solLineTasks = $"Tool {tool.ToolId}: ";
                String solLine = "           ";

                foreach (var assignedTask in assignedJobs[tool])
                {
                    String name = $"job_{assignedTask.RootJob.JobId}_task_{assignedTask.TaskId}";
                    // Add spaces to output to align columns.
                    solLineTasks += $"{name,-15}";

                    String solTmp = $"[{assignedTask.Start},{assignedTask.Start + assignedTask.TaskDurration}]";
                    // Add spaces to output to align columns.
                    solLine += $"{solTmp,-15}";
                }
                output += solLineTasks + "\n";
                output += solLine + "\n";
            }
            // Finally print the solution found.
            Console.WriteLine($"Optimal Schedule Length: {solver.ObjectiveValue}");
            Console.WriteLine($"\n{output}");
        }
        else
        {
            Console.WriteLine("No solution found.");
        }

        Console.WriteLine("Statistics");
        Console.WriteLine($"  conflicts: {solver.NumConflicts()}");
        Console.WriteLine($"  branches : {solver.NumBranches()}");
        Console.WriteLine($"  wall time: {solver.WallTime()}s");

        foreach (Job job in allJobs)
        {
            int durSum = 0;
            string durs = "";
            foreach (JobTask jt in job.JobTaskList)
            {
                int curDur = (int)solver.Value(jt.TimingVars?.Item4);
                durSum += curDur;
                durs += $"{curDur}, ";
            }
            Console.WriteLine($"  job { job.JobId} took total: {durSum}  --> {durs}");
        }
    }
}
    