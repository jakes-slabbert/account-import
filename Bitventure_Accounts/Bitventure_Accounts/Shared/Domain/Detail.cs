namespace Bitventure_Accounts.Shared.Domain
{
    public class Detail
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }

        public string Status { get; set; }
        public string ReadableStatus { get { return Status == "00" ? "Successful" : Status == "30" ? "Disputed" : "Failed"; } }
        public DateTime EffectiveStatusDate { get; set; }
        public string TimeBreached { get { return (EffectiveStatusDate - TransactionDate).TotalDays >= 7 ? "Yes" : "No"; } }

        public Master Account { get; set; }

        private Detail()
        {
        }

        public Detail(int paymentId, DateTime transactionDate, decimal amount, string status, DateTime effectiveStatusDate, Master account)
        {
            PaymentId = paymentId;
            TransactionDate = transactionDate;
            Amount = amount;
            Status = status ?? throw new ArgumentNullException(nameof(status));
            EffectiveStatusDate = effectiveStatusDate;
            Account = account;
        }
    }
}
