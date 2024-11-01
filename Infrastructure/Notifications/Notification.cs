namespace Infrastructure.Notifications;

public record Notification(
    string email,
    string message,
    Guid inspectionId);