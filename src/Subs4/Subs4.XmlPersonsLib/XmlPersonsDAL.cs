using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Subs4.Common.Classes;
using Subs4.Common.Helpers;

namespace Subs4.XmlPersonsLib
{
    public class XmlPersonsDAL
    {
        public IEnumerable<Person> GetPersons(string filename)
        {
            var xe = XElement.Load(filename);
            return xe.Elements("person").Select(ToPerson).ToList();
        }

        private static Person ToPerson(XElement e)
        {
            var person = new Person
            {
                LastName = e.Element("LastName").Value,
                FirstName = e.Element("FirstName").Value,
                MiddleName = e.Element("MiddleName").Value,
                SNILS = e.Element("SSN").Value,
                DOB = DateTime.Parse(e.Element("BirthDate").Value),
                Address = new Address
                {
                    ZipCode = e.Element("Index").Value,
                    City = e.Element("City").Value,
                    Street = e.Element("Street").Value,
                    House = int.Parse(e.Element("House").Value),
                    Building = int.Parse(e.Element("Building").Value),
                    Flat = int.Parse(e.Element("Flat").Value),
                    Room = NullableConvert.ToInt(e.Element("Room").Value)
                }
            };

            var mainCategory = new Category
            {
                Code = e.Element("Category").Value,
                IsMain = true
            };

            person.Categories = person.Categories.Append(mainCategory).ToList();

            if (e.Element("AdditionalCategories") != null)
            {
                var additioncalCategories = e.Element("AdditionalCategories").Elements("Category").Select(x => new Category { Code = x.Attribute("num").Value }).ToList();
                person.Categories = person.Categories.Concat(additioncalCategories).ToList();
            }

            return person;
        }
    }
}
