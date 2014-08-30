using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs4.CsvReportReaderLib;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = @"C:\Users\smb\SkyDrive\atlant\1408.csv";
            var reader = new CsvReportReader();
            var r = reader.Load(filename);
        }
    }
}
