using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LFAOIReview
{
    class DoubleToPointConvertert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var convertFrom = new FontSizeConverter().ConvertFrom(value.ToString());
            if (convertFrom != null)
                return (double)convertFrom;
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
