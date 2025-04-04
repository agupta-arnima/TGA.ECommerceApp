using Capstone.ECommerceApp.Notification.Data.Context;
using Capstone.ECommerceApp.Notification.Domain.Interfaces;
using Capstone.ECommerceApp.Notification.Domain.Models;

namespace Capstone.ECommerceApp.Notification.Data.Repository;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext notificationDbContext;
    public NotificationRepository(NotificationDbContext notificationDbContext)
    {
        this.notificationDbContext = notificationDbContext;
    }

    public async Task LogEmail(string message, string email)
    {
        EmailLogger emailLog = new EmailLogger
        {
            Email = email, //email to send to
            EmailSent = DateTime.Now, //time sent
            Message = message //message to be sent
        };
        await notificationDbContext.EmailLoggers.AddAsync(emailLog);
        await notificationDbContext.SaveChangesAsync();
    }
}
