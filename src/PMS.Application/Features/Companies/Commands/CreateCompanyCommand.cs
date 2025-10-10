using Mapster;
using MediatR;
using PMS.Application.Pipelines;
using PMS.Core.Wrappers;
using PMS.Domain.Entities;

namespace PMS.Application.Features.Companies.Commands;

public class CreateCompanyCommand : IRequest<IResponseWrapper>, IValidateSelf
{
    public CreateCompanyRequest CreateCompany { get; set; }
}

public class CreateCompanyCommandHandler(ICompanyService companyService) : IRequestHandler<CreateCompanyCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = request.CreateCompany.Adapt<Company>();
        var companyId = await companyService.CreateAsync(company);

        return await ResponseWrapper<int>.SuccessAsync(companyId, "公司创建成功。");
    }
}
