using System;
using TGA.ECommerceApp.Notification.Application.Interfaces;
using TGA.ECommerceApp.Notification.Domain.Interfaces;
using TGA.ECommerceApp.Notification.Domain.Models;

namespace TGA.ECommerceApp.Notification.Application.Services
{
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
            await LogAndEmail(message, "dotnetmastery@gmail.com");
        }

        private async Task<bool> LogAndEmail(string message, string email) //message to be sent and send to email
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
}
