using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ODEliteTracker.WPFConverters
{
    public sealed class VisibilityToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility vis)
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorParameterMustBeAnEnumName");
            }

            return vis == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
