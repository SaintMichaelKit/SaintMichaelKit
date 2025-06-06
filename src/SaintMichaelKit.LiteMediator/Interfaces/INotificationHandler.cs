namespace SaintMichaelKit.LiteMediator.Interfaces;

/// <summary>
/// Handles a specific type of notification.
/// </summary>
public interface INotificationHandler<TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Handles the notification.
    /// </summary>
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}