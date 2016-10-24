using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbCom.CLP
{
    public class BoundedVariable
    {
        public double lowerBound, upperBound;

        public BoundedVariable(double lowerBound = 0.0, double upperBound = double.PositiveInfinity)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }
    }
}
