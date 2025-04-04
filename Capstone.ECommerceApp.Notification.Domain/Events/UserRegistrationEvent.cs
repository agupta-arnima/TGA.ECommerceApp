using Capstone.ECommerceApp.Domain.Core.Events;

namespace Capstone.ECommerceApp.Notification.Domain.Events
{
    public class UserRegistrationEvent : Event
    {
        public string Email { get; protected set; }
        public UserRegistrationEvent(string email)
        {
            Email = email;
        }
    }
}
