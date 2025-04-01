using EnvaTest.DTO;
using EnvaTest.Context;
using EnvaTest.Entities;
using EnvaTest.Result;
using EnvaTest.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace EnvaTest.Services.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly EnvaContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(EnvaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Result<TokenDTO>> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Username == loginDTO.Username);

                if (customer == null)
                    return Result<TokenDTO>.Error("Kullanıcı adı veya şifre hatalı");

                if (!BC.Verify(loginDTO.Password, customer.Password))
                    return Result<TokenDTO>.Error("Kullanıcı adı veya şifre hatalı");

                if (!customer.Active)
                    return Result<TokenDTO>.Error("Hesabınız aktif değil");

                var token = GenerateJwtToken(customer);
                customer.LastLoginDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Result<TokenDTO>.Success(token, "Giriş başarılı");
            }
            catch (Exception ex)
            {
                return Result<TokenDTO>.Error($"Giriş yapılırken bir hata oluştu: {ex.Message}");
            }
        }

        private TokenDTO GenerateJwtToken(Customer customer)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Name, customer.Username),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.Role, customer.IsAdmin ? "Admin" : "User")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:ExpirationInMinutes"])),
                signingCredentials: credentials
            );

            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }
    }
} 