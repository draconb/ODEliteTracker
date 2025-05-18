using ODEliteTracker.ViewModels.PopOuts;
using ODMVVM.ViewModels;

namespace ODEliteTracker.Models.Settings
{
    [Flags]
    public enum PopOutSettings
    {
        None,
        ShowInTaskBar = 1 << 0,
        AlwaysOnTop = 1 << 1,
        Active = 1 << 2,
    }

    public sealed class PopOutParams
    {
        public Type? Type { get; set; }
        public int Count { get; set; }
        public ODWindowPosition Position { get; set; } = new();
        public bool AlwaysOnTop { get; set; }
        public bool ShowTitle { get; set; } = true;
        public bool ShowInTaskBar { get; set; } = true;
        public bool ClickThrough { get; set; }
        public bool Active { get; set; }
        public object? AdditionalSettings { get; set; }
        public double UiScale { get; set; } = 1d;
        public double Opacity { get; set; } = 1d;

        public static PopOutParams CreateParams(PopOutViewModel popOut, int count, bool active)
        {
            return new()
            {
                Type = popOut.GetType(),
                Count = count,
                Position = popOut.Position.Clone(),
                AlwaysOnTop = popOut.AlwaysOnTop,
                ShowTitle = popOut.ShowTitle,
                ShowInTaskBar = popOut.ShowInTaskBar,
                ClickThrough = popOut.ClickThrough,
                Active = active,
                AdditionalSettings = popOut.AdditionalSettings,
                UiScale = popOut.UiScale,
                Opacity = popOut.Opacity
            };
        }

        public void UpdateParams(PopOutViewModel popOut, bool active)
        {
            Position = popOut.Position.Clone();
            AlwaysOnTop = popOut.AlwaysOnTop;
            ShowTitle = popOut.ShowTitle;
            ShowInTaskBar = popOut.ShowInTaskBar;
            ClickThrough = popOut.ClickThrough;
            Active = active;
            AdditionalSettings = popOut.AdditionalSettings;
            UiScale = popOut.UiScale;
            Opacity = popOut.Opacity;
        }
    }
}
