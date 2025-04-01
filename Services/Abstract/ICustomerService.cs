using EnvaTest.DTO;
using EnvaTest.Entities;
using EnvaTest.Result;

namespace EnvaTest.Services.Abstract
{
    public interface ICustomerService
    {
        Task<Result<IEnumerable<CustomerListDTO>>> GetAllCustomersAsync();
        Task<Result<Customer>> GetCustomerByIdAsync(long id);
        Task<Result<Customer>> CreateCustomerAsync(CustomerCreateDTO customerDTO);
        Task<Result<Customer>> UpdateCustomerAsync(long id, CustomerUpdateDTO customerDTO);
        Task<Result<Customer>> UpdateCustomerStatusAsync(long id, bool isActive);
        Task<Result<Customer>> ChangePasswordAsync(long id, ChangePasswordDTO changePasswordDTO);
    }
}
