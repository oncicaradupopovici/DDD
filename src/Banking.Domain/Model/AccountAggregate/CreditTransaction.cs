using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Domain.Model.AccountAggregate
{
    public class CreditTransaction : Transaction
    {
        internal CreditTransaction()
            : base(TransactionType.Credit)
        {
        }

        public override decimal GetBalanceAddedAmount()
        {
            return Amount;
        }
    }
}
