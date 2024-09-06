namespace Acme_CoReconciller.Utilities
{
    public class BankStatement
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string BankTransactionReferenceNumber {  get; set; }
        public string BankName {  get; set; }
        public decimal Amount { get; set; }
        public ICollection<InvoiceStatement> InvoiceStatements { get; set; }

    }
}
