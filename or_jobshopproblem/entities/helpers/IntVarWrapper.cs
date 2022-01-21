using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.Sat;

namespace or_jobshopproblem.entities.helpers
{
    internal class IntVarWrapper
    {
        public string Name { get; set; }
        public IntVar? IntegerVar { get; set; }
        public int LowerBound { get; set; }
        public int UpperBound { get; set; }

        public IntVarWrapper(string name, int lb, int ub)
        {
            this.Name = name;
            this.LowerBound = lb;
            this.UpperBound = ub;
        }

        public IntVar initVariable(CpModel model)
        {
            this.IntegerVar = model.NewIntVar(this.LowerBound, this.UpperBound, this.Name);
            return this.IntegerVar;
        }

        public int getValue(CpSolver solver)
        {
            return (int)solver.Value(this.IntegerVar);
        }
    }
}
