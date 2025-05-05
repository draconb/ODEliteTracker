namespace ODEliteTracker.Notifications
{
    public enum NotificationType
    {
        System,
        Station
    }

    public record NotificationArgs(string Header, string[] Text, NotificationType Type);
}
