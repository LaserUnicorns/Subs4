using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Subs4.Common.Classes;

namespace Subs4.WinApp.Report.Converters
{
    public class PersonBenefitSumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var person = value as Person;
            if (person == null) return null;
            return person.Benefits.Sum(x => x.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
