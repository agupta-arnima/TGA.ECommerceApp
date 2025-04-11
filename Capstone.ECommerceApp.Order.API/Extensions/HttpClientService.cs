using Polly;

namespace Capstone.ECommerceApp.Order.API.Extensions;

public static class HttpClientService
{
    public static void AddHttpClientService(this IServiceCollection services,
                                                 string serviceName, 
                                                 string serviceUrl, 
                                                 Func<IServiceProvider, DelegatingHandler> handlerFactory,
                                                 IAsyncPolicy<HttpResponseMessage> policy)
    {
        services.AddHttpClient(serviceName, c => c.BaseAddress = new Uri(serviceUrl))
        .AddHttpMessageHandler(handlerFactory);
        //.AddPolicyHandler(policy);
    }
}
