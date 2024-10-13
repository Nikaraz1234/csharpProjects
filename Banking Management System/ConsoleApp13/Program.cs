using System;
using System.Collections.Generic;

namespace ConsoleApp13
{

    public abstract class Account
    {
        private static int _nextTransactionId = 1;
        public int AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public decimal Balance { get; set; }

        public DateTime CreatedDate { get; set; }
        public List<Transaction> TransactionHistory { get; set; }

        public Account(int accountNumber, string accountHolder, decimal balance)
        {
            AccountNumber = accountNumber;
            AccountHolderName = accountHolder;
            Balance = balance;
            CreatedDate = DateTime.Now;
            TransactionHistory = new List<Transaction>();
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
            TransactionHistory.Add(new Transaction(GetNextTransactionId(), TransactionType.Deposit, amount, this, null, "Deposit"));

            Console.WriteLine("Deposited Successfuly.");
        }

        public virtual void Withdraw(decimal amount)
        {
            if(amount <= Balance)
            {
                Balance -= amount;
                TransactionHistory.Add(new Transaction(GetNextTransactionId(), TransactionType.Withdrawal, amount, this, null, "Withdrawal"));
                Console.WriteLine("Successful withdrawal.");
            }
            else
            {
                Console.WriteLine("Insuficcient Funds.");
            }
        }
        public void Transfer(Account targetAcc, decimal amount)
        {
            if(amount <= Balance)
            {
                Balance -= amount;
                targetAcc.Balance += amount;
                TransactionHistory.Add(new Transaction(GetNextTransactionId(), TransactionType.Transfer, amount, this, targetAcc, "Transfer to Account " + targetAcc.AccountNumber));
                Console.WriteLine("Successful Transfer.");

                Console.WriteLine("Successful Transfer.");
            }
            else
            {
                Console.WriteLine("Insuficcient Funds.");
            }
        }
        public void getTransactionHistory()
        {
            foreach(Transaction transaction in TransactionHistory)
            {
                Console.WriteLine($"{transaction.TransactionID}, {transaction.TransactionDate}");
            }
        }
        public static int GetNextTransactionId()
        {
            return _nextTransactionId++;
        }
    }

    public class SavingAccount : Account
    {
        public decimal InterestRate { get; set; }
        public decimal MinimumBalance { get; set; }

        public SavingAccount(int accountNumber, string accountHolder, decimal balance, decimal interestRate, decimal minimumBalance) :base(accountNumber, accountHolder, balance)
        {
            InterestRate = interestRate;
            MinimumBalance = minimumBalance;
        }

        public void ApplyInterest()
        {
            Balance += (Balance * InterestRate);
            Console.WriteLine("Interest applied.");
        }
        public override void Withdraw(decimal amount)
        {
            if(Balance-amount > MinimumBalance)
            {
                Balance -= amount;
                Console.WriteLine("Successful withdrawal.");
            }
            else
            {
                Console.WriteLine("Insufficient Funds.");
            }
        }
    }
    public class CheckingAccount : Account
    {
        public decimal OverdraftLimit { get; set; }
        

        public CheckingAccount(int accountNumber, string accountHolder, decimal balance, decimal overdraftLimit) : base(accountNumber, accountHolder, balance)
        {
            OverdraftLimit = overdraftLimit;
        }

        public override void Withdraw(decimal amount)
        {
            if(Balance-amount > OverdraftLimit)
            {
                Balance -= amount;
                Console.WriteLine("Successful withdrawal.");
            }
            else
            {
                Console.WriteLine("Insufficient Funds.");
            }
        }
    }

    public class Transaction
    {
        
        public int TransactionID { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public Account SourceAccount { get; set; }
        public Account TargetAccount { get; set; }
        public string Description { get; set; }

        public Transaction(int transactionId, TransactionType type, decimal amount, Account sourceAccount, Account targetAccount, string description)
        {
            TransactionID = transactionId;
            Type = type;
            Amount = amount;
            TransactionDate = DateTime.Now;
            SourceAccount = sourceAccount;
            TargetAccount = targetAccount;
        }

        public override string ToString()
        {
            return $"{TransactionDate}: {Type} of {Amount:C} from {SourceAccount.AccountNumber}" +
                   (TargetAccount != null ? $" to {TargetAccount.AccountNumber}" : "");
        }
        
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer
    }

    public class Bank
    {
        public List<Account> Accounts { get; set; }
        public string BankName { get; set; }
        public decimal TotalAssets { get; set; }
        public List<Transaction> Transactions { get; set; }

        public Bank(string bankName)
        {
            Accounts = new List<Account>();
            BankName = bankName;
            Transactions = new List<Transaction>();
            TotalAssets = TotalAsset();
        }

        public void CreateSavingsAccount(int accountNumber, string accountHolder, decimal initialBalance, decimal interestRate, decimal minimumBalance)
        {
            Accounts.Add(new SavingAccount(accountNumber, accountHolder, initialBalance, interestRate, minimumBalance));
            Console.WriteLine("Saving account created successfuly");
        }
        public void CreateCheckingAccount(int accountNumber, string accountHolder, decimal initialBalance, decimal overdraftLimit)
        {
            Accounts.Add(new CheckingAccount(accountNumber, accountHolder, initialBalance, overdraftLimit));
            Console.WriteLine("Checking account created successfuly");
        }
        public void ListAccount()
        {
            foreach(Account account in Accounts)
            {
                Console.WriteLine($"{account.AccountNumber} - {account.AccountHolderName}, Balance - {account.Balance} ");
                if(account is SavingAccount)
                {
                    Console.Write("Saving account");
                }
                else
                {
                    Console.Write("Checking account");
                }
            }
        }
        public Account findAccount(int accountNumber)
        {
            Account accountToFind = Accounts.Find(a => a.AccountNumber == accountNumber);
            return accountToFind;
        }
        public void Deposit(int accountNumber, decimal amount)
        {
            Account account = Accounts.Find(a => a.AccountNumber == accountNumber);
            account.Deposit(amount);

        }
        public void Withdraw(int accountNumber, decimal amount)
        {
            Account account = Accounts.Find(a => a.AccountNumber == accountNumber);
            account.Withdraw(amount);

        }
        public void Transfer(int fromAccountNumber, int toAccountNumber, decimal amount)
        {
            Account sourceAccount = Accounts.Find(a => a.AccountNumber == fromAccountNumber);
            Account targetAccount = Accounts.Find(a => a.AccountNumber == toAccountNumber);

            sourceAccount.Transfer(targetAccount, amount);
        }
        public void ListTransactions(int accountNumber)
        {
            Account account = Accounts.Find(a => a.AccountNumber == accountNumber);
            account.getTransactionHistory();
        }
        public decimal TotalAsset()
        {
            decimal sum = 0;
            foreach(Account account in Accounts)
            {
                sum += account.Balance;
            }
            return sum;
        }
        public void ApplyInterestToSavingsAccounts()
        {
            foreach (var account in Accounts)
            {
                if (account is SavingAccount savingsAccount)
                {
                    savingsAccount.ApplyInterest();
                }
            }
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Bank myBank = new Bank("My First Bank");

            
            myBank.CreateSavingsAccount(101, "Alice", 1000, 0.05m, 500);  
            myBank.CreateCheckingAccount(102, "Bob", 500, -200);          

            
            Console.WriteLine("\n--- Accounts List ---");
            myBank.ListAccount();

           
            Console.WriteLine("\n--- Depositing Money ---");
            myBank.Deposit(101, 200);  
            myBank.Deposit(102, 300);  

            
            Console.WriteLine("\n--- Withdrawing Money ---");
            myBank.Withdraw(101, 100); 
            myBank.Withdraw(102, 700);  

           
            Console.WriteLine("\n--- Applying Interest to Savings Accounts ---");
            myBank.ApplyInterestToSavingsAccounts();

            
            Console.WriteLine("\n--- Transferring Money ---");
            myBank.Transfer(101, 102, 150);  

  
            Console.WriteLine("\n--- Transactions for Alice's Account ---");
            myBank.ListTransactions(101);

            Console.WriteLine("\n--- Transactions for Bob's Account ---");
            myBank.ListTransactions(102);


            Console.WriteLine($"\nTotal assets in the bank: {myBank.TotalAsset()}");

            Console.ReadLine();
        }
    }
}
