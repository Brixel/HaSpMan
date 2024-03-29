﻿using Commands.Handlers.Transaction.AddAttachments;

using Domain.Interfaces;

namespace Commands.Handlers.Transaction.EditTransaction;

public class EditTransactionHandler : IRequestHandler<EditTransactionCommand>
{
    private readonly IFinancialYearRepository _financialYearRepository;
    private readonly IMediator _mediator;

    public EditTransactionHandler(IFinancialYearRepository financialYearRepository, IMediator mediator)
    {
        _financialYearRepository = financialYearRepository;
        _mediator = mediator;
    }

    public async Task Handle(EditTransactionCommand request, CancellationToken cancellationToken)
    {
        var financialYear = await _financialYearRepository.GetFinancialYearByTransactionId(request.Id, cancellationToken)
                ?? throw new ArgumentException($"No transaction found for Id {request.Id}", nameof(request));

        if (financialYear.StartDate <= request.ReceivedDateTime &&
           financialYear.EndDate >= request.ReceivedDateTime)
        {
            financialYear.ChangeTransaction(request.Id, request.CounterPartyName, request.MemberId, request.BankAccountId,
                request.ReceivedDateTime, request.TransactionTypeAmounts, request.Description);
        }
        else
        {
            var transaction = financialYear.Transactions.First(x => x.Id == request.Id);

            var matchingFinancialYear =
                await _financialYearRepository.GetByIdAsync(request.FinancialYearId, cancellationToken)
                    ?? throw new Exception("Could not get financial year specified in AddTransaction command");

            matchingFinancialYear.AddTransaction(transaction);
            matchingFinancialYear.ChangeTransaction(request.Id, request.CounterPartyName, request.MemberId, request.BankAccountId,
                request.ReceivedDateTime, request.TransactionTypeAmounts, request.Description);
            financialYear.Transactions.Remove(transaction);
        }

        await _financialYearRepository.SaveChangesAsync(cancellationToken);

        await _mediator.Send(new AddAttachmentsCommand(request.Id, request.NewAttachmentFiles), cancellationToken);

    }
}
