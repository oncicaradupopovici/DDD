using Banking.Domain.Model.AccountAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Domain.Interfaces.Services
{
    public interface IAccountService
    {
        Task<decimal> GetBalanceForAccountAtDateAsync(Guid accountId, DateTime atDate);
        Task DepositAsync(Guid accountId, decimal amount);
        Task WithdrawAsync(Guid accountId, decimal amount);
    }
}
