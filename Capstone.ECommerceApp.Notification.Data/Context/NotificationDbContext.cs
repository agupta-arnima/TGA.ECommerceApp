using Microsoft.EntityFrameworkCore;
using Capstone.ECommerceApp.Notification.Domain.Models;

namespace Capstone.ECommerceApp.Notification.Data.Context;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<EmailLogger> EmailLoggers { get; set; }
}
