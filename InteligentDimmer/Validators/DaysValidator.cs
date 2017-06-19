using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InteligentDimmer.Validators
{
    public class DaysValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int days;
            if (int.TryParse((string)value, out days))
            {
                if (days > 0)
                {
                    return new ValidationResult(true, null);
                }
                return new ValidationResult(false, "Wrong days number");
            }

            return new ValidationResult(false, "Not a number");
        }
    }
}
