using Mapster;
using MediatR;
using PMS.Core.Wrappers;

namespace PMS.Application.Features.Companies.Queries;

public class GetCompanyByIdQuery : IRequest<IResponseWrapper>
{
    public int CompanyId { get; set; }
}

public class GetCompanyByIdQueryHandler(ICompanyService companyService) : IRequestHandler<GetCompanyByIdQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await companyService.GetByIdAsync(request.CompanyId);
        if (company is null)
        {
            return await ResponseWrapper.FailAsync("公司不存在。");
        }

        return await ResponseWrapper<CompanyResponse>.SuccessAsync(company.Adapt<CompanyResponse>());
    }
}
