using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InteligentDimmer.Validators
{
    public class HoursValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int hour;
            if (int.TryParse((string) value, out hour))
            {
                if (hour >= 0 && hour < 24)
                {
                    return new ValidationResult(true, null);
                }
                return new ValidationResult(false, "Wrong hour");
            }

            return new ValidationResult(false, "Not a number");
        }
    }
}
