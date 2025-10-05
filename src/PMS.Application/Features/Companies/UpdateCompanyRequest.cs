namespace PMS.Application.Features.Companies;

public class UpdateCompanyRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime EstablishedDate { get; set; }
}
