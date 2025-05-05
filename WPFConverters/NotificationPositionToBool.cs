using ODEliteTracker.Models.Settings;
using System.Globalization;
using System.Windows.Data;

namespace ODEliteTracker.WPFConverters
{
    internal class NotificationPositionToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not NotificationPlacement pos)
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorParameterMustBeAnEnumName");
            }

            if (!Enum.IsDefined(typeof(NotificationPlacement), value))
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorValueMustBeAnEnum");
            }

            return (NotificationPlacement)value == pos;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not NotificationPlacement pos)
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorParameterMustBeAnEnumName");
            }

            return pos;
        }
    }
}
