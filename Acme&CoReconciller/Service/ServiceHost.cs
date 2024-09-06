using Acme_CoReconciller.Csv;
using Acme_CoReconciller.Google_Drive_Files;
using Acme_CoReconciller.Reconciller;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Acme_CoReconciller.Service
{
    public class ServiceHost : BackgroundService
    {
        private readonly Activity _driveservice;
        private readonly ReconciliationService _reconcillingservice;
        private readonly CsvActions _csvActions;

        public ServiceHost(Activity driveservice, ReconciliationService reconcillingservice, CsvActions csvActions)
        {
            _driveservice = driveservice;
            _reconcillingservice = reconcillingservice;
            _csvActions = csvActions;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ReconciliationService();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during service execution: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task ReconciliationService()
        {
            try
            {
                string InvoiceFileId = "199_fgJJxt0asX6oc4KHyU5cSL4MqTP3b";
                string BankFileId = "1Vp_gKybWR95zGsEMfb51vmsJDlL64F5k";

                var invoicename = "invoice" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".csv";
                var bankstatementname = "bankstatement" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".csv";

                var downloadedInvoiceFile = await _driveservice.DownloadFileAsync(InvoiceFileId, invoicename);
                var downloadedBankFile = await _driveservice.DownloadFileAsync(BankFileId, bankstatementname);

                var invoices = _csvActions.ReadInvoiceCsv(downloadedInvoiceFile);
                var banks = _csvActions.ReadBankCsv(downloadedBankFile);

                var (matched, unmatchedInvoices, unmatchedBanks) = _reconcillingservice.Reconcile(invoices, banks);

                var matchedFile = "matched.csv";
                _csvActions.WriteMatchedCsv(matchedFile, matched);
                await _driveservice.UploadFile(matchedFile, "text/csv");

                var unmatchedInvoicesFile = "unmatched_invoices.csv";
                _csvActions.WriteUnMatchedCsv(unmatchedInvoicesFile, unmatchedInvoices);
                await _driveservice.UploadFile(unmatchedInvoicesFile, "text/csv");

                var unmatchedBanksFile = "unmatched_banks.csv";
                _csvActions.WriteUnMatchedBankCsv(unmatchedBanksFile, unmatchedBanks);
                await _driveservice.UploadFile(unmatchedBanksFile, "text/csv");

                Console.WriteLine("Reconciliation process completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in reconciliation service: {ex.Message}");
                throw;
            }
        }
    }
}
