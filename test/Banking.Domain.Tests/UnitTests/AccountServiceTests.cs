using Banking.Domain.Interfaces.Repositories;
using Banking.Domain.Model.AccountAggregate;
using Banking.Domain.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Banking.Domain.Tests.UnitTests
{
    public class AccountServiceTests
    {
        [Fact]
        public async Task Deposit_LoadsAndSavesAccountFromRepository()
        {
            //Arrange
            var account = Account.NewAccount(Guid.NewGuid());
            var repo = Mock.Of<IAccountRepository>(
                r => r.GetAccountWithTransactionsAsync(account.AccountId) == Task.FromResult(account));
            var sut = new AccountService(repo);

            //Act
            await sut.DepositAsync(account.AccountId, 10);

            //Assert
            var repositoryMock = Mock.Get(repo);
            repositoryMock.Verify(r => r.GetAccountWithTransactionsAsync(account.AccountId), Times.Once);
            repositoryMock.Verify(r => r.GetAccountWithTransactionsAsync(account.AccountId), Times.Once);
        }

        [Fact]
        public async Task Deposit_And_Withdraw_LoadsAndSavesAccountFromRepository()
        {
            //Arrange
            var account = Account.NewAccount(Guid.NewGuid());
            var repo = Mock.Of<IAccountRepository>(
                r => r.GetAccountWithTransactionsAsync(account.AccountId) == Task.FromResult(account));
            var sut = new AccountService(repo);


            //Act
            await sut.DepositAsync(account.AccountId, 10);
            await sut.WithdrawAsync(account.AccountId, 10);

            //Assert
            var repositoryMock = Mock.Get(repo);
            repositoryMock.Verify(r => r.GetAccountWithTransactionsAsync(account.AccountId), Times.Exactly(2));
            repositoryMock.Verify(r => r.GetAccountWithTransactionsAsync(account.AccountId), Times.Exactly(2));
        }
    }
}
