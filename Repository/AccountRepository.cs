using AutoMapper;
using BusinessObject.DTO;
using BusinessObject.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMapper _mapper;
        public AccountRepository(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<List<AccountDTO>> GetAccounts(string? searchString)
        {
            return _mapper.Map<List<AccountDTO>>(await AccountDAO.GetAccounts(searchString));
        }
        public async Task<AccountDTO> GetAccountById(int id)
        {
            return _mapper.Map<AccountDTO>(await AccountDAO.GetAccountById(id));
        }

        public async Task<AccountDTO> GetAccountByEmail(string email)
        {
            return _mapper.Map<AccountDTO>(await AccountDAO.GetAccountByEmail(email));
        }

        public async Task<Account> Account(LoginDTO req) => await AccountDAO.GetAccount(req);
        public async Task DeleteAccount(int id)
        {
            await AccountDAO.DeleteAccount(id);
        }

        public async Task<string?> GetResetPassword(string? email)
        {
            return await AccountDAO.GetResetPassword(email);
        }

        public async Task<bool> Save(SignUpDTO req) => await AccountDAO.SaveCustomer(req);

        public async Task<bool> Update(SignUpDTO req) => await AccountDAO.UpdateCustomer(req);

      /*  public async Task<SignUpDTO> GetInfoCustomerById(SignUpDTO info) => await AccountDAO.GetInfoCustomer(info);*/

        
    }
}
