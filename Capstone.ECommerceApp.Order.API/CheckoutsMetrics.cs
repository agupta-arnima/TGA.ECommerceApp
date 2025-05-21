using System.Diagnostics.Metrics;

namespace Capstone.ECommerceApp.Order.API
{
    public class CheckoutsMetrics
    {
        //Checkouts Meter
        private Counter<int> CheckoutsCounter { get; }

        public CheckoutsMetrics(IMeterFactory meterFactory)
        {
            var CheckoutsMeter = meterFactory.Create("capstone.checkouts.meter");
            CheckoutsCounter = CheckoutsMeter.CreateCounter<int>("capstone.checkouts.count", "orders", description: "Counts the current checkouts");  //Counter       
        }

        public void IncreaseCheckouts(string productName, int quantity) => CheckoutsCounter.Add(quantity, KeyValuePair.Create<string, object>("ProductName", productName));
    }
}