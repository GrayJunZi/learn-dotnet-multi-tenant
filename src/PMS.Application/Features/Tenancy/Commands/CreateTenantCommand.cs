using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Tenancy.Commands;

public class CreateTenantCommand : IRequest<IResponseWrapper>
{
    public CreateTenantRequest CreateTenant { get; set; }
}

public class CreateTenantCommandHandler(ITenantService tenantService) : IRequestHandler<CreateTenantCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenantId = await tenantService.CreateTenantAsync(request.CreateTenant, cancellationToken);

        return await ResponseWrapper<string>.SuccessAsync(tenantId, "租户创建成功。");
    }
}