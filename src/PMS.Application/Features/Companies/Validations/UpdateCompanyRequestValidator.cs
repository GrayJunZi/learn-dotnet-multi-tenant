using FluentValidation;
using PMS.Domain.Entities;

namespace PMS.Application.Features.Companies.Validations;

internal class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyRequestValidator(ICompanyService companyService)
    {
        RuleFor(x => x.Id)
            .LessThanOrEqualTo(0)
            .MustAsync(async (id, ct) => await companyService.GetByIdAsync(id) is Company company && company.Id == id)
                .WithMessage("公司不存在。");

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
