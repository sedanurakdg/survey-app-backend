using FluentValidation;

namespace SurveyApp.Application.AnswerTemplates;

public sealed class CreateAnswerTemplateRequestValidator : AbstractValidator<CreateAnswerTemplateRequest>
{
    public CreateAnswerTemplateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Options)
            .NotNull()
            .Must(o => o.Count is >= 2 and <= 4)
            .WithMessage("Seçenek sayısı 2 ile 4 arasında olmalıdır.");

        RuleForEach(x => x.Options).ChildRules(opt =>
        {
            opt.RuleFor(o => o.Text).NotEmpty().MaximumLength(200);
            opt.RuleFor(o => o.SortOrder).InclusiveBetween((short)1, (short)4);
        });

        RuleFor(x => x.Options)
            .Must(o => o.Select(a => a.SortOrder).Distinct().Count() == o.Count)
            .WithMessage("SortOrder değerleri benzersiz olmalıdır.");
    }
}
