namespace Bitventure_Accounts.Shared.Domain
{
    public class Master
    {
        public int Id { get; set; }
        public string AccountHolder { get; set; }
        public string BranchCode { get; set; }
        public string AccountNumber { get; set; }
        public AccountType AccountType { get; set; }

        public ICollection<Detail> Details { get; set; }

        private Master()
        {
        }

        public Master(string accountHolder, string branchCode, string accountNumber, AccountType accountType)
        {
            AccountHolder = accountHolder;
            BranchCode = branchCode;
            AccountNumber = accountNumber;
            AccountType = accountType;
        }
    }

    public enum AccountType
    {
        Cheque = 1,
        Savings = 2,
    }
}
