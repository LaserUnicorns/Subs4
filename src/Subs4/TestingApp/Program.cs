using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs4.CsvReportReaderLib;
using XmlPersonsLib;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = @"C:\Users\smb\SkyDrive\atlant\1408.csv";
            var personsFileName = @"C:\Users\smb\SkyDrive\atlant\persons.xml";
            var xmldal = new XmlPersonsDAL();
            var persons = xmldal.GetPersons(personsFileName);
            var reader = new CsvReportReader(persons);
            var report = reader.Load(filename);
        }
    }
}
