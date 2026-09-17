using FluentValidation;

namespace Commands.Handlers.BankAccount.EditBankAccount;

public record EditBankAccountCommand(
       Guid Id,
       string Name,
       string AccountNumber,
       string PerformingUser
    ) : IRequest<Guid>;

public class EditBankAccountValidator : AbstractValidator<EditBankAccountCommand>
{
    public EditBankAccountValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(34);
        RuleFor(x => x.PerformingUser)
            .NotEmpty();
    }
}