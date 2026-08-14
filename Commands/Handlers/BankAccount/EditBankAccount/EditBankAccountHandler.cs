using Domain.Interfaces;

namespace Commands.Handlers.BankAccount.EditBankAccount;

public class EditBankAccountHandler : IRequestHandler<EditBankAccountCommand, Guid>
{
    private readonly IBankAccountRepository _repository;

    public EditBankAccountHandler(IBankAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(EditBankAccountCommand request, CancellationToken cancellationToken)
    {
        var bankAccount = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ArgumentException($"No bankaccount found for Id {request.Id}", nameof(request.Id));

        bankAccount.ChangeName(request.Name, request.PerformingUser);
        bankAccount.ChangeAccountNumber(request.AccountNumber, request.PerformingUser);

        await _repository.SaveAsync(cancellationToken);
        return bankAccount.Id;
    }
}