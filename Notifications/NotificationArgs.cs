using ODEliteTracker.Models.Settings;

namespace ODEliteTracker.Notifications
{
    public record NotificationArgs(string Header, string[] Text, NotificationOptions Type);
}
