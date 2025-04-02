using TGA.ECommerceApp.Domain.Core.Events;

namespace TGA.ECommerceApp.Notification.Domain.Events
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
