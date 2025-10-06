using MediatR;
using PMS.Application.Pipelines;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Companies.Commands;

public class UpdateCompanyCommand : IRequest<IResponseWrapper>, IValidateSelf
{
    public UpdateCompanyRequest UpdateCompany { get; set; }
}

public class UpdateCompanyCommandHandler(ICompanyService companyService) : IRequestHandler<UpdateCompanyCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await companyService.GetByIdAsync(request.UpdateCompany.Id);
        if (company is not null)
        {
            company.Name = request.UpdateCompany.Name;
            company.EstablishedDate = request.UpdateCompany.EstablishedDate;

            var companyId = await companyService.UpdateAsync(company);
            return await ResponseWrapper<int>.SuccessAsync(companyId, "公司更新成功。");
        }

        return await ResponseWrapper<int>.FailAsync("公司更新失败。");
    }
}
