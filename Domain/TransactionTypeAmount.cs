using Types;

namespace Domain;

public class TransactionTypeAmount : IEquatable<TransactionTypeAmount>
{
    public TransactionType TransactionType { get; set; }

    public decimal Amount { get; set; }

    public TransactionTypeAmount(
        TransactionType transactionType,
        decimal amount)
    {
        TransactionType = transactionType;
        Amount = amount;
    }

    public bool Equals(TransactionTypeAmount? other)
    {
        return other is not null && (ReferenceEquals(this, other) || (TransactionType == other.TransactionType && Amount == other.Amount));
    }

    public override bool Equals(object? obj)
    {
        return obj is not null && (ReferenceEquals(this, obj) || (obj.GetType() == GetType() && Equals((TransactionTypeAmount)obj)));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)TransactionType, Amount);
    }
};
