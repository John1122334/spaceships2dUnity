using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lpsolve55;
using UnityEngine;

namespace KerbCom.CLP.Solvers
{
    public class MaxLPSolve : Solver, IDisposable
    {
        private Dictionary<BoundedVariable, int> index_for_var;

        bool init = false;
        private int lp;

        private double[] results;
        private ReadOnlyIndexer<BoundedVariable, double> results_accessor;

        public override ReadOnlyIndexer<BoundedVariable, double> values
        {
            get
            {
                return results_accessor;
            }
        }

        private Status _status;

        public override Solver.Status status
        {
            get { return _status; }
        }

        public MaxLPSolve(Problem problem, IEnumerable<BoundedVariable> variables = null)
        {
            this.problem = problem;
            if (variables != null)
                this.variables = new HashSet<BoundedVariable>(variables).ToArray();
            else
            {
                var set = new HashSet<BoundedVariable>();
                foreach (Constraint c in problem.constraints)
                    foreach (BoundedVariable v in c.f.terms.Keys)
                        set.Add(v);
                this.variables = set.ToArray();
            }
            index_for_var = new Dictionary<BoundedVariable, int>();
            for (int i = 0; i < this.variables.Length; ++i)
                index_for_var[this.variables[i]] = i;
            lp = lpsolve.make_lp(problem.constraints.Count, this.variables.Count());
            lpsolve.set_verbose(lp, 3);
            //lpsolve.set_scaling(lp, lpsolve.lpsolve_scales.SCALE_CURTISREID | lpsolve.lpsolve_scales.SCALE_DYNUPDATE);
            init = true;
            int j;
            int varcount = this.variables.Count();
            int rowcount = problem.constraints.Count;
            for (j = 0; j < rowcount; ++j)
            {
                lpsolve.set_constr_type(lp, j + 1, lpsolve.lpsolve_constr_types.EQ);
            }
            lpsolve.set_maxim(lp);
            results = new double[varcount];
            results_accessor = new ReadOnlyIndexer<BoundedVariable, double>(
            (BoundedVariable v) =>
            {
                return results[index_for_var[v]];
            }, this.variables);

        }

        public override double objective_value
        {
            get { return lpsolve.get_objective(lp); }
        }

        public override void solve()
        {
            //lpsolve.default_basis(lp);
            //setup
            int varcount = variables.Count();
            int rowcount = problem.constraints.Count;
            double[] col = new double[rowcount + 1];
            int i, j;
            for (i = 0; i < varcount; ++i)
            {
                BoundedVariable var = variables[i];
                lpsolve.set_bounds(lp, i + 1, var.lowerBound, var.upperBound == double.PositiveInfinity ? lpsolve.get_infinite(lp) : var.upperBound);
                col[0] = problem.objective[var];
                for (j = 0; j < rowcount; ++j)
                {
                    col[j + 1] = problem.constraints[j].f[var];
                }
                lpsolve.set_column(lp, i + 1, col);
            }
            for (j = 0; j < rowcount; ++j)
            {
                lpsolve.set_rh(lp, j + 1, problem.constraints[j].RHS);
            }

            //solve
            lpsolve.lpsolve_return result = lpsolve.solve(lp);
            if (result == lpsolve.lpsolve_return.NUMFAILURE)
            {
                lpsolve.default_basis(lp);
                result = lpsolve.solve(lp);
            }
            switch (result)
            {
                case lpsolve.lpsolve_return.OPTIMAL:
                    lpsolve.get_variables(lp, results);
                    _status = Status.Optimal;
                    break;
                case lpsolve.lpsolve_return.INFEASIBLE:
                    _status = Status.Infeasible;
                    break;
                case lpsolve.lpsolve_return.UNBOUNDED:
                    _status = Status.Unconstrained;
                    break;
                default:
                    _status = Status.Error;
                    break;
            }
            if(result != lpsolve.lpsolve_return.OPTIMAL)
                Debug.Log("Solver result: " + result);
        }

        public void Dispose()
        {
            if (init)
            {
                lpsolve.delete_lp(lp);
                init = false;
            }
        }
    }
}
