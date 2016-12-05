using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace jTech.Wpf.Converters.VisibilityConverters
{
    public abstract class VisibilityConverter : IValueConverter
    {
        public Visibility InvisibilityType { get; set; }
        public bool IsReversed { get; set; }

        public VisibilityConverter()
        {
            InvisibilityType = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isVisibile = IsVisible(value, targetType, parameter, culture);

            return isVisibile && !IsReversed
                ? Visibility.Visible
                : InvisibilityType;
        }

        protected abstract bool IsVisible(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }



    }
}
