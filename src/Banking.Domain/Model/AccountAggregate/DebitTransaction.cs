using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Domain.Model.AccountAggregate
{
    public class DebitTransaction : Transaction
    {
        internal DebitTransaction()
            : base(TransactionType.Debit)
        {
        }

        public override decimal GetBalanceAddedAmount()
        {
            return -Amount;
        }

    }
}
