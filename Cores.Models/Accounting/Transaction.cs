namespace Cores.Models.Accounting;

public class Transaction
{
    public int Id { get; set; }
    public string ReferenceNo { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }

    public List<TransactionDetail> Details { get; set; } = [];

}