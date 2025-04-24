using System;
using System.Collections.Generic;

namespace EnvaTest.DTO
{
    public class MonthlyGHGData
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public double TotalGHG { get; set; }
        public int InvoiceCount { get; set; }
    }

    public class YearlyGHGResponseDTO
    {
        public int Year { get; set; }
        public List<MonthlyGHGData> MonthlyData { get; set; }
        
        public YearlyGHGResponseDTO()
        {
            MonthlyData = new List<MonthlyGHGData>();
        }
    }
} 