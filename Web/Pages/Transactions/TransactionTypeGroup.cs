using System.ComponentModel;

namespace Web.Pages.Transactions
{
    public enum TransactionTypeGroup
    {
        [Description("Debit")]
        Debit,
        [Description("Credit")]
        Credit
    }
}