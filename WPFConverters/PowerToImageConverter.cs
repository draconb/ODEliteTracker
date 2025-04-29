using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ODEliteTracker.WPFConverters
{
    public sealed class PowerToImageConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                switch (s)
                {
                    case "Aisling Duval":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/aisling-duval.png"));
                    case "Archon Delaine":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/archon-delaine.png"));
                    case "A. Lavigny-Duval":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/arissa-lavigny-duval.png"));
                    case "Denton Patreus":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/denton-patreus.png"));
                    case "Edmund Mahon":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/edmund-mahon.png"));
                    case "Felicia Winters":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/felicia-winters.png"));
                    case "Li Yong-Rui":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/li-yong-rui.png"));
                    case "Pranav Antal":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/pranav-antal.png"));
                    case "Zachary Hudson":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/zachary-hudson.png"));
                    case "Zemina Torval":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/zemina-torval.png"));
                    case "Yuri Grom":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/yuri-grom.png"));
                    case "Nakato Kaine":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/nakato-kaine.png"));
                    case "Jerome Archer":
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/PowerPlay/jerome-archer.png"));
                }
            }

            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
