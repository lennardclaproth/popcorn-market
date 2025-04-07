using FluentValidation;
using PopcornMarket.FinancialAtlas.Contracts.Requests;

namespace PopcornMarket.FinancialAtlas.Api.Endpoints.V1.CreateCompany;

public class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty");
        RuleFor(x => x.Name).MaximumLength(255).WithMessage("Name cannot be more than 100 characters");
        
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty");
        RuleFor(x => x.Description).MaximumLength(1500).WithMessage("Description cannot be more than 1500 characters (around 250 words)");
        
        RuleFor(x => x.Ceo).NotEmpty().WithMessage("Ceo cannot be empty");
        RuleFor(x => x.Ceo).MaximumLength(255).WithMessage("Ceo cannot be more than 255 characters");

        RuleFor(x => x.Ticker)
            .Must(t => t.Length is >= 3 and <= 5)
            .WithMessage("Ticker must be between 3 and 5");
        
        RuleFor(x => x.Industry).NotEmpty().WithMessage("Industry cannot be empty");
        RuleFor(x => x.Industry).MaximumLength(100).WithMessage("Industry cannot be more than 255 characters");
        
        RuleFor(x => x.Headquarters).NotEmpty().WithMessage("Ceo cannot be empty");
        RuleFor(x => x.Headquarters).MaximumLength(255).WithMessage("Ceo cannot be more than 255 characters");

        RuleFor(x => x.FoundedYear)
            .Must(fy => fy <= DateTime.Now.Year)
            .WithMessage("Founded year cannot be in the future");
        
        RuleFor(x => x.Employees).NotEmpty().WithMessage("Employees cannot be empty");
        RuleFor(x => x.Employees).GreaterThan(1);
    }
}
