using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Domain.Model.AccountAggregate
{
    //When implementing the data access a TPH mapping should be used with TransactionType as discriminator
    public abstract class Transaction
    {
        private static Dictionary<TransactionType, Func<Transaction>> _transactionTypeConstructor =
            new Dictionary<TransactionType, Func<Transaction>>
            {
                { TransactionType.Credit, ()=> new CreditTransaction() },
                { TransactionType.Debit, ()=> new DebitTransaction() }
            };

        public Guid TransactionId { get; private set; }
        public Guid AccountId { get; private set; }
        public TransactionType TransactionType { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime CreationDate { get; private set; }

        protected Transaction(TransactionType transactionType)
        {
            TransactionType = transactionType;
        }

        /// <summary>
        /// Returns the amount with the proper sign that will be added to balance
        /// </summary>
        /// <returns></returns>
        public abstract decimal GetBalanceAddedAmount();


        //Transaction factory method
        public static Transaction NewTransaction(Guid accountId, TransactionType transactionType, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (!_transactionTypeConstructor.ContainsKey(transactionType))
                throw new NotSupportedException($"Transaction type {transactionType} is not supported.");          

            var tran = _transactionTypeConstructor[transactionType]();
            tran.TransactionId = Guid.NewGuid();
            tran.AccountId = accountId;
            tran.Amount = amount;
            tran.CreationDate = DateTime.Now;

            return tran;
        }
    }

}
