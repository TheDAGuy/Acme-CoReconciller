namespace Acme_CoReconciller.Utilities
{
    public class InvoiceStatement
    {
        public int Id {get; set;}
        public int InvoiceNumber { get; set; }
        public string BankTransactionReferenceNumber { get; set; }
        public string BankName { get; set; }
        public decimal Amount { get; set; }
        public int BankStatementId { get; set; }
        public BankStatement BankStatement { get; set; }
    }
}
