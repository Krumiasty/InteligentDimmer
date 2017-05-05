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
            // TODO still accepts 123
            var tmpstr = value as string;
            var lastChar = tmpstr.Last();
            string strwithoutpercent;
            if (lastChar == '%')
            {
                strwithoutpercent = tmpstr.Remove(tmpstr.Length - 1);
            }
            else
            {
                strwithoutpercent = tmpstr;
            }
            int powerValue;
            if (!int.TryParse(strwithoutpercent, out powerValue))
            {
                return new ValidationResult(false, "Not a number");
            }
            if (powerValue >= 0 && powerValue <= 100)
            {
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, "Set power: 0 - 100");
        }
    }
}
