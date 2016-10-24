using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbCom.CLP
{
    public class Constraint
    {
        public LinearFunction f;
        public double RHS;

        public Constraint()
        {
            f = new LinearFunction();
            RHS = 0.0;
        }

        public Constraint(LinearFunction f, double RHS)
        {
            this.f = f;
            this.RHS = RHS;
        }

        public Constraint shallow_clone()
        {
            return new Constraint(f, RHS);
        }

        public Constraint shallow_clone(double new_RHS)
        {
            return new Constraint(f, new_RHS);
        }
    }
}
