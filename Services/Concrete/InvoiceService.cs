using EnvaTest.Context;
using EnvaTest.DTO;
using EnvaTest.Entities;
using EnvaTest.Result;
using EnvaTest.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EnvaTest.Services.Concrete
{
    public class InvoiceService : IInvoiceService
    {
        private readonly EnvaContext _context;
        private readonly IHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InvoiceService(EnvaContext context, IHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<InvoiceResponseDTO>> UploadInvoiceAsync(long customerId, InvoiceRequestDTO requestDTO)
        {
            try
            {
                // Validasyonlar
                if (!requestDTO.IsValidFileType())
                    return Result<InvoiceResponseDTO>.Error("Geçersiz dosya formatı. Sadece PDF, JPG ve JPEG formatları kabul edilmektedir.");

                if (!requestDTO.IsValidFileSize())
                    return Result<InvoiceResponseDTO>.Error("Dosya boyutu çok büyük. Maksimum dosya boyutu 10MB olmalıdır.");

                // Müşteri kontrolü
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                    return Result<InvoiceResponseDTO>.NotFound("Müşteri bulunamadı.");

                // Fatura tipi kontrolü
                var invoiceType = await _context.InvoiceTypes.FindAsync(requestDTO.InvoiceTypeId);
                if (invoiceType == null)
                    return Result<InvoiceResponseDTO>.NotFound("Geçersiz fatura tipi.");

                // Dosya kaydetme işlemi
                var fileName = await SaveInvoiceFileAsync(requestDTO.InvoiceFile, customerId);

                // Veritabanına kayıt
                var invoice = new Invoice
                {
                    CustomerId = customerId,
                    InvoiceTypeId = requestDTO.InvoiceTypeId,
                    InvoicePath = fileName,
                    Amount = requestDTO.Amount,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                // Response DTO oluşturma
                var responseDTO = new InvoiceResponseDTO
                {
                    Id = invoice.Id,
                    InvoicePath = invoice.InvoicePath,
                    Amount = invoice.Amount,
                    CustomerId = invoice.CustomerId,
                    CustomerName = customer.Name,
                    InvoiceTypeId = invoice.InvoiceTypeId,
                    InvoiceTypeName = invoiceType.InvoiceName,
                    CreatedAt = invoice.CreatedAt,
                    UpdatedAt = invoice.UpdatedAt
                };

                return Result<InvoiceResponseDTO>.Success(responseDTO, "Fatura başarıyla yüklendi.");
            }
            catch (Exception ex)
            {
                return Result<InvoiceResponseDTO>.Error($"Fatura yüklenirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<InvoiceResponseDTO>>> GetCustomerInvoicesAsync(long customerId)
        {
            try
            {
                var invoices = await _context.Invoices
                    .Include(i => i.InvoiceType)
                    .Include(i => i.Customer)
                    .Where(i => i.CustomerId == customerId)
                    .OrderByDescending(i => i.CreatedAt)
                    .Select(i => new InvoiceResponseDTO
                    {
                        Id = i.Id,
                        InvoicePath = i.InvoicePath,
                        Amount = i.Amount,
                        CustomerId = i.CustomerId,
                        CustomerName = i.Customer.Name,
                        InvoiceTypeId = i.InvoiceTypeId,
                        InvoiceTypeName = i.InvoiceType.InvoiceName,
                        CreatedAt = i.CreatedAt,
                        UpdatedAt = i.UpdatedAt,
                        GHG = i.GHG
                    })
                    .ToListAsync();

                return Result<IEnumerable<InvoiceResponseDTO>>.Success(invoices, "Faturalar başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<InvoiceResponseDTO>>.Error($"Faturalar getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<InvoiceResponseDTO>>> GetInvoiceByDateRangeAsync(InvoiceDateDTO dateRange, bool isAdmin, long currentUserId)
        {
            try
            {
                if (!isAdmin && dateRange.CustomerId != currentUserId)
                {
                    return Result<IEnumerable<InvoiceResponseDTO>>.Error("Bu işlem için yetkiniz bulunmamaktadır.");
                }

                var query = _context.Invoices
                    .Include(i => i.InvoiceType)
                    .Include(i => i.Customer)
                    .Where(i => i.CustomerId == dateRange.CustomerId);

                if (dateRange != null)
                {
                    query = query.Where(i => i.CreatedAt >= dateRange.BaslangicTarihi && 
                                           i.CreatedAt <= dateRange.BitisTarihi);
                }

                var invoices = await query
                    .OrderByDescending(i => i.CreatedAt)
                    .Select(i => new InvoiceResponseDTO
                    {
                        Id = i.Id,
                        InvoicePath = i.InvoicePath,
                        Amount = i.Amount,
                        CustomerId = i.CustomerId,
                        CustomerName = i.Customer.Name,
                        InvoiceTypeId = i.InvoiceTypeId,
                        InvoiceTypeName = i.InvoiceType.InvoiceName,
                        CreatedAt = i.CreatedAt,
                        UpdatedAt = i.UpdatedAt,
                        GHG = i.GHG
                    })
                    .ToListAsync();

                if (!invoices.Any())
                    return Result<IEnumerable<InvoiceResponseDTO>>.NotFound("Belirtilen tarih aralığında fatura bulunamadı.");

                return Result<IEnumerable<InvoiceResponseDTO>>.Success(invoices, "Faturalar başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<InvoiceResponseDTO>>.Error($"Faturalar getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<InvoiceResponseDTO>> UpdateInvoiceAsync(long invoiceId, InvoiceUpdateDTO updateDTO)
        {
            try
            {
                // Fatura kontrolü
                var invoice = await _context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceType)
                    .FirstOrDefaultAsync(i => i.Id == invoiceId);

                if (invoice == null)
                    return Result<InvoiceResponseDTO>.NotFound("Fatura bulunamadı.");

                // Dosya güncelleme
                if (updateDTO.InvoiceFile != null)
                {
                    // Dosya validasyonları
                    if (!IsValidFileType(updateDTO.InvoiceFile))
                        return Result<InvoiceResponseDTO>.Error("Geçersiz dosya formatı. Sadece PDF, JPG ve JPEG formatları kabul edilmektedir.");

                    if (!IsValidFileSize(updateDTO.InvoiceFile))
                        return Result<InvoiceResponseDTO>.Error("Dosya boyutu çok büyük. Maksimum dosya boyutu 10MB olmalıdır.");

                    // Eski dosyayı silme
                    var oldFilePath = Path.Combine(_environment.ContentRootPath, "Uploads", "Invoices", invoice.InvoicePath);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }

                    // Yeni dosyayı kaydetme
                    var fileName = await SaveInvoiceFileAsync(updateDTO.InvoiceFile, invoice.CustomerId);
                    invoice.InvoicePath = fileName;
                }

                // Fatura tipi güncelleme
                if (updateDTO.InvoiceTypeId.HasValue)
                {
                    var invoiceType = await _context.InvoiceTypes.FindAsync(updateDTO.InvoiceTypeId.Value);
                    if (invoiceType == null)
                        return Result<InvoiceResponseDTO>.NotFound("Geçersiz fatura tipi.");

                    invoice.InvoiceTypeId = updateDTO.InvoiceTypeId.Value;
                }

                // Tutar güncelleme
                if (updateDTO.Amount.HasValue)
                {
                    invoice.Amount = updateDTO.Amount.Value;
                }

                invoice.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Response DTO oluşturma
                var responseDTO = new InvoiceResponseDTO
                {
                    Id = invoice.Id,
                    InvoicePath = invoice.InvoicePath,
                    Amount = invoice.Amount,
                    CustomerId = invoice.CustomerId,
                    CustomerName = invoice.Customer.Name,
                    InvoiceTypeId = invoice.InvoiceTypeId,
                    InvoiceTypeName = invoice.InvoiceType.InvoiceName,
                    CreatedAt = invoice.CreatedAt,
                    UpdatedAt = invoice.UpdatedAt
                };

                return Result<InvoiceResponseDTO>.Success(responseDTO, "Fatura başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return Result<InvoiceResponseDTO>.Error($"Fatura güncellenirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<InvoiceTypeResponseDTO>>> GetInvoiceTypesAsync()
        {
            try
            {
                var invoiceTypes = await _context.InvoiceTypes
                    .Select(it => new InvoiceTypeResponseDTO
                    {
                        Id = it.Id,
                        InvoiceName = it.InvoiceName
                    })
                    .ToListAsync();

                if (!invoiceTypes.Any())
                    return Result<IEnumerable<InvoiceTypeResponseDTO>>.NotFound("Fatura tipi bulunamadı.");

                return Result<IEnumerable<InvoiceTypeResponseDTO>>.Success(invoiceTypes, "Fatura tipleri başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<InvoiceTypeResponseDTO>>.Error($"Fatura tipleri getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<YearlyGHGResponseDTO>> GetYearlyGHGDataAsync(int year, long customerId)
        {
            try
            {
                // Kullanıcının rolünü ve ID'sini kontrol et
                var userRole = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                var userCustomerIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userCustomerIdClaim))
                {
                    return Result<YearlyGHGResponseDTO>.Error("Kullanıcı bilgisi bulunamadı");
                }

                if (!long.TryParse(userCustomerIdClaim, out long currentCustomerId))
                {
                    return Result<YearlyGHGResponseDTO>.Error("Geçersiz kullanıcı ID'si");
                }

                // Admin değilse ve kendi ID'si değilse erişimi engelle
                if (userRole != "Admin" && currentCustomerId != customerId)
                {
                    return Result<YearlyGHGResponseDTO>.Error("Bu GHG verilerine erişim yetkiniz bulunmamaktadır");
                }

                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31);

                var invoices = await _context.Invoices
                    .Where(i => i.CustomerId == customerId &&
                           i.CreatedAt.Year == year)
                    .ToListAsync();

                if (!invoices.Any())
                    return Result<YearlyGHGResponseDTO>.NotFound($"{year} yılına ait fatura bulunamadı.");

                var response = new YearlyGHGResponseDTO { Year = year };

                // Türkçe ay isimleri
                var turkishMonths = new[] { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", 
                                          "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };

                // Her ay için veri hazırla
                for (int month = 1; month <= 12; month++)
                {
                    var monthlyInvoices = invoices.Where(i => i.CreatedAt.Month == month).ToList();
                    
                    var monthlyData = new MonthlyGHGData
                    {
                        Month = month,
                        MonthName = turkishMonths[month - 1],
                        TotalGHG = monthlyInvoices.Sum(i => i.GHG ?? 0.0),
                        InvoiceCount = monthlyInvoices.Count
                    };

                    response.MonthlyData.Add(monthlyData);
                }

                return Result<YearlyGHGResponseDTO>.Success(response, $"{year} yılı GHG verileri başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return Result<YearlyGHGResponseDTO>.Error($"GHG verileri getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<Dictionary<string, double>>> GetGHGByTypeAsync(long customerId)
        {
            try
            {
                // Kullanıcının rolünü ve ID'sini kontrol et
                var userRole = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                var userCustomerIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userCustomerIdClaim))
                {
                    return Result<Dictionary<string, double>>.Error("Kullanıcı bilgisi bulunamadı");
                }

                if (!long.TryParse(userCustomerIdClaim, out long currentCustomerId))
                {
                    return Result<Dictionary<string, double>>.Error("Geçersiz kullanıcı ID'si");
                }

                // Admin değilse ve kendi ID'si değilse erişimi engelle
                if (userRole != "Admin" && currentCustomerId != customerId)
                {
                    return Result<Dictionary<string, double>>.Error("Bu verilere erişim yetkiniz bulunmamaktadır");
                }

                // Faturaları ve tiplerini çek
                var invoices = await _context.Invoices
                    .Where(i => i.CustomerId == customerId)
                    .Include(i => i.InvoiceType)
                    .ToListAsync();

                var result = new Dictionary<string, double>();

                var grouped = invoices
                    .GroupBy(i => i.InvoiceType.InvoiceName)
                    .ToList();

                foreach (var group in grouped)
                {
                    var totalGHG = group.Sum(i => i.GHG ?? 0.0);
                    result[group.Key] = totalGHG;
                }

                return Result<Dictionary<string, double>>.Success(result, "Fatura tiplerine göre toplam GHG değerleri başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return Result<Dictionary<string, double>>.Error($"GHG verileri getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        private bool IsValidFileType(IFormFile file)
        {
            var allowedTypes = new[] { ".pdf", ".jpg", ".jpeg" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedTypes.Contains(extension);
        }

        private bool IsValidFileSize(IFormFile file)
        {
            return file.Length <= 10 * 1024 * 1024; // 10MB
        }

        private async Task<string> SaveInvoiceFileAsync(IFormFile file, long customerId)
        {
            // Uploads klasörü oluşturma
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads", "Invoices");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Benzersiz dosya adı oluşturma
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{customerId}_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Dosyayı kaydetme
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
} 