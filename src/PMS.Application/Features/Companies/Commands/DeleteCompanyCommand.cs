using MediatR;
using PMS.Core.Wrappers;

namespace PMS.Application.Features.Companies.Commands;

public class DeleteCompanyCommand : IRequest<IResponseWrapper>
{
    public int CompanyId { get; set; }
}

public class DeleteCompanyCommandHandler(ICompanyService companyService) : IRequestHandler<DeleteCompanyCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await companyService.GetByIdAsync(request.CompanyId);
        if (company is null)
        {
            return await ResponseWrapper<int>.FailAsync("公司不存在。");
        }

        var companyId = await companyService.DeleteAsync(company);
        return await ResponseWrapper<int>.SuccessAsync(companyId, "公司删除成功。");
    }
}

