using TGA.ECommerceApp.Domain.Core.Events;

namespace TGA.ECommerceApp.Auth.Domain.Events
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
