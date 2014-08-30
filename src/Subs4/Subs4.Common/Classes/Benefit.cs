using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs4.Common.Classes
{
    public class Benefit
    {
        public string ServiceGroupCode { get; set; }
        public string CategoryCode { get; set; }
        public double Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0} | {1} | {2}", ServiceGroupCode, CategoryCode, Value);
        }
    }
}
