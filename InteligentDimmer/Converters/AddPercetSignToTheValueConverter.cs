using System;
using System.Globalization;
using System.Windows.Data;

namespace InteligentDimmer.Converters
{
    public class AddPercetSignToTheValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{value}%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{value}%";
        }
    }
}
