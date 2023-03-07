using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class EmployeeDAO
    {
        public static async Task<List<Employee>> GetEmployees()
        {
            var listEmployees = new List<Employee>();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    listEmployees = await context.Employees.Include(x => x.Department).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listEmployees;
        }

        public static async Task<Employee> GetEmployeeById(int id)
        {
            Employee employee = new Employee();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    employee = await context.Employees.Include(x => x.Department).SingleOrDefaultAsync(x => x.EmployeeId == id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employee;
        }

        public static async Task<Employee> CreateEmployee(Employee Employee)
        {
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    await context.Employees.AddAsync(Employee);
                    await context.SaveChangesAsync();

                    return await context.Employees.Include(x => x.Department).SingleOrDefaultAsync(x => x.EmployeeId == Employee.EmployeeId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<Employee> UpdateEmployee(Employee Employee)
        {
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    context.Entry<Employee>(Employee).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();

                    return await context.Employees.Include(x => x.Department).SingleOrDefaultAsync(x => x.EmployeeId == Employee.EmployeeId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task DeleteEmployee(int id)
        {
            try
            {
                Employee employee = new Employee();
                using (var context = new ClothesStoreDBContext())
                {
                    employee = await context.Employees.Include(x => x.Department).SingleOrDefaultAsync(x => x.EmployeeId == id);
                    employee.IsActive = false;
                    context.Entry<Employee>(employee).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
