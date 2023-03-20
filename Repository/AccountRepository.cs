using AutoMapper;
using BusinessObject.DTO;
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
        public async Task<List<AccountDTO>> GetAccounts()
        {
            return _mapper.Map<List<AccountDTO>>(await AccountDAO.GetAccounts());
        }
        public async Task<AccountDTO> GetAccountById(int id)
        {
            return _mapper.Map<AccountDTO>(await AccountDAO.GetAccountById(id));
        }
        public async Task DeleteAccount(int id)
        {
            await AccountDAO.DeleteAccount(id);
        }
    }
}
