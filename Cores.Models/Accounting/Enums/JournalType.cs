namespace Cores.Models.Accounting.Enums;

public enum JournalType
{
    General,        // For miscellaneous entries
    Sales,          // For all sales transactions
    Purchases,      // For all purchase transactions
    Cash,           // For cash transactions
    Payroll,        // For salary and wage entries
    Adjusting       // For end-of-period adjustment
}