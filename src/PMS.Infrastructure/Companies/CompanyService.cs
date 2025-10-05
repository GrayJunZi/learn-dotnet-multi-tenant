using Microsoft.EntityFrameworkCore;
using PMS.Application.Features.Companies;
using PMS.Domain.Entities;
using PMS.Infrastructure.Contexts;

namespace PMS.Infrastructure.Companies;

public class CompanyService(ApplicationDbContext applicationDbContext) : ICompanyService
{
    public async Task<int> CreateAsync(Company company)
    {
        await applicationDbContext.Companies.AddAsync(company);
        await applicationDbContext.SaveChangesAsync();
        return company.Id;
    }

    public async Task<int> UpdateAsync(Company company)
    {
        applicationDbContext.Companies.Update(company);
        await applicationDbContext.SaveChangesAsync();
        return company.Id;
    }

    public async Task<int> DeleteAsync(Company company)
    {
        applicationDbContext.Companies.Remove(company);
        await applicationDbContext.SaveChangesAsync();
        return company.Id;
    }

    public async Task<Company> GetByIdAsync(int companyId)
    {
        return await applicationDbContext.Companies
            .FirstOrDefaultAsync(x => x.Id == companyId);
    }

    public async Task<Company> GetByNameAsync(string companyName)
    {
        return await applicationDbContext.Companies
            .FirstOrDefaultAsync(x => x.Name == companyName);
    }

    public async Task<List<Company>> GetAllAsync()
    {
        return await applicationDbContext.Companies.ToListAsync();
    }
}
