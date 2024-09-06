using Acme_CoReconciller.Utilities;
using System.Globalization;
using CsvHelper;

namespace Acme_CoReconciller.Csv
{
    public class CsvActions
    {
        public List<InvoiceStatement> ReadInvoiceCsv(string FilePath)
        {
            using (var reader = new StreamReader(FilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<InvoiceStatement>().ToList();
                return records;
            }
        }   

        public List<BankStatement> ReadBankCsv(string FilePath)
        {
            using (var reader = new StreamReader(FilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<BankStatement>().ToList();
                return records;
            }
        }

        public void WriteMatchedCsv(string FilePath, List<(int invoiceId, int bankId)> matched)
        {
            using(var writer = new StreamWriter(FilePath))
            using(var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(matched.Select(m => new { InvoiceId = m.invoiceId, BankId = m.bankId }));
            }
        }

        public void WriteUnMatchedCsv(string FilePath, List<int> unmatchedInvoices)
        {
            using (var writer = new StreamWriter(FilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(unmatchedInvoices.Select(id => new { InvoiceId = id }));
            }
        }

        public void WriteUnMatchedBankCsv(string FilePath, List<int> unmatchedBanks)
        {
            using (var writer = new StreamWriter(FilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(unmatchedBanks.Select(id => new { BankId = id }));
            }
        }
    }
}
