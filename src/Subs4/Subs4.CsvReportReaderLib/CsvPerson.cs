using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs4.CsvReportReaderLib
{
    class CsvPerson
    {
        public string LastNameWithInitials { get; set; }
        public string Address { get; set; }

        public double? Heating { get; set; }
        public double? Maintenance { get; set; }
        public double? HotWater { get; set; }
        public double? ColdWater { get; set; }
        public double? Sewerage { get; set; }
        public double? Gas { get; set; }

        public double? Sum { get; set; }

        public double CalcSum
        {
            get
            {
                return Heating.GetValueOrDefault() +
                       Maintenance.GetValueOrDefault() +
                       HotWater.GetValueOrDefault() +
                       ColdWater.GetValueOrDefault() +
                       Sewerage.GetValueOrDefault() +
                       +Gas.GetValueOrDefault();
            }
        }
    }
}
