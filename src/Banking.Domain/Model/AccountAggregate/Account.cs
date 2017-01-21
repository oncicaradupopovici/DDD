using Banking.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Domain.Model.AccountAggregate
{
    public class Account
    {
        public Guid AccountId { get; private set; }
        public Guid AccountHolderId { get; private set; }
        public decimal Balance { get; private set; }
        public DateTime CreationDate { get; private set; }

        private readonly ICollection<Transaction> _transactions = new List<Transaction>();

        //The DataAccess implementation may require a collection instead of an enunmeration, but for the sake of the 
        //domain design an enumeration provides the best encapsulation here
        public IEnumerable<Transaction> Transactions { get { return _transactions; } }
        
        //ORM's usually support this
        private Account()
        {
        }

        //Account factory method
        public static Account NewAccount(Guid accountHolderId)
        {
            var acc = new Account()
            {
                AccountId = Guid.NewGuid(),
                AccountHolderId = accountHolderId,
                Balance = 0,
                CreationDate = DateTime.Now
            };

            return acc;
        }

        //As this is the only method with side-effects, invariants(balance >= 0) are tested shere
        internal void AddTransaction(Transaction transaction)
        {
            if (transaction.AccountId != AccountId)
                throw new InvalidOperationException("Transaction belongs to another account");

            if(transaction.CreationDate < CreationDate)
                throw new InvalidOperationException("Transaction created before account");


            var balanceAmountToAdd = transaction.GetBalanceAddedAmount();
            if(balanceAmountToAdd == 0)
                throw new InvalidOperationException("Empty transaction");

            if (Balance + balanceAmountToAdd < 0)
                throw new InvalidOperationException("Insufficient founds.");

            Balance += balanceAmountToAdd;
            _transactions.Add(transaction);
        }

    }
}
