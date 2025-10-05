using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Tenancy.Queryies;

public class GetTenantsQuery : IRequest<IResponseWrapper>
{
}

public class GetTenantsQueryHandler(ITenantService tenantService) : IRequestHandler<GetTenantsQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        var tenants = await tenantService.GetTenantsAsync();
        return await ResponseWrapper<List<TenantResponse>>.SuccessAsync(tenants);
    }
}
