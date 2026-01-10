using FluentValidation;

namespace SurveyApp.Application.Surveys;

public sealed class CreateSurveyRequestValidator : AbstractValidator<CreateSurveyRequest>
{
    public CreateSurveyRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);

        RuleFor(x => x.StartsAtUtc)
            .NotEmpty();

        RuleFor(x => x.EndsAtUtc)
            .NotEmpty()
            .GreaterThan(x => x.StartsAtUtc)
            .WithMessage("EndsAtUtc, StartsAtUtc'tan büyük olmalıdır.");

        RuleFor(x => x.QuestionIds)
            .NotNull()
            .Must(x => x!.Count > 0)
            .WithMessage("En az 1 soru seçilmelidir.");
    }
}
