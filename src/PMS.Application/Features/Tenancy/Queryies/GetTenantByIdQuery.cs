using MediatR;
using PMS.Core.Wrappers;

namespace PMS.Application.Features.Tenancy.Queryies;

public class GetTenantByIdQuery : IRequest<IResponseWrapper>
{
    public string TenantId { get; set; }
}

public class GetTenantByIdQueryHandler(ITenantService tenantService) : IRequestHandler<GetTenantByIdQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenant = await tenantService.GetTenantByIdAsync(request.TenantId);
        if (tenant is not null)
        {
            return await ResponseWrapper<TenantResponse>.SuccessAsync(tenant);
        }

        return await ResponseWrapper<TenantResponse>.FailAsync("租户不存在。");
    }
}
