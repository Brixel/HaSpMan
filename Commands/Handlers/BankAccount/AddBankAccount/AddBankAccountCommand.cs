using FluentValidation;

namespace Commands.Handlers.BankAccount.AddBankAccount;

public record AddBankAccountCommand(
    string Name,
    string AccountNumber,
    string PerformingUser
) : IRequest<Guid>;

public class AddAccountCommandValidator : AbstractValidator<AddBankAccountCommand>
{
    public AddAccountCommandValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(34);

        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.PerformingUser)
            .NotEmpty();
    }
}
