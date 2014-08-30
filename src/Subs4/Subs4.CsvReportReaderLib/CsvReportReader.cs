using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Subs4.Common.Classes;
using Subs4.Common.Helpers;

namespace Subs4.CsvReportReaderLib
{
    public class CsvReportReader
    {
        private const int HEADER_ROWS = 8;
        private const int FOOTER_ROWS = 3;

        private readonly Dictionary<string, Func<CsvPerson, double?>> _benefitCodes = new Dictionary<string, Func<CsvPerson, double?>>
                                                                                      {
                                                                                          {"03", p => p.Maintenance},
                                                                                          {"10", p => p.Heating},
                                                                                          {"11", p => p.HotWater},
                                                                                          {"12", p => p.ColdWater},
                                                                                          {"13", p => p.Sewerage},
                                                                                          {"22", p => p.Gas},
                                                                                      };

        private readonly IEnumerable<Person> _personsInfos;

        public CsvReportReader(IEnumerable<Person> personsInfos)
        {
            _personsInfos = personsInfos.ToList();
        }

        public IEnumerable<Person> Load(string filename)
        {
            List<Person> r = File.ReadLines(filename, Encoding.Default)
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

            if (Math.Abs(person.Sum.GetValueOrDefault() - person.CalcSum) > 0.01)
                throw new BadRowException(person.LastNameWithInitials);

            return person;
        }

        private Person ToPerson(IEnumerable<CsvPerson> csvPersons)
        {
            Person personInfo = _personsInfos.First(x => x.LastNameWithInitials == csvPersons.First().LastNameWithInitials);

            var person = new Person
                         {
                             LastName = personInfo.LastName,
                             FirstName = personInfo.FirstName,
                             MiddleName = personInfo.MiddleName,
                             DOB = personInfo.DOB,
                             SNILS = personInfo.SNILS,
                             Address = personInfo.Address,
                             Categories = personInfo.Categories.ToList()
                         };

            string catCode = person.Categories.First(x => x.IsMain).Code;

            if (csvPersons.Count() == 1)
            {
                CsvPerson csvPerson = csvPersons.First();

                foreach (var benefitCode in _benefitCodes)
                {
                    double? val = benefitCode.Value(csvPerson);
                    if (val.HasValue)
                    {
                        var benefit = new Benefit
                                      {
                                          CategoryCode = catCode,
                                          ServiceGroupCode = benefitCode.Key,
                                          Value = val.Value
                                      };
                        person.Benefits = person.Benefits.Append(benefit).ToList();
                    }
                }

                return person;
            }
            if (csvPersons.Count() == 2)
            {
                CsvPerson csvPerson = csvPersons.First(x => x.Sum != x.Maintenance);

                foreach (var benefitCode in _benefitCodes)
                {
                    double? val = benefitCode.Value(csvPerson);
                    if (val.HasValue)
                    {
                        var benefit = new Benefit
                                      {
                                          CategoryCode = catCode,
                                          ServiceGroupCode = benefitCode.Key,
                                          Value = val.Value
                                      };
                        person.Benefits = person.Benefits.Append(benefit).ToList();
                    }
                }

                var otherCsvPerson = csvPersons.First(x => x.Sum == x.Maintenance);

                if (otherCsvPerson.Maintenance.HasValue)
                {
                    var benefit = new Benefit
                                  {
                                      CategoryCode = person.Categories.First(x => !x.IsMain).Code,
                                      ServiceGroupCode = "03",
                                      Value = otherCsvPerson.Maintenance.Value
                                  };
                    person.Benefits = person.Benefits.Append(benefit).ToList();
                }

                return person;
            }

            throw new NotSupportedException();
        }
    }
}