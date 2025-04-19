using EnvaTest.DTO;

namespace EnvaTest.Services.Abstract
{
    public interface ICalculatorService
    {
        Task<double> CalculateTehlikesizAtikEmisyonu(TehlikesizAtikDto dto);
        Task<double> CalculateYanginTupuEmisyonu(YanginTupuDto dto);
        Task<double> CalculateDogalgazEmisyonu(DogalgazDto dto);
        Task<double> CalculateDizelEmisyonu(DizelDto dto);
    }
}
