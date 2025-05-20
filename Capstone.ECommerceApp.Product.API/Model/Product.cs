using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Indexes;
using System.Text;

namespace Capstone.ECommerceApp.Product.API.Model
{
    public partial class AIProduct
    {
        [SimpleField(IsKey = true, IsFilterable = true)]
        public string ProductId { get; set; }

        [SearchableField(IsSortable = true)]
        public string ProductName { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string Description { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public string Category { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public string[] Tags { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public double? Price { get; set; }
    }

    public partial class AIProduct
    {
        // This implementation of ToString() is only for the purposes of the sample console application.
        // You can override ToString() in your own model class if you want, but you don't need to in order
        // to use the Azure Search .NET SDK.
        public override string ToString()
        {
            var builder = new StringBuilder();

            if (!String.IsNullOrEmpty(ProductId))
            {
                builder.AppendFormat("ProductId: {0}\n", ProductId);
            }

            if (!String.IsNullOrEmpty(ProductName))
            {
                builder.AppendFormat("Name: {0}\n", ProductName);
            }

            if (!String.IsNullOrEmpty(Description))
            {
                builder.AppendFormat("Description: {0}\n", Description);
            }

            if (!String.IsNullOrEmpty(Category))
            {
                builder.AppendFormat("Category: {0}\n", Category);
            }

            if (Tags != null && Tags.Length > 0)
            {
                builder.AppendFormat("Tags: [ {0} ]\n", String.Join(", ", Tags));
            }

            if (Price.HasValue)
            {
                builder.AppendFormat("Price: {0}\n", Price);
            }

            return builder.ToString();
        }
    }
}
