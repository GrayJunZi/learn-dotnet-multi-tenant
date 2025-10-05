using PMS.Domain.Entities;

namespace PMS.Application.Features.Companies;

public interface ICompanyService
{
    Task<int> CreateAsync(Company company);
    Task<int> UpdateAsync(Company company);
    Task<int> DeleteAsync(Company company);
    Task<Company> GetByIdAsync(int companyId);
    Task<Company> GetByNameAsync(string companyName);
    Task<List<Company>> GetAllAsync();
}
