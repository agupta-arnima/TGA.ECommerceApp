using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.ECommerceApp.Notification.Domain.Interfaces;

public interface INotificationRepository
{
    public Task LogEmail(string message, string email);
}
