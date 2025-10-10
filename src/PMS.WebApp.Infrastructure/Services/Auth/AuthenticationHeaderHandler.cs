using Blazored.LocalStorage;
using PMS.WebApp.Infrastructure.Constants;

namespace PMS.WebApp.Infrastructure.Services.Auth;

public class AuthenticationHeaderHandler(ILocalStorageService localStorageService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Headers.Authorization?.Scheme != "Bearer")
            {
                var savedToken = await localStorageService.GetItemAsStringAsync(StorageConstants.AuthToken, cancellationToken);
                if (string.IsNullOrEmpty(savedToken))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", savedToken);
                }
            }
            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
