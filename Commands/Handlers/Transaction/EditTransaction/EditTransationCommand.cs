using Commands.Handlers.Transaction.AddAttachments;

using Domain;

using FluentValidation;

using MediatR;

namespace Commands.Handlers.Transaction.EditTransaction;

public record EditTransactionCommand(
    Guid Id,
    string CounterPartyName,
    Guid? MemberId,
    Guid BankAccountId,
    DateTimeOffset ReceivedDateTime,
    string Description,
    ICollection<TransactionTypeAmount> TransactionTypeAmounts, 
    ICollection<AttachmentFile> AttachmentFiles) : IRequest<Guid>;


public class EditTransactionCommandValidator : AbstractValidator<EditTransactionCommand>
{
    public EditTransactionCommandValidator()
    {
        RuleFor(x => x.CounterPartyName)
            .MaximumLength(120);
        RuleFor(x => x.Description)
            .MaximumLength(1000);
        RuleFor(x => x.BankAccountId)
            .NotEmpty();
        When(x => x.MemberId != null, () =>
        {
            RuleFor(x => x.MemberId)
                .NotEmpty();
        });

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.ReceivedDateTime)
            .LessThanOrEqualTo(x => DateTimeOffset.Now);
        RuleForEach(x => x.TransactionTypeAmounts)
            .SetValidator(new TransactionTypeValidator());

        RuleForEach(x => x.AttachmentFiles)
            .SetValidator(new AttachmentValidator());

    }
}

