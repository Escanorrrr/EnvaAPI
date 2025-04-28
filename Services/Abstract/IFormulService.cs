using EnvaTest.DTO;
using EnvaTest.Entities;

namespace EnvaTest.Services.Abstract
{
    public interface IFormulService
    {
        Task<List<FormulResponseDTO>> GetAllFormulasAsync();
        Task<FormulResponseDTO> GetFormulaByIdAsync(int id);
        Task<List<FormulResponseDTO>> GetFormulasByCustomerIdAsync(int customerId);
        Task<FormulResponseDTO> CreateFormulaAsync(FormulCreateDTO formulDto);
        Task<FormulResponseDTO> UpdateFormulaAsync(int id, FormulUpdateDTO formulDto);
        Task<bool> DeleteFormulaAsync(int id);
    }
} 