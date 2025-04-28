using EnvaTest.DTO;
using EnvaTest.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnvaTest.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class FormulController : ControllerBase
    {
        private readonly IFormulService _formulService;

        public FormulController(IFormulService formulService)
        {
            _formulService = formulService;
        }

        [HttpGet]
        public async Task<ActionResult<List<FormulResponseDTO>>> GetAllFormulas()
        {
            var formulas = await _formulService.GetAllFormulasAsync();
            return Ok(formulas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FormulResponseDTO>> GetFormula(int id)
        {
            var formula = await _formulService.GetFormulaByIdAsync(id);
            if (formula == null)
                return NotFound();

            return Ok(formula);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<FormulResponseDTO>>> GetFormulasByCustomer(int customerId)
        {
            var formulas = await _formulService.GetFormulasByCustomerIdAsync(customerId);
            return Ok(formulas);
        }

        [HttpPost]
        public async Task<ActionResult<FormulResponseDTO>> CreateFormula(FormulCreateDTO formulDto)
        {
            try
            {
                var createdFormula = await _formulService.CreateFormulaAsync(formulDto);
                return CreatedAtAction(nameof(GetFormula), new { id = createdFormula.Id }, createdFormula);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FormulResponseDTO>> UpdateFormula(int id, FormulUpdateDTO formulDto)
        {
            try
            {
                var updatedFormula = await _formulService.UpdateFormulaAsync(id, formulDto);
                if (updatedFormula == null)
                    return NotFound();

                return Ok(updatedFormula);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFormula(int id)
        {
            var result = await _formulService.DeleteFormulaAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 