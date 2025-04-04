using System;
using Capstone.ECommerceApp.Notification.Application.Interfaces;
using Capstone.ECommerceApp.Notification.Domain.Interfaces;
using Capstone.ECommerceApp.Notification.Domain.Models;

namespace Capstone.ECommerceApp.Notification.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository notificationRepository;
    public NotificationService(INotificationRepository notificationRepository)
    {
        this.notificationRepository = notificationRepository;
    }

    public async Task RegisterUserEmailAndLog(string userEmail)
    {
        string message = "New user has registered with email address: " + userEmail;
        await LogAndEmail(message, "TGA-admin@gmail.com");
    }

    private async Task<bool> LogAndEmail(string message, string email) //message to be sent and send from email
    {
        try
        {
            await notificationRepository.LogEmail(message, email);
            //Email Logic
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
