using FluentValidation;
using PMS.Application.Features.Companies.Commands;

namespace PMS.Application.Features.Companies.Validations;

public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator(ICompanyService companyService)
    {
        RuleFor(x => x.UpdateCompany)
            .SetValidator(new UpdateCompanyRequestValidator(companyService));
    }
}
