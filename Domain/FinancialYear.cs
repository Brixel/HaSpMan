﻿namespace Domain;

public class FinancialYear
{
#pragma warning disable CS8618
    public FinancialYear() { } // Make EFCore happy
#pragma warning restore CS8618

    public FinancialYear(DateTimeOffset startDate)
    {
        StartDate = startDate;
        EndDate = StartDate.AddYears(1).AddDays(-1);
        Transactions = new List<Transaction>();
    }

    public Guid Id { get; private set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; private set; }

    public bool IsClosed { get; private set; }

    public ICollection<Transaction> Transactions { get; private set; }

    public string Name => $"{StartDate.Year} - {EndDate.Year}";

    public void Close()
    {
        if (IsClosed)
        {
            throw new InvalidOperationException("Financial year is already closed");
        }

        IsClosed = true;

        foreach (var transaction in Transactions)
            transaction.SetAsReadOnly();
    }

    public void AddTransaction(Transaction transaction)
    {
        if (IsClosed)
        {
            throw new InvalidOperationException("Financial year is closed");
        }
        Transactions.Add(transaction);
    }

    public void ChangeTransaction(Guid transactionId, string counterPartyName, Guid? memberId, Guid bankAccountId,
        DateTimeOffset receivedDateTime, ICollection<TransactionTypeAmount> transactionTypeAmounts, string description)
    {
        if (IsClosed)
        {
            throw new InvalidOperationException("Financial year is already closed");
        }

        var transaction = Transactions.First(x => x.Id == transactionId);
        var totalAmount = transactionTypeAmounts.Sum(x => x.Amount);

        transaction.ChangeCounterParty(counterPartyName, memberId);
        transaction.ChangeBankAccountId(bankAccountId);
        transaction.ChangeReceivedDateTime(receivedDateTime);
        transaction.ChangeAmount(totalAmount, transactionTypeAmounts);
        transaction.ChangeDescription(description);
    }
}