using MediatR;
using PMS.Core.Wrappers;

namespace PMS.Application.Features.Tenancy.Commands;

public class DeactivateTenantCommand : IRequest<IResponseWrapper>
{
    public string TenantId { get; set; }
}

public class DeactivateTenantCommandHandler(ITenantService tenantService) : IRequestHandler<DeactivateTenantCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(DeactivateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenantId = await tenantService.DeactivateAsync(request.TenantId);

        return await ResponseWrapper<string>.SuccessAsync(tenantId,"租户禁用成功。");
    }
}
