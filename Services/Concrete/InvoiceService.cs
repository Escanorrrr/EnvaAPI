using EnvaTest.Context;
using EnvaTest.DTO;
using EnvaTest.Entities;
using EnvaTest.Result;
using EnvaTest.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace EnvaTest.Services.Concrete
{
    public class InvoiceService : IInvoiceService
    {
        private readonly EnvaContext _context;
        private readonly IHostEnvironment _environment;

        public InvoiceService(EnvaContext context, IHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
                        UpdatedAt = i.UpdatedAt
                    })
                    .ToListAsync();

                return Result<IEnumerable<InvoiceResponseDTO>>.Success(invoices, "Faturalar başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<InvoiceResponseDTO>>.Error($"Faturalar getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<InvoiceResponseDTO>> GetInvoiceByIdAsync(long invoiceId)
        {
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.InvoiceType)
                    .Include(i => i.Customer)
                    .FirstOrDefaultAsync(i => i.Id == invoiceId);

                if (invoice == null)
                    return Result<InvoiceResponseDTO>.NotFound("Fatura bulunamadı.");

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

                return Result<InvoiceResponseDTO>.Success(responseDTO, "Fatura başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return Result<InvoiceResponseDTO>.Error($"Fatura getirilirken bir hata oluştu: {ex.Message}");
            }
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