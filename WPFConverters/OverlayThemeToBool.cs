using ODEliteTracker.Themes.Overlay;
using System.Globalization;
using System.Windows.Data;

namespace ODEliteTracker.WPFConverters
{
    internal class OverlayThemeToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not OverlayTheme pos)
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorParameterMustBeAnEnumName");
            }

            if (!Enum.IsDefined(typeof(OverlayTheme), value))
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorValueMustBeAnEnum");
            }

            return (OverlayTheme)value == pos;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not OverlayTheme pos)
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorParameterMustBeAnEnumName");
            }

            return pos;
        }
    }
}
