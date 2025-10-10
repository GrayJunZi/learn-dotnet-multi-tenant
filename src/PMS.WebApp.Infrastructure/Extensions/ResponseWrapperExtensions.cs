using PMS.Core.Wrappers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace PMS.WebApp.Infrastructure.Extensions;

public static class ResponseWrapperExtensions
{
    public static async Task<IResponseWrapper<T>> WrapToResponse<T>(this HttpResponseMessage httpResponseMessage)
    {
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<ResponseWrapper<T>>(new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        return response;
    }

    public static async Task<IResponseWrapper> WrapToResponse(this HttpResponseMessage httpResponseMessage)
    {
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<ResponseWrapper>(new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        return response;
    }
}
