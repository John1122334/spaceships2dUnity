using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbCom.CLP
{
    public class Problem
    {
        public List<Constraint> constraints = new List<Constraint>();

        public LinearFunction objective = new LinearFunction();
    }
}
