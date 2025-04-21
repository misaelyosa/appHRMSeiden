using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp
{
    public class DateToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateOnly date)
            {
                return date.ToString("dd-MM-yyyy");
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(DateOnly.TryParse(value?.ToString(), out DateOnly date))
            {
                return date;
            }
            return default(DateOnly);
        }
    }
}
