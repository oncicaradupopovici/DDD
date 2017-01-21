using Banking.Domain.Interfaces.Repositories;
using Banking.Domain.Interfaces.Services;
using Banking.Domain.Model.AccountAggregate;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Banking.Domain.Services
{
    public class AccountService : IAccountService
    {
        private volatile static Dictionary<Guid, SemaphoreSlim> _accountSemaphores = new Dictionary<Guid, SemaphoreSlim>();
        private volatile static object _accountSemaphoresLock = new object();

        private IAccountRepository _accountRepository;
        

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<decimal> GetBalanceForAccountAtDateAsync(Guid accountId, DateTime atDate)
        {
            if (atDate > DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(atDate));

            var account = await _accountRepository.GetAccountWithTransactionsAsync(accountId);

            if (account == null)
                throw new Exception("Account not found");

            if (atDate < account.CreationDate )
                throw new ArgumentOutOfRangeException(nameof(atDate));

            var balance = account.Balance;
            var transactionsFromThatDateToPresent = account.Transactions.Where(t => t.CreationDate >= atDate);
            foreach(var t in transactionsFromThatDateToPresent)
            {
                balance -= t.GetBalanceAddedAmount();
            }

            return balance;
        }

        public async Task DepositAsync(Guid accountId, decimal amount) 
            => await TransferAsync(accountId, amount, TransactionType.Credit);

        public async Task WithdrawAsync(Guid accountId, decimal amount)
            => await TransferAsync(accountId, amount, TransactionType.Debit);

        internal async Task TransferAsync(Guid accountId, decimal amount, TransactionType transactionType)
        {
            var accountSemphore = GetAccountSemaphore(accountId);
            await accountSemphore.WaitAsync();

            try
            {
                var account = await _accountRepository.GetAccountWithTransactionsAsync(accountId);

                if (account == null)
                    throw new Exception("Account not found");

                var transaction = Transaction.NewTransaction(accountId, transactionType, amount);
                account.AddTransaction(transaction);
                await _accountRepository.SaveAsync();
            }
            finally
            {
                accountSemphore.Release();
            }
        }

        private SemaphoreSlim GetAccountSemaphore(Guid accountId)
        {
            if (!_accountSemaphores.ContainsKey(accountId))
            {
                lock (_accountSemaphoresLock)
                {
                    if(!_accountSemaphores.ContainsKey(accountId))
                        _accountSemaphores[accountId] = new SemaphoreSlim(1);
                }
            }

            return _accountSemaphores[accountId];
        }


    }
}
