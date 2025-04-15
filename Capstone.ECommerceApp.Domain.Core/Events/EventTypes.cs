namespace Capstone.ECommerceApp.Domain.Core.Events;

public enum EventTypes
{
    UserRegistration = 0,
    OrderCreated = 1,
    OrderUpdated = 2,
    OrderFailed = 3,
    PaymentConfirmed = 4,
    PaymentRejected = 5
}
