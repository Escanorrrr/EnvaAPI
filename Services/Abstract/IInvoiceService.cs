using EnvaTest.DTO;
using EnvaTest.Entities;
using EnvaTest.Result;

namespace EnvaTest.Services.Abstract
{
    public interface IInvoiceService
    {
        Task<Result<InvoiceResponseDTO>> UploadInvoiceAsync(long customerId, InvoiceRequestDTO requestDTO);
        Task<Result<IEnumerable<InvoiceResponseDTO>>> GetCustomerInvoicesAsync(long customerId);
        Task<Result<InvoiceResponseDTO>> GetInvoiceByIdAsync(long invoiceId);
    }
} 