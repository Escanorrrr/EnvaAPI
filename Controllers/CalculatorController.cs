using EnvaTest.DTO;
using EnvaTest.Result;
using EnvaTest.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EnvaTest.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CalculatorController : ControllerBase
    {
        private readonly ICalculatorService _calculatorService;

        public CalculatorController(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        [HttpPost("tehlikesiz-atik")]
        public async Task<ActionResult<Result<double>>> CalculateTehlikesizAtik([FromBody] TehlikesizAtikDto dto)
        {
            if (dto == null || dto.FaaliyetVerisiKg <= 0 || dto.ToplamUretimKg <= 0 || 
                dto.InvoiceId <= 0 || dto.CustomerId <= 0)
                return Result<double>.Error("Geçerli veri girilmelidir.");

            try
            {
                double sonuc = await _calculatorService.CalculateTehlikesizAtikEmisyonu(dto);
                return Result<double>.Success(sonuc, "Tehlikesiz atık karbon ayak izi hesaplandı.");
            }
            catch (DivideByZeroException ex)
            {
                return Result<double>.Error(ex.Message);
            }
            catch (Exception)
            {
                return Result<double>.Error("Hesaplama sırasında bir hata oluştu.");
            }
        }

        [HttpPost("yangin-tupu")]
        public async Task<ActionResult<Result<double>>> CalculateYanginTupu([FromBody] YanginTupuDto dto)
        {
            if (dto == null || dto.FaaliyetVerisiKg <= 0 || dto.TupSayisi <= 0 || 
                dto.ToplamUretimTon <= 0 || dto.InvoiceId <= 0 || dto.CustomerId <= 0)
                return Result<double>.Error("Geçerli veri girilmelidir.");

            try
            {
                double sonuc = await _calculatorService.CalculateYanginTupuEmisyonu(dto);
                return Result<double>.Success(sonuc, "Yangın tüpü karbon ayak izi hesaplandı.");
            }
            catch (DivideByZeroException ex)
            {
                return Result<double>.Error(ex.Message);
            }
            catch (Exception)
            {
                return Result<double>.Error("Hesaplama sırasında bir hata oluştu.");
            }
        }

        [HttpPost("dogalgaz")]
        public async Task<ActionResult<Result<double>>> CalculateDogalgaz([FromBody] DogalgazDto dto)
        {
            if (dto == null || dto.FaaliyetVerisiM3 <= 0 || 
                dto.ToplamUretimTon <= 0 || dto.InvoiceId <= 0 || dto.CustomerId <= 0)
                return Result<double>.Error("Geçerli veri girilmelidir.");

            try
            {
                double sonuc = await _calculatorService.CalculateDogalgazEmisyonu(dto);
                return Result<double>.Success(sonuc, "Doğalgaz karbon ayak izi hesaplandı.");
            }
            catch (DivideByZeroException ex)
            {
                return Result<double>.Error(ex.Message);
            }
            catch (Exception)
            {
                return Result<double>.Error("Hesaplama sırasında bir hata oluştu.");
            }
        }

        [HttpPost("dizel")]
        public async Task<ActionResult<Result<double>>> CalculateDizel([FromBody] DizelDto dto)
        {
            if (dto == null || dto.FaaliyetVerisiLitre <= 0 || 
                dto.ToplamUretimTon <= 0 || dto.InvoiceId <= 0 || dto.CustomerId <= 0)
                return Result<double>.Error("Geçerli veri girilmelidir.");

            try
            {
                double sonuc = await _calculatorService.CalculateDizelEmisyonu(dto);
                return Result<double>.Success(sonuc, "Dizel karbon ayak izi hesaplandı.");
            }
            catch (DivideByZeroException ex)
            {
                return Result<double>.Error(ex.Message);
            }
            catch (Exception)
            {
                return Result<double>.Error("Hesaplama sırasında bir hata oluştu.");
            }
        }
    }
}
    


