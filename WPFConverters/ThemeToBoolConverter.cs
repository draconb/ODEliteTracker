using ODEliteTracker.Themes;
using System.Globalization;
using System.Windows.Data;

namespace ODEliteTracker.WPFConverters
{
    internal class ThemeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not string enumString)
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorParameterMustBeAnEnumName");
            }

            if (!Enum.IsDefined(typeof(Theme), value))
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorValueMustBeAnEnum");
            }

            var enumValue = Enum.Parse<Theme>(enumString);

            return enumValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not string enumString)
            {
                throw new ArgumentException("ExceptionThemeToBoolConvertorParameterMustBeAnEnumName");
            }

            return Enum.Parse<Theme>(enumString);
        }
    }
}
