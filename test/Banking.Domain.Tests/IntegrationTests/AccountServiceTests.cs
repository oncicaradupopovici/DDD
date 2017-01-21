using Banking.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Banking.Domain.Interfaces.Repositories;
using Banking.Domain.Model.AccountAggregate;
using Xunit;
using System.Threading;
using System.Diagnostics;

namespace Banking.Domain.Tests.IntegrationTests
{
    public class AccountServiceTests
    {

        [Fact]
        public async Task Deposit_ChangesBalanceAndRecordsTransaction()
        {
            //Arrange       
            var account = Account.NewAccount(Guid.NewGuid());
            var repo = Mock.Of<IAccountRepository>(
                r => r.GetAccountWithTransactionsAsync(account.AccountId) == Task.FromResult(account));
            var sut = new AccountService(repo);
            var depositValue = 10M;

            //Act
            await sut.DepositAsync(account.AccountId, depositValue);

            //Assert
            Assert.Equal(depositValue, account.Balance);
            Assert.Equal(1, account.Transactions.Count());
        }


        [Fact]
        public void Deposit_IsThreadSafe()
        {
            //Arrange       
            var rand = new Random(1);
            var account = Account.NewAccount(Guid.NewGuid());
            var repo = Mock.Of<IAccountRepository>(
                r => r.GetAccountWithTransactionsAsync(account.AccountId) == Task.FromResult(account));
            var sut = new AccountService(repo);


            //Act --> start 100 concurent tasks that call deposit on the same account
            var threadsCount = 100;
            var depositValue = 10;
            var concurentThreads = new Thread[threadsCount];
            for (var i = 0; i < threadsCount; i++)
            {
                concurentThreads[i] = new Thread(() =>
                {
                    Thread.Sleep(rand.Next(100));
                    sut.DepositAsync(account.AccountId, depositValue).Wait();
                });
            }
            foreach (var t in concurentThreads)
            {
                t.Start();
            }

            foreach (var t in concurentThreads)
            {
                t.Join();
            }

            //Assert
            var repositoryMock = Mock.Get(repo);
            repositoryMock.Verify(r => r.GetAccountWithTransactionsAsync(account.AccountId), Times.Exactly(threadsCount));
            Assert.Equal(threadsCount, account.Transactions.Count());
            Assert.Equal(threadsCount * depositValue, account.Balance);
        }


        [Fact]
        public async Task Withdraw_ChangesBalanceAndRecordsTransaction()
        {
            //Arrange       
            var account = Account.NewAccount(Guid.NewGuid());
            var repo = Mock.Of<IAccountRepository>(
                r => r.GetAccountWithTransactionsAsync(account.AccountId) == Task.FromResult(account));
            var sut = new AccountService(repo);
            var amount = 10M;

            //Act
            await sut.DepositAsync(account.AccountId, amount);
            await sut.WithdrawAsync(account.AccountId, amount);

            //Assert
            Assert.Equal(0, account.Balance);
            Assert.Equal(2, account.Transactions.Count());
        }


        [Fact]
        public async Task Withdraw_ThrowsExceptionWhenAmountIsGreaterThenBalance()
        {
            //Arrange       
            var account = Account.NewAccount(Guid.NewGuid());
            var repo = Mock.Of<IAccountRepository>(
                r => r.GetAccountWithTransactionsAsync(account.AccountId) == Task.FromResult(account));
            var sut = new AccountService(repo);
            var amount = 10M;

            //Act
            Exception exception = null;
            await sut.DepositAsync(account.AccountId, amount);
            try
            {
                sut.WithdrawAsync(account.AccountId, amount + 1).Wait();
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            //Assert
            Assert.IsType<AggregateException>(exception);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }


        [Fact]
        public async Task GetBalanceForAccountAtDateAsync_Works()
        {
            //Arrange       
            var account = Account.NewAccount(Guid.NewGuid());
            var repo = Mock.Of<IAccountRepository>(
                r => r.GetAccountWithTransactionsAsync(account.AccountId) == Task.FromResult(account));
            var sut = new AccountService(repo);


            //Act
            var t0 = DateTime.Now;
            await sut.DepositAsync(account.AccountId, 10);
            await Task.Delay(100);
            var t1 = DateTime.Now;
            await sut.DepositAsync(account.AccountId, 10);
            await Task.Delay(100);
            var t2 = DateTime.Now;
            await sut.DepositAsync(account.AccountId, 10);
            await Task.Delay(100);
            var t3 = DateTime.Now;
            await sut.DepositAsync(account.AccountId, 10);

            var b0 = await sut.GetBalanceForAccountAtDateAsync(account.AccountId, t0);
            var b1 = await sut.GetBalanceForAccountAtDateAsync(account.AccountId, t1);
            var b2 = await sut.GetBalanceForAccountAtDateAsync(account.AccountId, t2);
            var b3 = await sut.GetBalanceForAccountAtDateAsync(account.AccountId, t3);

            //Assert
            Assert.Equal(0, b0);
            Assert.Equal(10, b1);
            Assert.Equal(20, b2);
            Assert.Equal(30, b3);
        }

    }
}
