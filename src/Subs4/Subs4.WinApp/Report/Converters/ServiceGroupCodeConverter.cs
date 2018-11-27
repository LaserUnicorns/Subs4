using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Subs4.WinApp.Report.Converters
{
    class ServiceGroupCodeConverter : IValueConverter
    {
        private static readonly Dictionary<string, string> Services =
            new Dictionary<string, string>
            {
                //{"03", "Содержание"},
                {"01", "Содержание"},
                {"10", "Отопление"},
                {"11", "Горячая вода"},
                {"12", "Холодная вода"},
                {"13", "Канализация"},
                {"22", "Газ"}
            };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var code = value.ToString();
            return Services.ContainsKey(code) ? Services[code] : code;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
