using Microsoft.EntityFrameworkCore;
using TGA.ECommerceApp.Notification.Domain.Models;

namespace TGA.ECommerceApp.Notification.Data.Context
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        public DbSet<EmailLogger> EmailLoggers { get; set; }
    }
}
