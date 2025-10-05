using FluentValidation;

namespace PMS.Application.Features.Companies.Validations;

internal class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("公司名称不能为空。")
            .MaximumLength(60)
                .WithMessage("公司名称字符长度不能超过60。");

        RuleFor(x => x.EstablishedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("建立日期不能是未来日期。");
    }
}
