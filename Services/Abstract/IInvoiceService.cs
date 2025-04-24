using EnvaTest.DTO;
using EnvaTest.Entities;
using EnvaTest.Result;

namespace EnvaTest.Services.Abstract
{
    public interface IInvoiceService
    {
        Task<Result<InvoiceResponseDTO>> UploadInvoiceAsync(long customerId, InvoiceRequestDTO requestDTO);
        Task<Result<IEnumerable<InvoiceResponseDTO>>> GetCustomerInvoicesAsync(long customerId);
        Task<Result<IEnumerable<InvoiceResponseDTO>>> GetInvoiceByDateRangeAsync(InvoiceDateDTO dateRange, bool isAdmin, long currentUserId);
        Task<Result<InvoiceResponseDTO>> UpdateInvoiceAsync(long invoiceId, InvoiceUpdateDTO updateDTO);
        Task<Result<IEnumerable<InvoiceTypeResponseDTO>>> GetInvoiceTypesAsync();
        Task<Result<YearlyGHGResponseDTO>> GetYearlyGHGDataAsync(int year, long customerId);
    }
} 