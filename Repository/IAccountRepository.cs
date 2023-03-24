using BusinessObject.DTO;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IAccountRepository
    {
        Task<List<AccountDTO>> GetAccounts(string? searchString);
        Task<AccountDTO> GetAccountById(int id);
        Task<AccountDTO> GetAccountByEmail(string email);
        Task<Account> Account(LoginDTO req);
        Task DeleteAccount(int id);
    }
}
