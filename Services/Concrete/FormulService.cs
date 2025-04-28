using EnvaTest.Context;
using EnvaTest.DTO;
using EnvaTest.Entities;
using EnvaTest.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace EnvaTest.Services.Concrete
{
    public class FormulService : IFormulService
    {
        private readonly EnvaContext _context;

        public FormulService(EnvaContext context)
        {
            _context = context;
        }

        public async Task<List<FormulResponseDTO>> GetAllFormulasAsync()
        {
            var formulas = await _context.Formulas.ToListAsync();
            return formulas.Select(f => new FormulResponseDTO
            {
                Id = f.Id,
                CustomerId = f.CustomerId,
                Name = f.Name,
                Description = f.Description,
                TypeId = f.TypeId,
                Operation = f.Operation,
                Amount = f.Amount,
                CreatedAt = f.CreatedAt
            }).ToList();
        }

        public async Task<FormulResponseDTO> GetFormulaByIdAsync(int id)
        {
            var formula = await _context.Formulas.FindAsync(id);
            if (formula == null)
                return null;

            return new FormulResponseDTO
            {
                Id = formula.Id,
                CustomerId = formula.CustomerId,
                Name = formula.Name,
                Description = formula.Description,
                TypeId = formula.TypeId,
                Operation = formula.Operation,
                Amount = formula.Amount,
                CreatedAt = formula.CreatedAt
            };
        }

        public async Task<List<FormulResponseDTO>> GetFormulasByCustomerIdAsync(int customerId)
        {
            var formulas = await _context.Formulas
                .Where(f => f.CustomerId == customerId)
                .ToListAsync();

            return formulas.Select(f => new FormulResponseDTO
            {
                Id = f.Id,
                CustomerId = f.CustomerId,
                Name = f.Name,
                Description = f.Description,
                TypeId = f.TypeId,
                Operation = f.Operation,
                Amount = f.Amount,
                CreatedAt = f.CreatedAt
            }).ToList();
        }

        public async Task<FormulResponseDTO> CreateFormulaAsync(FormulCreateDTO formulDto)
        {
            var formula = new Formul
            {
                CustomerId = formulDto.CustomerId,
                Name = formulDto.Name,
                Description = formulDto.Description,
                TypeId = formulDto.TypeId,
                Operation = formulDto.Operation,
                Amount = formulDto.Amount,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Formulas.AddAsync(formula);
            await _context.SaveChangesAsync();

            return new FormulResponseDTO
            {
                Id = formula.Id,
                CustomerId = formula.CustomerId,
                Name = formula.Name,
                Description = formula.Description,
                TypeId = formula.TypeId,
                Operation = formula.Operation,
                Amount = formula.Amount,
                CreatedAt = formula.CreatedAt
            };
        }

        public async Task<FormulResponseDTO> UpdateFormulaAsync(int id, FormulUpdateDTO formulDto)
        {
            var existingFormula = await _context.Formulas.FindAsync(id);
            
            if (existingFormula == null)
                return null;

            existingFormula.CustomerId = formulDto.CustomerId;
            existingFormula.Name = formulDto.Name;
            existingFormula.Description = formulDto.Description;
            existingFormula.TypeId = formulDto.TypeId;
            existingFormula.Operation = formulDto.Operation;
            existingFormula.Amount = formulDto.Amount;

            await _context.SaveChangesAsync();

            return new FormulResponseDTO
            {
                Id = existingFormula.Id,
                CustomerId = existingFormula.CustomerId,
                Name = existingFormula.Name,
                Description = existingFormula.Description,
                TypeId = existingFormula.TypeId,
                Operation = existingFormula.Operation,
                Amount = existingFormula.Amount,
                CreatedAt = existingFormula.CreatedAt
            };
        }

        public async Task<bool> DeleteFormulaAsync(int id)
        {
            var formula = await _context.Formulas.FindAsync(id);
            if (formula == null)
                return false;

            _context.Formulas.Remove(formula);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 