using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace InteligentDimmer.Converters
{
    public class SliderValueToSunOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intOpacity = value as int?;
            if (intOpacity != null && intOpacity != 0)
            {
                var opacity = (double)intOpacity / 100;
                return opacity;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
