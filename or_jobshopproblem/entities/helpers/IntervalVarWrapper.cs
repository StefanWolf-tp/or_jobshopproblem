using Google.OrTools.Sat;

namespace or_jobshopproblem.entities.helpers
{
    internal class IntervalVarWrapper
    {
        public string Name { get; set; }
        public IntervalVar? IntervalVar { get; set; }
        public int LowerBound { get; set; }
        public int UpperBound { get; set; }

        public IntervalVarWrapper(string name, int lb, int ub)
        {
            this.Name = name;
            this.LowerBound = lb;
            this.UpperBound = ub;
        }

        public IntervalVar InitVariable(CpModel model)
        {
            this.IntervalVar = model.NewIntervalVar(this.LowerBound, 50, this.UpperBound, this.Name);            
            return this.IntervalVar;
        }

        /*public int getValue(CpSolver solver)
        {
            return (int)solver.Value(this.IntervalVar);
        }*/

    }
}
