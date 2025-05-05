using ODEliteTracker.Notifications.Themes;
using System.Globalization;
using System.Windows.Data;

namespace ODEliteTracker.WPFConverters
{
    public sealed class NotificationThemeToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not NotificationTheme pos)
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorParameterMustBeAnEnumName");
            }

            if (!Enum.IsDefined(typeof(NotificationTheme), value))
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorValueMustBeAnEnum");
            }

            return (NotificationTheme)value == pos;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not NotificationTheme pos)
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorParameterMustBeAnEnumName");
            }

            return pos;
        }
    }
}
