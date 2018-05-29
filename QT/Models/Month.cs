using System;
using System.Collections.Generic;
using System.Linq;

namespace QT.Models
{
    public class Month
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        
        public DateTime CurrentDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Boking> Bokings { get; set; }

        public Month()
        {
            SetDefault(DateTime.Today);
        }

        public Month(DateTime date)
        {
            SetDefault(date);
        }
        
        private void SetDefault(DateTime date)
        {
            CurrentDate = date;
            FromDate = new DateTime(date.Year, date.Month, 1);
            ToDate = FromDate.AddMonths(1).AddDays(-1); ;
            
            Bokings = new List<Boking>();
        }

        public double GetSumBookings()
        {
            return Bokings.Sum(b => double.Parse(b.DeliveryCost));
        }
    }
}