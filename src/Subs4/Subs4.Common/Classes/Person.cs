using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs4.Common.Classes
{
    public class Person
    {
        public Person()
        {
            Categories = new List<Category>();
            Benefits = new List<Benefit>();
        }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SNILS { get; set; }
        public DateTime DOB { get; set; }
        public Address Address { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1} {2}", LastName, FirstName, MiddleName).Trim(); }
        }

        public string LastNameWithInitials
        {
            get { return string.Format("{0} {1}. {2}.", LastName, FirstName[0], MiddleName[0]); }
        }

        
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Benefit> Benefits { get; set; }

        public override string ToString()
        {
            return LastNameWithInitials;
        }
    }
}
