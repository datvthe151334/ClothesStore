﻿using BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IAccountRepository
    {
        Task<List<AccountDTO>> GetAccounts();
        Task<AccountDTO> GetAccountById(int id);
    }
}