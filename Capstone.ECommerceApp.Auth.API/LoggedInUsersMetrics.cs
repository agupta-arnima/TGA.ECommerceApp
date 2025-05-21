using System.Diagnostics.Metrics;

namespace Capstone.ECommerceApp.Auth.API
{
    public class LoggedInUsersMetrics
    {
        //Users Meter
        private UpDownCounter<int> LoggedInUsersUpDownCounter { get; }

        public LoggedInUsersMetrics(IMeterFactory meterFactory)
        {
            var loggedInUsersMeter = meterFactory.Create("capstone.loggedin.users.meter");
            LoggedInUsersUpDownCounter = loggedInUsersMeter.CreateUpDownCounter<int>("capstone.loggedin.users.count", "users", description: "Counts the number of logged in Users");  //UpDownCounter       
        }

        public void IncreaseLoggedinUsers() => LoggedInUsersUpDownCounter.Add(1);
        public void DecreaseLoggedinUsers() => LoggedInUsersUpDownCounter.Add(-1);
    }
}