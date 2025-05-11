using EnvaTest.Services.Abstract;
using EnvaTest.DTO;
using EnvaTest.Context;
using Microsoft.EntityFrameworkCore;

namespace EnvaTest.Services.Concrete
{
    public class CalculatorService : ICalculatorService
    {
        private readonly EnvaContext _context;
        private readonly IFormulService _formulService;

        public CalculatorService(EnvaContext context, IFormulService formulService)
        {
            _context = context;
            _formulService = formulService;
        }

        public async Task<double> CalculateTehlikesizAtikEmisyonu(TehlikesizAtikDto dto)
        {
            const double conversionFactor = 0.001;
            const double standartEmisyonFaktoru = 21.294;

            double ghgTon = dto.FaaliyetVerisiKg
                            * conversionFactor
                            * standartEmisyonFaktoru
                            * conversionFactor;

            if (dto.ToplamUretimKg == 0)
                throw new DivideByZeroException("Toplam üretim 0 olamaz.");

            var sonuc = ghgTon / dto.ToplamUretimKg;

            // Müşteriye ait formülleri kontrol et
            var formulas = await _formulService.GetFormulasByCustomerIdAsync(dto.CustomerId);
            
            if (formulas != null && formulas.Any())
            {
                foreach (var formula in formulas)
                {
                    // Operation tipine göre işlem yap
                    switch (formula.Operation)
                    {
                        case "1": // Toplama
                            sonuc += formula.Amount;
                            break;
                        case "2": // Çıkarma
                            sonuc -= formula.Amount;
                            break;
                        case "3": // Çarpma
                            sonuc *= formula.Amount;
                            break;
                        case "4": // Bölme
                            if (formula.Amount != 0)
                                sonuc /= formula.Amount;
                            break;
                    }
                }
            }

            // Hesaplama sonucunu faturaya kaydet
            var invoice = await _context.Invoices.FindAsync(dto.InvoiceId);
            if (invoice == null)
                throw new Exception($"Fatura bulunamadı. ID: {dto.InvoiceId}");

            invoice.GHG = sonuc;
            invoice.UpdatedAt = DateTime.UtcNow;
            
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            return sonuc;
        }

        public async Task<double> CalculateYanginTupuEmisyonu(YanginTupuDto dto)
        {
            const double kacacakEmisyonOrani = 0.04;
            const double conversionFactor = 0.001;

            double ghgTon = dto.FaaliyetVerisiKg
                            * dto.TupSayisi
                            * kacacakEmisyonOrani
                            * conversionFactor;

            if (dto.ToplamUretimTon == 0)
                throw new DivideByZeroException("Toplam üretim 0 olamaz.");

            var sonuc = ghgTon / dto.ToplamUretimTon;

            // Müşteriye ait formülleri kontrol et
            var formulas = await _formulService.GetFormulasByCustomerIdAsync(dto.CustomerId);
            
            if (formulas != null && formulas.Any())
            {
                foreach (var formula in formulas)
                {
                    // Operation tipine göre işlem yap
                    switch (formula.Operation)
                    {
                        case "1": // Toplama
                            sonuc += formula.Amount;
                            break;
                        case "2": // Çıkarma
                            sonuc -= formula.Amount;
                            break;
                        case "3": // Çarpma
                            sonuc *= formula.Amount;
                            break;
                        case "4": // Bölme
                            if (formula.Amount != 0)
                                sonuc /= formula.Amount;
                            break;
                    }
                }
            }

            // Hesaplama sonucunu faturaya kaydet
            var invoice = await _context.Invoices.FindAsync(dto.InvoiceId);
            if (invoice == null)
                throw new Exception($"Fatura bulunamadı. ID: {dto.InvoiceId}");

            invoice.GHG = sonuc;
            invoice.UpdatedAt = DateTime.UtcNow;
            
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            return sonuc;
        }

        public async Task<double> CalculateDogalgazEmisyonu(DogalgazDto dto)
        {
            // Sabitler
            const double emisyonFaktoruCO2 = 56100;
            const double emisyonFaktoruCH4 = 5;
            const double emisyonFaktoruNO2 = 0.1;
            const double altIsilDeger = 8100; // kcal/m3
            const double gwpCO2 = 1;
            const double gwpCH4 = 21;
            const double gwpNO2 = 310;
            const double conversion1 = 0.000000001;
            const double conversion2 = 4.18;
            const double conversion3 = 0.001;

            double ghgCO2 = dto.FaaliyetVerisiM3 * emisyonFaktoruCO2 * altIsilDeger * gwpCO2 * conversion1 * conversion2 * conversion3;
            double ghgCH4 = dto.FaaliyetVerisiM3 * emisyonFaktoruCH4 * altIsilDeger * gwpCH4 * conversion1 * conversion2 * conversion3;
            double ghgNO2 = dto.FaaliyetVerisiM3 * emisyonFaktoruNO2 * altIsilDeger * gwpNO2 * conversion1 * conversion2 * conversion3;

            double toplamCO2e = ghgCO2 + ghgCH4 + ghgNO2;

            if (dto.ToplamUretimTon == 0)
                throw new DivideByZeroException("Toplam üretim 0 olamaz.");

            var sonuc = toplamCO2e / dto.ToplamUretimTon;

            // Müşteriye ait formülleri kontrol et
            var formulas = await _formulService.GetFormulasByCustomerIdAsync(dto.CustomerId);
            
            if (formulas != null && formulas.Any())
            {
                foreach (var formula in formulas)
                {
                    // Operation tipine göre işlem yap
                    switch (formula.Operation)
                    {
                        case "1": // Toplama
                            sonuc += formula.Amount;
                            break;
                        case "2": // Çıkarma
                            sonuc -= formula.Amount;
                            break;
                        case "3": // Çarpma
                            sonuc *= formula.Amount;
                            break;
                        case "4": // Bölme
                            if (formula.Amount != 0)
                                sonuc /= formula.Amount;
                            break;
                    }
                }
            }

            // Hesaplama sonucunu faturaya kaydet
            var invoice = await _context.Invoices.FindAsync(dto.InvoiceId);
            if (invoice == null)
                throw new Exception($"Fatura bulunamadı. ID: {dto.InvoiceId}");

            invoice.GHG = sonuc;
            invoice.UpdatedAt = DateTime.UtcNow;
            
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            return sonuc;
        }

        public async Task<double> CalculateDizelEmisyonu(DizelDto dto)
        {
            const double emisyonFaktoruCO2 = 74100;
            const double emisyonFaktoruCH4 = 3.9;
            const double emisyonFaktoruNO2 = 3.9;
            const double altIsilDeger = 43; // MJ/kg
            const double gwpCO2 = 1;
            const double gwpCH4 = 21;
            const double gwpNO2 = 310;
            const double conversion1 = 0.000001; // 1/TJ = MJ
            const double conversion2 = 0.001; // kg -> ton
            const double yogunluk = 0.83; // kg/L

            double ghgCO2 = dto.FaaliyetVerisiLitre * emisyonFaktoruCO2 * altIsilDeger * gwpCO2 * conversion1 * conversion2 * yogunluk;
            double ghgCH4 = dto.FaaliyetVerisiLitre * emisyonFaktoruCH4 * altIsilDeger * gwpCH4 * conversion1 * conversion2 * yogunluk;
            double ghgNO2 = dto.FaaliyetVerisiLitre * emisyonFaktoruNO2 * altIsilDeger * gwpNO2 * conversion1 * conversion2 * yogunluk;

            double toplamCO2e = ghgCO2 + ghgCH4 + ghgNO2;

            if (dto.ToplamUretimTon == 0)
                throw new DivideByZeroException("Toplam üretim 0 olamaz.");

            var sonuc = toplamCO2e / dto.ToplamUretimTon;

            // Müşteriye ait formülleri kontrol et
            var formulas = await _formulService.GetFormulasByCustomerIdAsync(dto.CustomerId);
            
            if (formulas != null && formulas.Any())
            {
                foreach (var formula in formulas)
                {
                    // Operation tipine göre işlem yap
                    switch (formula.Operation)
                    {
                        case "1": // Toplama
                            sonuc += formula.Amount;
                            break;
                        case "2": // Çıkarma
                            sonuc -= formula.Amount;
                            break;
                        case "3": // Çarpma
                            sonuc *= formula.Amount;
                            break;
                        case "4": // Bölme
                            if (formula.Amount != 0)
                                sonuc /= formula.Amount;
                            break;
                    }
                }
            }

            // Hesaplama sonucunu faturaya kaydet
            var invoice = await _context.Invoices.FindAsync(dto.InvoiceId);
            if (invoice == null)
                throw new Exception($"Fatura bulunamadı. ID: {dto.InvoiceId}");

            invoice.GHG = sonuc;
            invoice.UpdatedAt = DateTime.UtcNow;
            
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            return sonuc;
        }

        public async Task<double> CalculateElektrikEmisyonu(ElektrikDto dto)
        {
            const double emisyonFaktoru = 0.447; // KG CO2e/KWH
            const double kgToTon = 0.001;

            // 1. Faaliyet Verisi (KWH) × Standart Emisyon Faktörü = GHG (KG CO2e)
            double ghgKg = dto.FaaliyetVerisiKwh * emisyonFaktoru;

            // 2. GHG (KG CO2e) / 1000 = GHG (ton CO2e)
            double ghgTon = ghgKg * kgToTon;

            if (dto.ToplamUretimKg == 0)
                throw new DivideByZeroException("Toplam üretim 0 olamaz.");

            // 3. GHG (ton CO2e) / Toplam Üretim (ton) = Toplam GHG (tonCO2e/ton ürün)
            double toplamUretimTon = dto.ToplamUretimKg * kgToTon;
            double sonuc = ghgTon / toplamUretimTon;

            // Formül desteği
            var formulas = await _formulService.GetFormulasByCustomerIdAsync(dto.CustomerId);
            if (formulas != null && formulas.Any())
            {
                foreach (var formula in formulas)
                {
                    switch (formula.Operation)
                    {
                        case "1": // Toplama
                            sonuc += formula.Amount;
                            break;
                        case "2": // Çıkarma
                            sonuc -= formula.Amount;
                            break;
                        case "3": // Çarpma
                            sonuc *= formula.Amount;
                            break;
                        case "4": // Bölme
                            if (formula.Amount != 0)
                                sonuc /= formula.Amount;
                            break;
                    }
                }
            }

            // Hesaplama sonucunu faturaya kaydet
            var invoice = await _context.Invoices.FindAsync(dto.InvoiceId);
            if (invoice != null)
            {
                invoice.GHG = sonuc;
                invoice.UpdatedAt = DateTime.UtcNow;
                _context.Invoices.Update(invoice);
                await _context.SaveChangesAsync();
            }

            return sonuc;
        }

        public async Task<double> CalculateBuharEmisyonu(BuharDto dto)
        {
            const double emisyonFaktoru = 0.000054; // KG CO2e/KG
            const double conversionFactor = 1000; // 1 TON = 1000 KG
            const double kgToTon = 0.001;

            // 1. Faaliyet Verisi (TON) × Conversion Factor × Standart Emisyon Faktörü = GHG (KG CO2e)
            double ghgKg = dto.FaaliyetVerisiTon * conversionFactor * emisyonFaktoru;

            // 2. GHG (KG CO2e) / 1000 = GHG (ton CO2e)
            double ghgTon = ghgKg * kgToTon;

            if (dto.ToplamUretimTon == 0)
                throw new DivideByZeroException("Toplam üretim 0 olamaz.");

            // 3. GHG (ton CO2e) / Toplam Üretim (ton) = Toplam GHG (tonCO2e/ton ürün)
            double sonuc = ghgTon / dto.ToplamUretimTon;

            // Formül desteği
            var formulas = await _formulService.GetFormulasByCustomerIdAsync(dto.CustomerId);
            if (formulas != null && formulas.Any())
            {
                foreach (var formula in formulas)
                {
                    switch (formula.Operation)
                    {
                        case "1": // Toplama
                            sonuc += formula.Amount;
                            break;
                        case "2": // Çıkarma
                            sonuc -= formula.Amount;
                            break;
                        case "3": // Çarpma
                            sonuc *= formula.Amount;
                            break;
                        case "4": // Bölme
                            if (formula.Amount != 0)
                                sonuc /= formula.Amount;
                            break;
                    }
                }
            }

            // Hesaplama sonucunu faturaya kaydet
            var invoice = await _context.Invoices.FindAsync(dto.InvoiceId);
            if (invoice != null)
            {
                invoice.GHG = sonuc;
                invoice.UpdatedAt = DateTime.UtcNow;
                _context.Invoices.Update(invoice);
                await _context.SaveChangesAsync();
            }

            return sonuc;
        }

        public async Task<double> CalculateTehlikeliAtikEmisyonu(TehlikeliAtikDto dto)
        {
            const double conversionFactor = 0.001;
            const double standartEmisyonFaktoru = 21.294;

            double ghgTon = dto.FaaliyetVerisiKg
                            * conversionFactor
                            * standartEmisyonFaktoru
                            * conversionFactor;

            if (dto.ToplamUretimKg == 0)
                throw new DivideByZeroException("Toplam üretim 0 olamaz.");

            var sonuc = ghgTon / dto.ToplamUretimKg;

            // Müşteriye ait formülleri kontrol et
            var formulas = await _formulService.GetFormulasByCustomerIdAsync(dto.CustomerId);
            if (formulas != null && formulas.Any())
            {
                foreach (var formula in formulas)
                {
                    switch (formula.Operation)
                    {
                        case "1": // Toplama
                            sonuc += formula.Amount;
                            break;
                        case "2": // Çıkarma
                            sonuc -= formula.Amount;
                            break;
                        case "3": // Çarpma
                            sonuc *= formula.Amount;
                            break;
                        case "4": // Bölme
                            if (formula.Amount != 0)
                                sonuc /= formula.Amount;
                            break;
                    }
                }
            }

            // Hesaplama sonucunu faturaya kaydet
            var invoice = await _context.Invoices.FindAsync(dto.InvoiceId);
            if (invoice != null)
            {
                invoice.GHG = sonuc;
                invoice.UpdatedAt = DateTime.UtcNow;
                _context.Invoices.Update(invoice);
                await _context.SaveChangesAsync();
            }

            return sonuc;
        }
    }
}

