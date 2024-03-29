﻿using Domain;
using Domain.Interfaces;

using FluentValidation;

namespace Commands.Handlers.Transaction.AddAttachments;

public record AddAttachmentsCommand(Guid TransactionId, ICollection<AttachmentFile> Attachments) : IRequest<Unit>;

public class AddAttachmentsCommandValidator : AbstractValidator<AddAttachmentsCommand>
{
    public AddAttachmentsCommandValidator()
    {
        RuleFor(x => x.TransactionId).NotEmpty();
        RuleForEach(x => x.Attachments).SetValidator(new AttachmentValidator());
    }
}

public class AttachmentValidator : AbstractValidator<AttachmentFile>
{
    public AttachmentValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.ContentType).NotEmpty();
        RuleFor(x => x.UnsafePath).NotEmpty();
    }
}

public class AddAttachmentsHandler : IRequestHandler<AddAttachmentsCommand, Unit>
{
    private readonly IFinancialYearRepository _financialYearRepository;
    private readonly IAttachmentStorage _attachmentStorage;

    public AddAttachmentsHandler(IFinancialYearRepository financialYearRepository, IAttachmentStorage attachmentStorage)
    {
        _financialYearRepository = financialYearRepository;
        _attachmentStorage = attachmentStorage;
    }

    public async Task<Unit> Handle(AddAttachmentsCommand request, CancellationToken cancellationToken)
    {
        var financialYear = await _financialYearRepository.GetFinancialYearByTransactionId(request.TransactionId, cancellationToken)
            ?? throw new ArgumentException($"No financial year found for transactionId {request.TransactionId}", nameof(request.TransactionId));

        var transaction = financialYear.Transactions.First(x => x.Id == request.TransactionId);

        var transactionAttachments = await StoreAttachmentsAsync(request, cancellationToken);

        transaction.AddAttachments(transactionAttachments);

        await _financialYearRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task<List<TransactionAttachment>> StoreAttachmentsAsync(AddAttachmentsCommand request, CancellationToken cancellationToken)
    {
        var transactionAttachments = new List<TransactionAttachment>();
        var attachments = request.Attachments;
        foreach (var attachment in attachments)
        {
            var transactionAttachment = new TransactionAttachment(request.TransactionId, attachment.FileName);
            await using FileStream fs = new(attachment.UnsafePath, FileMode.Open);
            using var memoryStream = new MemoryStream();
            await fs.CopyToAsync(memoryStream, cancellationToken);
            await _attachmentStorage.AddAsync(transactionAttachment.FullPath, attachment.ContentType, memoryStream.ToArray(), cancellationToken);

            transactionAttachments.Add(transactionAttachment);
        }

        return transactionAttachments;
    }
}