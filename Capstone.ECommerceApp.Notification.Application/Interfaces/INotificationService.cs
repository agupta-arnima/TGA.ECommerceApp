namespace Capstone.ECommerceApp.Notification.Application.Interfaces;

public interface INotificationService
{
    Task RegisterUserEmailAndLog(string userEmail);
}