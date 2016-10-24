using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbCom.CLP.Solvers
{
    public abstract class Solver
    {
        protected Problem problem;
        protected BoundedVariable[] variables;
        public enum Status
        {
            Optimal,
            Unconstrained,
            Infeasible,
            Error
        }
        public abstract double objective_value { get; }
        public abstract void solve();
        public abstract Status status { get; }
        public abstract KerbCom.ReadOnlyIndexer<KerbCom.CLP.BoundedVariable, double> values { get; }

        public string dump(IDictionary<BoundedVariable, string> vnames, IDictionary<Constraint, string> cnames)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Variables: " + String.Join(", ", variables.Select(v => vnames[v]).ToArray()));
            sb.AppendLine("Maximise Z = " + String.Join(" + ", variables.Select(v => problem.objective[v] + vnames[v]).ToArray()));
            foreach (Constraint c in problem.constraints)
            {
                sb.AppendLine(cnames[c] + ": " + String.Join(" + ", variables.Select(v => c.f[v] + vnames[v]).ToArray()) + " = " + c.RHS);
            }
            return sb.ToString();
        }
    }
}
