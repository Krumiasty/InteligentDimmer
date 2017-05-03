using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InteligentDimmer.Validators
{
    public class SetPowerValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var powerValue = value as int?;
            if (powerValue == null)
            {
                return new ValidationResult(false, "Not a number");
            }
            if (powerValue >= 0 || powerValue <= 100)
            {
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, "Set power: 0 - 100");
        }
    }
}
