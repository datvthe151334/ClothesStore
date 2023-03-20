using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class AccountDAO
    {
        public static async Task<List<Account>> GetAccounts()
        {
            var listAccounts = new List<Account>();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    var a = await context.Accounts.ToListAsync();
                    listAccounts = await context.Accounts.Include(x => x.Employee).Include(x => x.Customer).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listAccounts;
        }

        public static async Task<Account> GetAccountById(int id)
        {
            Account? account = new Account();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    account = await context.Accounts.Include(x => x.Employee).Include(x => x.Customer).SingleOrDefaultAsync(x => x.AccountId == id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return account;
        }

        public static async Task DeleteAccount(int id)
        {
            try
            {
                Account? account = new Account();
                using (var context = new ClothesStoreDBContext())
                {
                    account = await context.Accounts.Include(x => x.Customer).Include(x => x.Employee).SingleOrDefaultAsync(x => x.AccountId == id);
                    if(account != null)
                    {
                        account.IsActive = false;
                        if (account.Role == 1)
                        {
                            account.Employee.IsActive = false;
                        }
                        if (account.Role == 2)
                        {
                            account.Customer.IsActive = false;
                        }
                        context.Entry<Account>(account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
