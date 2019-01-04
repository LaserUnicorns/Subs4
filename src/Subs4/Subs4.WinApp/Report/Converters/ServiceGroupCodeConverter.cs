using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Subs4.Common;

namespace Subs4.WinApp.Report.Converters
{
    class ServiceGroupCodeConverter : IValueConverter
    {
        private static readonly Dictionary<string, string> Services =
            new Dictionary<string, string>
            {
                {ServiceCodes.Maintenance, "Содержание"},
                {ServiceCodes.Heating, "Отопление"},
                {ServiceCodes.HotWater, "Горячая вода"},
                {ServiceCodes.ColdWater, "Холодная вода"},
                {ServiceCodes.Sewerage, "Канализация"},
                {ServiceCodes.Gas, "Газ"},
                {ServiceCodes.Garbage, "Вывоз мусора"},
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
