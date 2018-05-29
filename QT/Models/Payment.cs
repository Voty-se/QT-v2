using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PublicHoliday;

namespace QT.Models
{
    public static class Zones
    {
        public static Home Home = new Home();
        public static SideWalk SideWalk = new SideWalk();

        public static List<SelectListItem> ZoneList(bool retur, bool home, DayOfWeek day, DateTime date, bool showPrice = true)
        {
            var holidayExtraPrice = new SwedenPublicHoliday().IsPublicHoliday(date) ? "Röddag extra 500kr" : "";
            List<SelectListItem> list;
            if (home)
            {
                list = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text =
                            Home.Zone1.Name + (showPrice ? $" ({(retur ? Home.Zone1.PriceXLutx : Home.Zone1.PriceCustomer)} kr {holidayExtraPrice})" : ""),
                        Value = Home.Zone1.Name,
                        Selected = true
                    }
                };

                if ( DayOfWeek.Monday <= day || day <= DayOfWeek.Friday)
                    list.Add(new SelectListItem
                    {
                        Text =
                            Home.Zone2.Name + (showPrice ? $" ({(retur ? Home.Zone2.PriceXLutx : Home.Zone2.PriceCustomer)} kr {holidayExtraPrice})" : ""),
                        Value = Home.Zone2.Name
                    });


                if (DayOfWeek.Tuesday >= day || day <= DayOfWeek.Thursday)
                    list.Add(new SelectListItem
                    {
                        Text = Home.Zone3.Name + (showPrice ? $" ({(retur ? Home.Zone3.PriceXLutx : Home.Zone3.PriceCustomer) } kr /km) {holidayExtraPrice}" : ""),
                        Value = Home.Zone3.Name
                    });

                list.Add(new SelectListItem
                {
                    Text = Home.Danmark.Name + (showPrice ? $" ({(retur ? Home.Danmark.PriceXLutx : Home.Danmark.PriceCustomer)} kr {holidayExtraPrice})" : ""),
                    Value = Home.Danmark.Name
                });

                return list;
            }

            list = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text =
                        SideWalk.Zone1.Name + (showPrice ? $" ({(retur ? SideWalk.Zone1.PriceXLutx : SideWalk.Zone1.PriceCustomer)} kr {holidayExtraPrice})" : ""),
                    Value = SideWalk.Zone1.Name,
                    Selected = true
                }
            };

            if (day >= DayOfWeek.Monday || day <= DayOfWeek.Friday)
                list.Add(new SelectListItem
                {
                    Text =
                            SideWalk.Zone2.Name + (showPrice ? $" ({(retur ? SideWalk.Zone2.PriceXLutx : SideWalk.Zone2.PriceCustomer)} kr  {holidayExtraPrice})" : ""),
                    Value = SideWalk.Zone2.Name
                }
                );

            if (day == DayOfWeek.Tuesday || day == DayOfWeek.Thursday)
                list.Add(new SelectListItem
                {
                    Text = SideWalk.Zone3.Name + (showPrice ? $" ({ (retur ? SideWalk.Zone3.PriceXLutx : SideWalk.Zone3.PriceCustomer)} kr /km {holidayExtraPrice})" : ""),
                    Value = SideWalk.Zone3.Name
                });

            list.Add(new SelectListItem
                {
                    Text =
                        SideWalk.Danmark.Name + (showPrice ? $" ({(retur ? SideWalk.Danmark.PriceXLutx : SideWalk.Danmark.PriceCustomer)} kr {holidayExtraPrice} )" : ""),
                    Value = SideWalk.Danmark.Name
                }
            );

            return list;
        }

        public static decimal PriceForDelivery(string wayOfDelivery, string zone, decimal distance, int nbrExtraItems,
            bool xlutzPays, DayOfWeek day, DateTime date)
        {
            var holidayExtraPrice = new SwedenPublicHoliday().IsPublicHoliday(date);
            var total = (nbrExtraItems) * (wayOfDelivery == Delivery.Home.ToString() ? Home.ExtraItem : SideWalk.ExtraItem);

            if (zone == "Zon1" && wayOfDelivery == Delivery.SideWalk.ToString())
                total = 0;

            if (day == DayOfWeek.Saturday || holidayExtraPrice)
                total += 500;
            
            switch (zone)
            {
                case "Zon1":
                    if (xlutzPays)
                        total += wayOfDelivery == Delivery.Home.ToString()
                        ? Home.Zone1.PriceXLutx
                        : SideWalk.Zone1.PriceXLutx;
                    else
                        total += wayOfDelivery == Delivery.Home.ToString()
                            ? Home.Zone1.PriceCustomer
                            : SideWalk.Zone1.PriceCustomer;
                    break;
                case "Zon2":
                    if (xlutzPays)
                        total += wayOfDelivery == Delivery.Home.ToString()
                        ? Home.Zone2.PriceXLutx
                        : SideWalk.Zone2.PriceXLutx;
                    else
                        total += wayOfDelivery == Delivery.Home.ToString()
                            ? Home.Zone2.PriceCustomer
                            : SideWalk.Zone2.PriceCustomer;
                    break;
                case "Danmark":
                    total = wayOfDelivery == Delivery.Home.ToString()
                        ? Home.Danmark.PriceCustomer
                        : SideWalk.Danmark.PriceCustomer;
                    break;
                default:
                    if (xlutzPays) total += (distance *
                                   (wayOfDelivery == Delivery.Home.ToString()
                                       ? Home.Zone3.PriceXLutx
                                       : SideWalk.Zone3.PriceXLutx)) +
                                  (wayOfDelivery == Delivery.Home.ToString() ? Home.Zone3BasePriceXLutuz : 0);
                    else
                        total += (distance *
                                  (wayOfDelivery == Delivery.Home.ToString()
                                      ? Home.Zone3.PriceCustomer
                                      : SideWalk.Zone3.PriceCustomer)) +
                                 (wayOfDelivery == Delivery.Home.ToString() ? Home.Zone3BasePriceCustomer : 0);
                    break;

            }
            return total;
        }

        public static decimal PriceForPicups(string wayOfDelivery, string zone, decimal distance, int nbrExtraItems, bool xlutzPays, DateTime date)
        {
            var total = (nbrExtraItems) * (wayOfDelivery == Delivery.Home.ToString() ? Home.ExtraItem : SideWalk.ExtraItem);

            var holidayExtraPrice = new SwedenPublicHoliday().IsPublicHoliday(date);
            if (holidayExtraPrice)
                total += 500;

            switch (zone)
            {
                case "Zon1":
                    if (xlutzPays)
                        total += wayOfDelivery == Delivery.Home.ToString()
                        ? Home.Zone1.PickupXLutz
                        : SideWalk.Zone1.PickupXLutz;
                    else
                        total += wayOfDelivery == Delivery.Home.ToString()
                        ? Home.Zone1.PickupCustomer
                        : SideWalk.Zone1.PickupCustomer;
                    break;
                case "Zon2":
                    if (xlutzPays)
                        total += wayOfDelivery == Delivery.Home.ToString()
                        ? Home.Zone2.PickupXLutz
                        : SideWalk.Zone2.PickupXLutz;
                    else
                        total += wayOfDelivery == Delivery.Home.ToString()
                        ? Home.Zone2.PickupCustomer
                        : SideWalk.Zone2.PickupCustomer;
                    break;
                case "Danmark":
                    if (xlutzPays)
                        total += wayOfDelivery == Delivery.Home.ToString()
                        ? Home.Danmark.PickupXLutz
                        : SideWalk.Danmark.PickupXLutz;
                    else
                        total = wayOfDelivery == Delivery.Home.ToString()
                        ? Home.Danmark.PickupCustomer
                        : SideWalk.Danmark.PickupCustomer;
                    break;
                default:
                    if (xlutzPays)
                        total += Home.Zone1.PriceXLutx;
                    else
                        total += (wayOfDelivery == Delivery.Home.ToString()
                                ? Home.Zone3.PickupCustomer
                                : SideWalk.Zone3.PickupCustomer);
                    break;

            }
            return total;
        }
    }

    public class SideWalk
    {
        public Zone Zone1 = new Zone { Name = "Zon1", PriceCustomer = 349, PickupCustomer = 150, PriceXLutx = 160, PickupXLutz = 160};
        public Zone Zone2 = new Zone { Name = "Zon2", PriceCustomer = 599, PickupCustomer = 300, PriceXLutx = 342, PickupXLutz = 342};
        public Zone Zone3 = new Zone { Name = "Zon3", PriceCustomer = 14, PickupCustomer = 300, PriceXLutx = 8, PickupXLutz = 10};
        //public Zone Danmark = new Zone { Name = "Danmark", PriceCustomer = 2999, PickupCustomer = 2999, PriceXLutx = 2999, PickupXLutz = 2999};
        public Zone Danmark = new Zone { Name = "Danmark", PriceCustomer = 2999, PickupCustomer = 300, PriceXLutx = 2999, PickupXLutz = 300 };
        public decimal ExtraItem = 100;
    }

    public class Home
    {
        public Zone Zone1 = new Zone { Name = "Zon1", PriceCustomer = 699, PickupCustomer = 350, PriceXLutx = 360, PickupXLutz = 360};
        public Zone Zone2 = new Zone { Name = "Zon2", PriceCustomer = 999, PickupCustomer = 500, PriceXLutx = 592, PickupXLutz = 592};
        public Zone Zone3 = new Zone { Name = "Zon3", PriceCustomer = 14, PickupCustomer = 500, PriceXLutx = 8, PickupXLutz = 10};
        //public Zone Danmark = new Zone { Name = "Danmark", PriceCustomer = 3499, PickupCustomer = 3499, PriceXLutx = 3499, PickupXLutz = 3499 };
        public Zone Danmark = new Zone { Name = "Danmark", PriceCustomer = 3499, PickupCustomer = 350, PriceXLutx = 3499, PickupXLutz = 350 };
        public decimal Zone3BasePriceCustomer = 500;
        public decimal Zone3BasePriceXLutuz = 300;
        public decimal ExtraItem = 100;
    }

    public class Zone
    {
        public string Name { get; set; }
        public decimal PriceXLutx { get; set; }
        public decimal PriceCustomer { get; set; }
        public decimal PickupXLutz { get; set; }
        public decimal PickupCustomer { get; set; }
    }
}