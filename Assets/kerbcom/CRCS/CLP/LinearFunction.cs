using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbCom.CLP
{
    public class LinearFunction
    {
        public Dictionary<BoundedVariable, double> terms = new Dictionary<BoundedVariable,double>();

        public double this[BoundedVariable v]
        {
            get
            {
                double result;
                terms.TryGetValue(v, out result);
                return result;
            }
            set
            {
                if (value == 0.0)
                    terms.Remove(v);
                else
                    terms[v] = value;
            }
        }
    }
}
