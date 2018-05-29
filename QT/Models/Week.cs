using System;
using System.Collections.Generic;

namespace QT.Models
{
    public class Week
    {
        public int DayFrom { get; set; }
        public int DayTo { get; set; }
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }

        public DateTime DateFrom => new DateTime(YearFrom, MonthFrom, DayFrom);
        public DateTime DateTo => new DateTime(YearTo, MonthTo, DayTo);

        public DateTime CurrentDate { get; set; }
        public List<Boking> Bokings { get; set; }

        public Week()
        {
            SetDefault(DateTime.Today);
        }

        public Week(DateTime date)
        {
            SetDefault(date);
        }

        //private void SetBookings()

        private void SetDefault(DateTime date)
        {
            DayFrom = date.DayOfWeek == DayOfWeek.Sunday ? date.AddDays(-6).Day : date.AddDays(1 - (double) date.DayOfWeek).Day;
            DayTo = date.DayOfWeek == DayOfWeek.Sunday ? date.Day : date.AddDays(7 - (double)date.DayOfWeek).Day;

            MonthFrom = DayFrom < DayTo ? date.Month : date.Day < 7 ? date.Month - 1 : date.Month;
            MonthTo = DayFrom < DayTo ? date.Month : date.Day < 7 ? date.Month : date.Month + 1;

            YearFrom = (MonthFrom == 12 && MonthTo == 1) ? (date.Month == 12 ? date.Year : date.Year - 1) : date.Year;
            YearTo = (MonthFrom == 12 && MonthTo == 1) ? (date.Month == 1 ? date.Year : date.Year + 1) : date.Year;

            CurrentDate = date;

            Bokings = new List<Boking>();
        }
    }
}