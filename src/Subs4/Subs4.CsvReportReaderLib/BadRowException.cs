using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs4.Common.Exceptions;

namespace Subs4.CsvReportReaderLib
{
    public class BadRowException : BusinessException
    {
        private readonly string _personName;

        public BadRowException(string personName)
        {
            _personName = personName;
        }

        public override string Message
        {
            get { return string.Format("Wrong sum at person row ({0}).", _personName); }
        }
    }
}
