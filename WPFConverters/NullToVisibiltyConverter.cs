using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ODEliteTracker.WPFConverters
{
    internal class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not string enumString)
            {
                throw new ArgumentException("ExceptionNullToVisibilityConverterParameterMustBeAnEnumName");
            }

            if (Enum.TryParse<Visibility>(enumString, out var vis))
            { 
                return value == null ? vis : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
