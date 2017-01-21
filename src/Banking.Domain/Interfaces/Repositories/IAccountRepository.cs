using Banking.Domain.Model.AccountAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Domain.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountWithTransactionsAsync(Guid accountId);
        Task SaveAsync();
    }
}
