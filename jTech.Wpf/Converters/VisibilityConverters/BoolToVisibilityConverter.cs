using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jTech.Wpf.Converters.VisibilityConverters
{
    public class BoolToVisibilityConverter : VisibilityConverter
    {
        protected override bool IsVisible(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value is bool) && (bool) value;
        }

    }
}
