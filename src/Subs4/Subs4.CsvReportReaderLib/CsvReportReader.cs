using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs4.Common.Classes;
using Subs4.Common.Helpers;

namespace Subs4.CsvReportReaderLib
{
    public class CsvReportReader
    {
        private const int HEADER_ROWS = 8;
        private const int FOOTER_ROWS = 3;

        public IEnumerable<Person> Load(string filename)
        {
            var r = File.ReadLines(filename, Encoding.Default)
                        .Skip(HEADER_ROWS)
                        .Reverse()
                        .Skip(FOOTER_ROWS)
                        .Reverse()
                        .Select(x => x.Split(';'))
                        .Select(ToCsvPerson)
                        .GroupBy(x => x.LastNameWithInitials)
                        .Select(ToPerson)
                        .ToList();

            return r;
        }

        private static CsvPerson ToCsvPerson(string[] line)
        {
            var person = new CsvPerson
                         {
                             LastNameWithInitials = line[1], 
                             Address = line[4],
                             Maintenance = NullableConvert.ToDouble(line[6]),
                             HotWater = NullableConvert.ToDouble(line[7]),
                             ColdWater = NullableConvert.ToDouble(line[9]),
                             Sewerage = NullableConvert.ToDouble(line[10]),
                             Gas = NullableConvert.ToDouble(line[11]),
                             Sum = NullableConvert.ToDouble(line[12])
                         };

            if (Math.Abs(person.Sum.GetValueOrDefault() - person.CalcSum.GetValueOrDefault()) > 0.01)
                throw new BadRowException(person.LastNameWithInitials);

            return person;
        }

        private Person ToPerson(IEnumerable<CsvPerson> csvPersons)
        {
            throw new NotImplementedException();
        }
    }
}
