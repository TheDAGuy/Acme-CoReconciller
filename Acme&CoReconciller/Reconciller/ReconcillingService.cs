using Acme_CoReconciller.Utilities;

namespace Acme_CoReconciller.Reconciller
{
    public class ReconciliationService
    {
        public (List<(int invoiceId, int bankId)> matched, List<int> unmatchedInvoices, List<int> unmatchedBanks)  Reconcile(List<InvoiceStatement> invoices, List<BankStatement> banks)
        {
            var matched = new List<(int invoiceId, int bankId)>();
            var unmatchedInvoices = new List<int>();
            var unmatchedBanks = new List<int>();

            foreach (var bank in banks)
            {
                var matchinginvoices = invoices.Where(i => i.BankTransactionReferenceNumber == bank.BankTransactionReferenceNumber);
                if (matchinginvoices.Any())
                {
                    var InvoiceTotal = matchinginvoices.Sum(i => i.Amount);
                    if(InvoiceTotal ==  bank.Amount)
                    {
                        foreach (var invoice in matchinginvoices)
                        {
                            matched.Add((invoice.Id, bank.Id));
                        }
                    }
                }
                else
                {
                    unmatchedBanks.Add(bank.Id);
                }
            }
            var unmatchedInvoicesIds = invoices.Where( j => !matched.Select(m => m.invoiceId).Contains(j.Id)).Select(j => j.Id).ToList();
            unmatchedInvoices.AddRange(unmatchedInvoicesIds);

            return (matched, unmatchedInvoices, unmatchedBanks);
        }
    }
}
