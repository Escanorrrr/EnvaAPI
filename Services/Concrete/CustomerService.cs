using EnvaTest.DTO;
using EnvaTest.Entities;
using EnvaTest.Result;
using EnvaTest.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;
using EnvaTest.Context;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EnvaTest.Services.Concrete
{
    public class CustomerService : ICustomerService
    {
        private readonly EnvaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerService(EnvaContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<IEnumerable<CustomerListDTO>>> GetAllCustomersAsync()
        {
            try
            {
                var customers = await _context.Customers
                    .Select(c => new CustomerListDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                        LastName = c.LastName,
                        Email = c.Email,
                        PhoneNumber = c.PhoneNumber,
                        RegistrationDate = c.RegistrationDate,
                        LastLoginDate = c.LastLoginDate,                       
                        Active = c.Active,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToListAsync();

                return Result<IEnumerable<CustomerListDTO>>.Success(customers, "Müşteriler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<CustomerListDTO>>.Error($"Müşteriler getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<Customer>> GetCustomerByIdAsync(long id)
        {
            try
            {
                // Kullanıcının rolünü ve ID'sini kontrol et
                var userRole = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                var userCustomerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Admin değilse ve kendi ID'si değilse erişimi engelle
                if (userRole != "Admin" && (!long.TryParse(userCustomerId, out long currentCustomerId) || currentCustomerId != id))
                {
                    return Result<Customer>.Error("Bu müşteri bilgilerine erişim yetkiniz bulunmamaktadır");
                }

                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                    return Result<Customer>.NotFound($"ID: {id} olan müşteri bulunamadı");

                return Result<Customer>.Success(customer, "Müşteri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return Result<Customer>.Error($"Müşteri getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<Customer>> CreateCustomerAsync(CustomerCreateDTO customerDTO)
        {
            try
            {
                // Username kontrolü
                if (await _context.Customers.AnyAsync(c => c.Username == customerDTO.Username))
                {
                    return Result<Customer>.Error("Bu kullanıcı adı zaten kullanılıyor");
                }

                // Email kontrolü
                if (await _context.Customers.AnyAsync(c => c.Email == customerDTO.Email))
                {
                    return Result<Customer>.Error("Bu email adresi zaten kullanılıyor");
                }

                var customer = new Customer
                {
                    Name = customerDTO.Name,
                    LastName = customerDTO.LastName,
                    Email = customerDTO.Email,
                    PhoneNumber = customerDTO.PhoneNumber,
                    Address = customerDTO.Address,
                    Username = customerDTO.Username,
                    Password = BC.HashPassword(customerDTO.Password), // Şifreyi hash'liyoruz
                    TaxId = customerDTO.TaxId,
                    IsAdmin = customerDTO.IsAdmin,
                    Active = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    RegistrationDate = DateTime.UtcNow
                };
                
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return Result<Customer>.Success(customer, "Müşteri başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                return Result<Customer>.Error($"Müşteri oluşturulurken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<Customer>> UpdateCustomerAsync(long id, CustomerUpdateDTO customerDTO)
        {
            try
            {
                var existingCustomer = await _context.Customers.FindAsync(id);
                if (existingCustomer == null)
                    return Result<Customer>.NotFound($"ID: {id} olan müşteri bulunamadı");

                // Username kontrolü
                if (await _context.Customers.AnyAsync(c => c.Username == customerDTO.Username && c.Id != id))
                {
                    return Result<Customer>.Error("Bu kullanıcı adı zaten kullanılıyor");
                }

                // Email kontrolü
                if (await _context.Customers.AnyAsync(c => c.Email == customerDTO.Email && c.Id != id))
                {
                    return Result<Customer>.Error("Bu email adresi zaten kullanılıyor");
                }

                existingCustomer.Name = customerDTO.Name;
                existingCustomer.LastName = customerDTO.LastName;
                existingCustomer.Email = customerDTO.Email;
                existingCustomer.PhoneNumber = customerDTO.PhoneNumber;
                existingCustomer.Address = customerDTO.Address;
                existingCustomer.Username = customerDTO.Username;
                existingCustomer.TaxId = customerDTO.TaxId;
                existingCustomer.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Result<Customer>.Success(existingCustomer, "Müşteri başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                return Result<Customer>.Error($"Müşteri güncellenirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<Customer>> UpdateCustomerStatusAsync(long id, bool isActive)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                    return Result<Customer>.NotFound($"ID: {id} olan müşteri bulunamadı");

                customer.Active = isActive;
                customer.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Result<Customer>.Success(customer, $"Müşteri durumu başarıyla {(isActive ? "aktif" : "pasif")} olarak güncellendi");
            }
            catch (Exception ex)
            {
                return Result<Customer>.Error($"Müşteri durumu güncellenirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<Customer>> ChangePasswordAsync(long id, ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                    return Result<Customer>.NotFound($"ID: {id} olan müşteri bulunamadı");

                // Mevcut şifre kontrolü
                if (!BC.Verify(changePasswordDTO.CurrentPassword, customer.Password))
                    return Result<Customer>.Error("Mevcut şifre hatalı");

                // Yeni şifre hash'leme
                customer.Password = BC.HashPassword(changePasswordDTO.NewPassword);
                customer.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Result<Customer>.Success(customer, "Şifre başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                return Result<Customer>.Error($"Şifre güncellenirken bir hata oluştu: {ex.Message}");
            }
        }
    }
}
