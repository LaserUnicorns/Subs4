using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs4.Common.Helpers
{
    public static class NullableConvert
    {
        public static double? ToDouble(object value)
        {
            if (value == null) return null;

            double d;
            if (double.TryParse(value.ToString(), out d))
                return d;
            return null;
        }
    }
}
