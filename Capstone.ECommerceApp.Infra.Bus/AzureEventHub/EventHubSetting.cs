namespace Capstone.ECommerceApp.Infra.Bus;

public class EventHubSetting
{
    public string? ConnectionString { get; set; }
    public string? EventHubName { get; set; }
    public string? StorageConnectionString { get; set; }
    public string? BlobContainerName { get; set; }

}
