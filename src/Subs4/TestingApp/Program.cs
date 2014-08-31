using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs4.CsvReportReaderLib;
using Subs4.DbfLib;
using Subs4.ReportLib;
using Subs4.XmlPersonsLib;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = @"C:\Users\smb\SkyDrive\atlant\1408.csv";
            var personsFileName = @"C:\Users\smb\SkyDrive\atlant\persons.xml";
            var pdfFileName = @"C:\Users\smb\SkyDrive\atlant\report.pdf";
            var dbfdb = @"C:\Users\smb\SkyDrive\atlant";

            var xmldal = new XmlPersonsDAL();
            var persons = xmldal.GetPersons(personsFileName);
            var reader = new CsvReportReader(persons);
            var report = reader.Load(filename);

            //ReportCreator.CreateReport(report, pdfFileName);
            //Process.Start(pdfFileName);

            using (var dbf = new DbfDAL())
            {
                dbf.Connect(dbfdb);

                foreach (var person in report)
                {
                    dbf.AddPersonBenefits(person);
                }
            }
        }
    }
}
