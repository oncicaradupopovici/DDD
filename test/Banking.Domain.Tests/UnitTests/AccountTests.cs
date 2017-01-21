using Banking.Domain.Model.AccountAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Banking.Domain.Tests.UnitTests
{
    public class AccountTests
    {
        [Fact]
        public void NewAccount_IsValid()
        {
            //Arrange
            var accountHolderId = Guid.NewGuid();

            //Act
            var account = Account.NewAccount(accountHolderId);

            //Assert
            Assert.Equal(0, account.Balance);
            Assert.Equal(0, account.Transactions.Count());
            Assert.Equal(accountHolderId, account.AccountHolderId);
        }
    }
}
