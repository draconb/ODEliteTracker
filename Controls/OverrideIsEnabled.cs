using System.Windows.Controls;
using System.Windows;

namespace ODEliteTracker.Controls
{
    public sealed class OverrideIsEnabled : ContentControl
    {
        static OverrideIsEnabled()
        {
            IsEnabledProperty.OverrideMetadata(
                typeof(OverrideIsEnabled),
                new UIPropertyMetadata(
                    defaultValue: true,
                    propertyChangedCallback: (_, __) => { },
                    coerceValueCallback: (_, x) => x));
        }
    }
}
