using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InteligentDimmer.Validators
{
    public class MinutesValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int minute;
            if (int.TryParse((string)value, out minute))
            {
                if (minute >= 0 && minute < 60)
                {
                    return new ValidationResult(true, null);
                }
                return new ValidationResult(false, "Wrong minute");
            }

            return new ValidationResult(false, "Not a number");
        }
    }
}
