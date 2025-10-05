using Mapster;
using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Companies.Queries;

public class GetCompaniesQuery : IRequest<IResponseWrapper>
{

}

public class GetCompaniesQueryHandler(ICompanyService companyService) : IRequestHandler<GetCompaniesQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await companyService.GetAllAsync();
        if (companies?.Count > 0)
        {
            return await ResponseWrapper<List<CompanyResponse>>.SuccessAsync(companies.Adapt<List<CompanyResponse>>());
        }
        return await ResponseWrapper.FailAsync("公司不存在。");
    }
}
