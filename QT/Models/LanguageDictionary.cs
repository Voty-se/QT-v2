using System;
using System.Collections.Generic;
using QT.Authentication;

namespace QT.Models
{
    public static class LanguageDictionary
    {
        public static Dictionary<string, string> Translate => new Dictionary<string, string>
        {
            { Status.All.ToString(), "Alla" },
            { Status.Done.ToString(), "Klar" },
            { Status.New.ToString(), "Ny" },
            { Status.NotDone.ToString(), "Pågång" },
            { Delivery.Home.ToString(), "Bostad" },
            { Delivery.SideWalk.ToString(), "Trottoar" },
            { DayOfWeek.Monday.ToString(), "Måndag" },
            { DayOfWeek.Tuesday.ToString(), "Tisdag" },
            { DayOfWeek.Wednesday.ToString(), "Onsdag" },
            { DayOfWeek.Thursday.ToString(), "Torsdag" },
            { DayOfWeek.Friday.ToString(), "Fredag" },
            { DayOfWeek.Saturday.ToString(), "Lördag" },
            { DayOfWeek.Sunday.ToString(), "Söndag" },
            { BookingTypes.Booking.ToString(), "Leverans" },
            { BookingTypes.Return.ToString(), "Retur" },
            { BookingTypes.Monting.ToString(), "Montering" },
            { Role.xlutz.ToString(), "Säljare" },
            { Role.LogisticAdministrator.ToString(), "Reklamation" }
        };
    }
}