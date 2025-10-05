using FluentValidation;
using PMS.Application.Features.Companies.Commands;

namespace PMS.Application.Features.Companies.Validations;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.CreateCompany)
            .SetValidator(new CreateCompanyRequestValidator());
    }
}
