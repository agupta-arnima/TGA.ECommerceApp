
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Capstone.ECommerceApp.Infra.Common.Configuration.AzureKeyVault;

public static class KeyVaultConfiguration
{
    public static void AddAzureKeyVaultIfConfigured(HostBuilderContext context,
                                                    IConfigurationBuilder config)
    {
        var builtConfig = config.Build();
        var keyVaultUri = builtConfig["AzureConfiguration:AzureKeyVault:VaultUri"];

        if (!string.IsNullOrEmpty(keyVaultUri))
        {
            try
            {
                var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = builtConfig["AKS_MANAGED_IDENTITY_CLIENT_ID"]
                });

                var secretClient = new SecretClient(new Uri(keyVaultUri), credential);
                config.AddAzureKeyVault(secretClient,
                        new KeyVaultSecretManager());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load Azure Key Vault. Using local settings. Error: {ex.Message}");
            }
        }
    }
}
