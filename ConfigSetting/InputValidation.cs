using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ConfigSetting
{
    public class IntValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int d = 0;
            if (int.TryParse(value.ToString(), out d))
            {
                if(d>0)
                    return new ValidationResult(true, null);
            }

            return new ValidationResult(false, "Validation Failed");
        }
    }

    public class DoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double d = 0;
            if (double.TryParse(value.ToString(), out d))
            {
                if(d>0.0)
                    return new ValidationResult(true, null);
            }

            return new ValidationResult(false, "Validation Failed");
        }
    }

    public class FloatValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            float d = 0;
            if (float.TryParse(value.ToString(), out d))
            {
                if(d>0.0f)
                    return new ValidationResult(true, null);
            }

            return new ValidationResult(false, "Validation Failed");
        }
    }

    public class StringValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (!string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(true, null);

            return new ValidationResult(false, "Validation Failed");
        }
    }
}
