using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Capstone.ECommerceApp.Infra.Common;
using Capstone.ECommerceApp.Product.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.ECommerceApp.Product.API.Controllers;

[Route("api/product-ai")]
[ApiController]
public class ProductAPIAIController : ControllerBase
{
    private readonly SearchClient _searchClient;
    private readonly SearchIndexClient _searchIndexClient;
    private readonly IConfiguration _configuration;
    public ProductAPIAIController(SearchClient searchClient,
                                  SearchIndexClient searchIndexClient,
                                  IConfiguration configuration)
    {
        _searchClient = searchClient;
        _searchIndexClient = searchIndexClient;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAll()
    {
        var options = new SearchOptions
        {
            IncludeTotalCount = true
        };
        options.Select.Add("ProductId");
        options.Select.Add("ProductName");
        options.Select.Add("Price");
        options.Select.Add("Description");
        options.Select.Add("Category");
        options.Select.Add("Tags");

        var response = await _searchClient.SearchAsync<AIProduct>("*", options);
        return Ok(response.Value.GetResults().Select(r => r.Document));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SerachByFilter([FromQuery] string filter, [FromQuery] string orderby)
    {
        var options = new SearchOptions 
        { 
            Filter = filter, 
            OrderBy = { orderby },
            IncludeTotalCount = true
        };        
        options.Select.Add("ProductId");
        options.Select.Add("ProductName");
        options.Select.Add("Price");
        options.Select.Add("Description");                
        options.Select.Add("Category");
        options.Select.Add("Tags");

        var response = await _searchClient.SearchAsync<AIProduct>("*", options);
        return Ok(response.Value.GetResults().Select(r => r.Document));
    }

    [HttpGet("searchbytag")]
    public async Task<IActionResult> SearchByTag([FromQuery] string tag)
    {
        var options = new SearchOptions
        {
            SearchFields = { "Tags" }
        };
        options.Select.Add("ProductId");
        options.Select.Add("ProductName");
        options.Select.Add("Price");
        options.Select.Add("Description");
        options.Select.Add("Category");
        options.Select.Add("Tags");

        var response = await _searchClient.SearchAsync<AIProduct>(tag, options);
        return Ok(response.Value.GetResults().Select(r => r.Document));
    }

    [HttpGet("suggestions")]
    public async Task<IActionResult> Suggestions([FromQuery] string query)
    {
        var suggestOptions = new SuggestOptions { UseFuzzyMatching = true, Size = 5 };
        var suggestions = await _searchClient.SuggestAsync<AIProduct>(query, "sg", suggestOptions);
        return Ok(suggestions.Value.Results.Select(s => s.Text));
    }

    [HttpGet("fuzzy")]
    public async Task<IActionResult> FuzzySearch([FromQuery] string query)
    {
        var options = new SearchOptions()
        {
            SearchMode = SearchMode.Any,
            QueryType = SearchQueryType.Full,
            IncludeTotalCount = true,
            SearchFields = { "ProductName, Description" }
        };

        options.Select.Add("ProductId");
        options.Select.Add("ProductName");
        options.Select.Add("Price");
        options.Select.Add("Description");
        options.Select.Add("Category");
        options.Select.Add("Tags");

        var response = await _searchClient.SearchAsync<AIProduct>(query, options);

        var results = response.Value.GetResults().Select(r => new {
            r.Document.ProductId,
            r.Document.ProductName,
            r.Document.Price,
            r.Document.Description,
            r.Document.Category,
            r.Document.Tags
        });
        return Ok(results);
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedData()
    {
        string indexName = _configuration[$"{KeyVaultConfig.SecretPrefix}-ai-search-index"];
        // Create index
        CreateIndex(indexName, _searchIndexClient);

        SearchClient ingesterClient = _searchIndexClient.GetSearchClient(indexName);

        var products = new List<AIProduct>
        {
             new AIProduct
             {
                 ProductId = "1",
                 ProductName = "Apple",
                 Price = 15,
                 Description = "Apples are very nutritious, packed with vitamin C, antioxidants, and fiber.",
                 Category = "Fruits",
                 Tags = new[] { "nutritious", "antioxidants", "fiber" }
             },
             new AIProduct
             {
                 ProductId = "2",
                 ProductName = "Orange",
                 Price = 13.99,
                 Description = "Oranges are a popular citrus fruit, rich in vitamin C and other nutrients.",
                 Category = "Fruits",
                 Tags = new[] { "citrus fruit" }
             },
             new AIProduct
             {
                 ProductId = "3",
                 ProductName = "Banana",
                 Price = 10.99,
                 Description = "Bananas are a good source of nutrients and are consumed worldwide.",
                 Category = "Fruits",
                 Tags = new[] { "berry", "nutrients", "nutritious" }
             }
        };

        try
        {
            var batch = IndexDocumentsBatch.Upload(products);
            var result = await _searchClient.IndexDocumentsAsync(batch);
            return Ok("Seed data uploaded successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error uploading seed data: {ex.Message}");
        }
    }

    private static void DeleteIndexIfExists(string indexName, SearchIndexClient adminClient)
    {
        adminClient.GetIndexNames();
        {
            adminClient.DeleteIndex(indexName);
        }
    }
    
    // Create tga-catalog-products index
    private static void CreateIndex(string indexName, SearchIndexClient adminClient)
    {
        FieldBuilder fieldBuilder = new FieldBuilder();
        var searchFields = fieldBuilder.Build(typeof(AIProduct));

        var definition = new SearchIndex(indexName, searchFields);

        var suggester = new SearchSuggester("sg", new[] { "ProductName", "Category" });
        definition.Suggesters.Add(suggester);

        adminClient.CreateOrUpdateIndex(definition);
    }
}