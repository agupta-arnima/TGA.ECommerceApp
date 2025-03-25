using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TGA.ECommerceApp.Auth.API;

public class RequireCertificateAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var clientCertificate = context.HttpContext.Connection.ClientCertificate;
        if (clientCertificate == null)
        {
            context.Result = new ForbidResult();
        }
    }
}
