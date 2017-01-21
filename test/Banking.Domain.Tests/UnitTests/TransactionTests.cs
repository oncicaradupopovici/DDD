using Banking.Domain.Model.AccountAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Banking.Domain.Tests.UnitTests
{
    public class TransactionTests
    {
        [Fact]
        public void NewTransaction_IsValidForValidInput()
        {
            //Arrange
            var accountId = Guid.NewGuid();
            var amount = 100;

            //Act
            var creditTransaction = Transaction.NewTransaction(accountId, TransactionType.Credit, amount);
            var debitTransaction = Transaction.NewTransaction(accountId, TransactionType.Debit, amount);

            //Assert
            Assert.Equal(accountId, creditTransaction.AccountId);
            Assert.Equal(accountId, debitTransaction.AccountId);
            Assert.Equal(amount, creditTransaction.Amount);
            Assert.Equal(amount, debitTransaction.Amount);
            Assert.IsType<CreditTransaction>(creditTransaction);
            Assert.IsType<DebitTransaction>(debitTransaction);
        }

        [Fact]
        public void NewTransaction_ThrowsWhenAmountIsZeroOrNegative()
        {
            //Arrange
            var accountId = Guid.NewGuid();

            //Act
            Action zeroAmountTransactionAction = ()=> Transaction.NewTransaction(accountId, TransactionType.Credit, 0);
            Action negativeAmountTransactionAction = ()=> Transaction.NewTransaction(accountId, TransactionType.Credit, -100);

            //Assert
            Assert.Throws<ArgumentOutOfRangeException>(zeroAmountTransactionAction);
            Assert.Throws<ArgumentOutOfRangeException>(negativeAmountTransactionAction);
        }
    }
}
