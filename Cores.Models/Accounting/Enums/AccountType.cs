namespace Cores.Models.Accounting.Enums;

public enum AccountType
{
    Asset,      // Debit increases, Credit decreases
    Liability,  // Credit increases, Debit decreases
    Equity,     // Credit increases, Debit decreases
    Revenue,    // Credit increases, Debit decreases
    Expense     // Debit increases, Credit decreases
}