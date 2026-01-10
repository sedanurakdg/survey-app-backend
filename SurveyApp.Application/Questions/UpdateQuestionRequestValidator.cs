using FluentValidation;
using SurveyApp.Core.Abstractions;

namespace SurveyApp.Application.Questions;

public sealed class UpdateQuestionRequestValidator : AbstractValidator<UpdateQuestionRequest>
{
    public UpdateQuestionRequestValidator(IAnswerTemplateRepository answerTemplateRepo)
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.AnswerTemplateId)
            .GreaterThan(0);
    }
}
