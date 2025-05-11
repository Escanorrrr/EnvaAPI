using EnvaTest.DTO;

namespace EnvaTest.Services.Abstract
{
    public interface ICalculatorService
    {
        Task<double> CalculateTehlikesizAtikEmisyonu(TehlikesizAtikDto dto);
        Task<double> CalculateTehlikeliAtikEmisyonu(TehlikeliAtikDto dto);
        Task<double> CalculateYanginTupuEmisyonu(YanginTupuDto dto);
        Task<double> CalculateDogalgazEmisyonu(DogalgazDto dto);
        Task<double> CalculateDizelEmisyonu(DizelDto dto);
        Task<double> CalculateElektrikEmisyonu(ElektrikDto dto);
        Task<double> CalculateBuharEmisyonu(BuharDto dto);
    }
}
