using MediatR;
using PMS.Core.Wrappers;

namespace PMS.Application.Features.Tenancy.Commands;

public class UpdateTenantSubscriptionCommand : IRequest<IResponseWrapper>
{
    public UpdateTenantSubscriptionRequest UpdateTenantSubscription { get; set; }
}

public class UpdateTenantSubscriptionCommandHandler(ITenantService tenantService) : IRequestHandler<UpdateTenantSubscriptionCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(UpdateTenantSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = await tenantService.UpdateSubscriptionAsync(request.UpdateTenantSubscription);

        return await ResponseWrapper<string>.SuccessAsync(tenantId, "租户订阅更新成功。");
    }
}