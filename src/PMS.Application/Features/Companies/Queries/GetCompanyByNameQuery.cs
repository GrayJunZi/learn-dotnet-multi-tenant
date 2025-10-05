using Mapster;
using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Companies.Queries;

public class GetCompanyByNameQuery : IRequest<IResponseWrapper>
{
    public string CompanyName { get; set; }
}

public class GetCompanyByNameQueryHandler(ICompanyService companyService) : IRequestHandler<GetCompanyByNameQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetCompanyByNameQuery request, CancellationToken cancellationToken)
    {
        var company = await companyService.GetByNameAsync(request.CompanyName);
        if (company is null)
        {
            return await ResponseWrapper.FailAsync("公司不存在。");
        }

        return await ResponseWrapper<CompanyResponse>.SuccessAsync(company.Adapt<CompanyResponse>());
    }
}
