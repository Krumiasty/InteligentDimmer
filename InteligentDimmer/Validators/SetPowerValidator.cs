using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace InteligentDimmer.Validators
{
    public class SetPowerValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var tmpstr = value as string;
            if (string.IsNullOrEmpty(tmpstr))
            {
                return new ValidationResult(false, "This field cannot be empty!");
            }
            var lastChar = tmpstr.Last();
            var strwithoutpercent = lastChar == '%' ? tmpstr.Remove(tmpstr.Length - 1) : tmpstr;
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
