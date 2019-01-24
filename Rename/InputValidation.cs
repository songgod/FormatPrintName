using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rename
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

    public class MLNameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strvalue = value.ToString();
            if (string.IsNullOrWhiteSpace(strvalue))
                return new ValidationResult(false, "输入必须为“数字+#”格式");
            int index = strvalue.IndexOf('#');
            if(index!=strvalue.Count()-1)
                return new ValidationResult(false, "输入必须为“数字+#”格式");

            string strnum = strvalue.Substring(0, strvalue.Count() - 1);
            int ival = 0;
            if(!int.TryParse(strnum, out ival))
                return new ValidationResult(false, "输入必须为“数字+#”格式");
            
            return new ValidationResult(true, null);
        }
    }
}
