using System.Diagnostics.Metrics;

namespace Capstone.ECommerceApp.ShoppingCart.API
{
    public class CartsMetrics
    {
        //Carts Meter
        private Counter<int> CartsCounter { get; }

        public CartsMetrics(IMeterFactory meterFactory)
        {
            var CartsMeter = meterFactory.Create("capstone.carts.meter");
            CartsCounter = CartsMeter.CreateCounter<int>("capstone.carts.count", "carts", description: "Counts the current Cart items");  //Counter       
        }

        public void IncreaseCarts(string productName, int quantity) => CartsCounter.Add(quantity, KeyValuePair.Create<string, object>("ProductName", productName));
    }
}