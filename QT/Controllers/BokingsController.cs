using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using QT.Authentication;
using QT.Models;

namespace QT.Controllers
{
    public class BokingsController : Controller
    {
        private readonly QTransportModelContainer _db = new QTransportModelContainer();

        // GET: Bokings
        [VotyAuthorize()]
        public ActionResult Index()
        {
            return GetWeekCalendar(DateTime.Today.AddDays(1), Status.All);
        }

        // GET: Bokings
        [VotyAuthorize]
        public List<Boking> GetBookings(DateTime from, DateTime to, Status status)
        {
            var bookingSet =
                _db.BokingSet.Where(b => b.BookingDay >= from && b.BookingDay <= to
                && (status == Status.All || b.Status == status.ToString()))
                    .OrderByDescending(b => b.BookingDay);

            return bookingSet.Any() ? bookingSet.ToList() : new List<Boking>();
        }

        // GET: Bokings
        //[VotyAuthorize]
        [HttpPost]
        public string GetBookingByIdOrOrderId(string orderNbr)
        {
            var booking = _db.BokingSet.FirstOrDefault(b => b.OrderNbr == orderNbr);
            return JsonConvert.SerializeObject(booking, Formatting.Indented);
        }

        [HttpGet]
        [VotyAuthorize]
        public ActionResult GetWeekCalendar(DateTime date, Status status)
        {
            var week = new Week(date);
            //var statusList = new List<string>() { status.ToString() };

            var bokingSet = GetBookings(week.DateFrom, week.DateTo, status);
            week.Bokings = bokingSet;

            return View("Calendar", week);
        }

        [HttpGet]
        [VotyAuthorize]
        public ActionResult GetCurrentList(DateTime date, Status status)
        {
            if(date < DateTime.Today)
                date = DateTime.Today;
            
            var mounth = new Month(date);

            var bokingSet = GetBookings(date, date, status);
            mounth.Bokings = bokingSet;

            ViewBag.Status = status;
            return View("List", mounth);
        }

        [HttpGet]
        [VotyAuthorize]
        public ActionResult GetMounthList(DateTime date, Status status)
        {
            var mounth = new Month(date);

            var bokingSet = GetBookings(date, mounth.ToDate, status);
            mounth.Bokings = bokingSet;

            ViewBag.Status = status;
            ViewBag.CurrentPageStatus = date == DateTime.Today.AddDays(1) ? "Comming" : status.ToString();
            return View("List", mounth);
        }

        [HttpPost]
        [VotyAuthorize]
        public ActionResult GetMounthListInterval(DateTime fromDate, DateTime toDate, FormCollection collection)
        {
            var mounth = new Month(fromDate)
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            var status = collection["status"];
            var statusEnum = (Status)Enum.Parse(typeof(Status), status);
            var bokingSet = GetBookings(mounth.FromDate, mounth.ToDate, statusEnum);
            mounth.Bokings = bokingSet;

            ViewBag.Status = statusEnum;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return View("List", mounth);
        }


        [HttpGet]
        [VotyAuthorize]
        public ActionResult GetUnpaidInterval(DateTime fromDate, DateTime toDate)
        {
            var mounth = new Month(fromDate);

            var bookingSet =
                _db.BokingSet.Where(b => b.BookingDay >= fromDate && b.BookingDay <= toDate
                && b.Status == Status.Done.ToString()
                && !b.Payed
                && b.PayBySupplier)
                    .OrderByDescending(b => b.BookingDay);

            mounth.Bokings = bookingSet.ToList();

            return View("ListUnpaid", mounth);
        }

        [HttpGet]
        [VotyAuthorize]
        public ActionResult GetUnpaid(DateTime date, Status status)
        {
            var dateFrom = new DateTime(date.Year, date.Month, 1);
            var dateTo = dateFrom.AddMonths(1).AddDays(-1);
            return GetUnpaidInterval(dateFrom, dateTo);
        }

        [VotyAuthorize(Role.qt)]
        public ActionResult MyBokings(int? id)
        {
            var bookingSet =
                _db.BokingSet.Where(b => !b.Done && b.Car.Id == id)
                    .OrderByDescending(b => b.BookingDay);

            var mounth = new Month(DateTime.Today) {Bokings = bookingSet.ToList()};

            return View("List", mounth);
        }
                
      
        [HttpGet]
        [VotyAuthorize(Role.qt)]
        public ActionResult MarkAsDone(int? id)
        {
            return View("ConfirmDone", id);
        }

        [HttpPost]
        [VotyAuthorize(Role.qt)]
        public ActionResult MarkAsDone(int? id, string status)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                Boking boking = _db.BokingSet.Find(id);
                if (boking == null)
                {
                    return HttpNotFound();
                }
                boking.Status = Status.Done.ToString();
                _db.Entry(boking).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                ViewBag.Error = $"{e.Message}. {e.InnerException?.Message}";
                return HttpNotFound();
            }
        }

        [HttpGet]
        [VotyAuthorize(Role.qt)]
        public ActionResult MarkNotDone(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View("MarkNotDone", id);
        }
        [HttpPost]
        [VotyAuthorize(Role.qt)]
        public ActionResult MarkNotDone(int? id, string note)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                var boking = _db.BokingSet.Find(id);
                if (boking == null)
                {
                    return HttpNotFound();
                }
                boking.Status = Status.NotDone.ToString();
                boking.Remarks += ".\n" + note;
                _db.Entry(boking).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                ViewBag.Error = "Bara admin kan markera bokningar som betalda. ";
                ViewBag.Error += $"{e.Message}. {e.InnerException?.Message}";
                return View(Request.Url);
            }
        }

        [VotyAuthorize(Role.qt)]
        public ActionResult ConfirmPayment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                //if (AdminAuthenticationHelper.Current.GetAdminUserData().UserName.ToLower() == "admin" ||
                //    password == "xxxlutz@3399")
                //{
                    var boking = _db.BokingSet.Find(id);
                    if (boking == null)
                    {
                        return HttpNotFound();
                    }
                    boking.Payed = true;
                    _db.Entry(boking).State = EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                //}
                
            }
            catch (Exception e)
            {
                ViewBag.Error = "Det gick inte att markera leveransen som betald. ";
                ViewBag.Error += $"{e.Message}. {e.InnerException?.Message}";
                return View();
            }
        }

        // GET: Bokings/Edit/5
        [VotyAuthorize(Role.qt)]
        public ActionResult MarkAsPayed(int id)
        {
            return View("ConfirmPayment", id);
        }

        //private decimal GetDistanceDecimal(string distance)
        //{
        //    decimal distanceDbl;
        //    decimal.TryParse(distance, out distanceDbl);
        //    return distanceDbl;
        //}

        // GET: Bokings/Details/5
        [VotyAuthorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boking boking = _db.BokingSet.Find(id);
            if (boking == null)
            {
                return HttpNotFound();
            }
            return View(boking);
        }

        // GET: Bokings/Create
        [VotyAuthorize(Role.xlutz, Role.Administrator)]
        public ActionResult Create(DateTime time, int part, Delivery delivery)
        {
            Session["products"] = new List<StandarProduct>();
            var booking = new Boking
            {
                Type = BookingTypes.Booking.ToString(),
                WayOfDelivery = delivery.ToString(),
                BookingDay = time,
                PartOfTheDay = (short) part, 
                Remarks = "",
                NbrItems =  0,
                Pickup = false,
                NbrItemsPickup = 0,
                PayBySupplier = false
            };

            return View(booking);
        }
        
        // GET: Bokings/Create
        [VotyAuthorize(Role.LogisticAdministrator)]
        public ActionResult CreateRetur(DateTime time, int part, Delivery delivery)
        {
            Session["products"] = new List<StandarProduct>();
            var booking = new Boking()
            {
                Type = BookingTypes.Return.ToString(),
                BookingDay = time,
                PartOfTheDay = (short)part,
                WayOfDelivery = delivery.ToString(),
                Remarks = "",
                NbrItems = 0,
                Pickup = false,
                NbrItemsPickup = 0,
                PayBySupplier = true
            };
            return View(booking);
        }

        // GET: Bokings/Create
        [VotyAuthorize(Role.xlutz, Role.LogisticAdministrator)]
        public ActionResult CreateMonting(DateTime time, int part, Delivery delivery)
        {
            Session["products"] = new List<StandarProduct>();
            var booking = new Boking
            {
                Type = BookingTypes.Monting.ToString(),
                WayOfDelivery = delivery.ToString(),
                BookingDay = time,
                PartOfTheDay = (short)part,
                Remarks = "",
                NbrItems = 0,
                Pickup = false,
                NbrItemsPickup = 0,
                PayBySupplier = false,
                Zone = "Zon1"
            };

            return View(booking);
        }

        // POST: Bokings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [VotyAuthorize(Role.xlutz, Role.Administrator, Role.qt)]
        public ActionResult Create(Boking boking)
        {
            if (ModelState.IsValid)
            {
                var id = AdminAuthenticationHelper.Current.GetAdminUserData().Id;
                try
                {
                    var driverId = boking.WayOfDelivery == Delivery.Home.ToString() ? 1 : 2;
                    var user = _db.UserSet.First(u => u.Id == id);
                    var userFor = _db.UserSet.First(dr => dr.Role == Role.qt.ToString() && dr.Order == driverId);
                    
                    boking.CreatedBy = user;
                    boking.Car = userFor;

                    boking.DeliveryCost = GetPrices(boking.Zone, boking.WayOfDelivery, boking.Distance, boking.Pickup, false, boking.NbrItems, boking.NbrItemsPickup, boking.BookingDay).ToString(CultureInfo.InvariantCulture);
                    boking.Status = Status.New.ToString();

                    if (boking.Remarks == null)
                        boking.Remarks = "";
                    
                    _db.BokingSet.Add(boking);

                    _db.SaveChanges();

                    if (!SendNotificationsToCustomer(boking))
                    {
                        ViewBag.Error = "Misslyckades att skicka avisering till kunden.";
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.Error = "Please check the inforamtions and try again. ";
                    ViewBag.Error += e.Message + ". " + e.InnerException?.Message;
                }
            }

            ViewBag.Delivery = boking.WayOfDelivery;

            return View(boking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VotyAuthorize(Role.LogisticAdministrator, Role.Administrator)]
        public ActionResult CreateRetur(Boking boking, bool leverera)
        {
            if (ModelState.IsValid)
            {
                var id = AdminAuthenticationHelper.Current.GetAdminUserData().Id;
                try
                {
                    var driverId = boking.WayOfDelivery == Delivery.Home.ToString() ? 1 : 2;
                    var user = _db.UserSet.First(u => u.Id == id);
                    var userFor = _db.UserSet.First(dr => dr.Role == Role.qt.ToString() && dr.Order == driverId);

                    boking.CreatedBy = user;
                    boking.Car = userFor;

                    boking.DeliveryCost = GetPrices(boking.Zone, boking.WayOfDelivery, boking.Distance, boking.Pickup, true, boking.NbrItems, boking.NbrItemsPickup, boking.BookingDay, leverera).ToString(CultureInfo.InvariantCulture);
                    boking.Status = Status.New.ToString();

                    if (boking.Remarks == null)
                        boking.Remarks = "";
                    if (boking.OrderNbr == null)
                        boking.OrderNbr = "";

                    boking.Email = leverera;

                    _db.BokingSet.Add(boking);

                    _db.SaveChanges();

                    if (!SendNotificationsToCustomer(boking))
                    {
                        ViewBag.Error = "Misslyckades att skicka avisering till kunden.";
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.Error = "Please check the inforamtions and try again. ";
                    ViewBag.Error += e.Message + ". " + e.InnerException?.Message;
                }
            }

            ViewBag.Delivery = boking.WayOfDelivery;

            return View(boking);
        } 
        
        // POST: Bokings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [VotyAuthorize(Role.xlutz, Role.Administrator, Role.qt)]
        public ActionResult CreateMonting(Boking boking)
        {
            if (ModelState.IsValid)
            {
                var id = AdminAuthenticationHelper.Current.GetAdminUserData().Id;
                try
                {
                    var driverId = boking.WayOfDelivery == Delivery.Home.ToString() ? 1 : 2;
                    var user = _db.UserSet.First(u => u.Id == id);
                    var userFor = _db.UserSet.First(dr => dr.Role == Role.qt.ToString() && dr.Order == driverId);

                    boking.CreatedBy = user;
                    boking.Car = userFor;

                    var productList = (List<StandarProduct>)Session["products"] ?? new List<StandarProduct>();
                    if (productList != null && productList.Any())
                        boking.Product = productList.Select(p => new Product
                        {
                            Name = p.Name,
                            Price = p.Price.ToString(CultureInfo.InvariantCulture),
                            Type = p.Text,
                            Quantity = p.Time.ToString()
                        }).ToList();

                    //var nbrItems = boking.NbrItems > 0 ? boking.NbrItems - 1 : 0;

                    //boking.DeliveryCost = GetPriceFrMonting(productList).ToString();
                    boking.DeliveryCost = GetPrices(boking.Zone, boking.WayOfDelivery, boking.Distance, boking.Pickup, false, boking.NbrItems, boking.NbrItemsPickup, boking.BookingDay, boking.Canceled).ToString(CultureInfo.InvariantCulture);//use unused cancel property as delivery. 
                    boking.Status = Status.New.ToString();

                    if (boking.Remarks == null)
                        boking.Remarks = "";

                    _db.BokingSet.Add(boking);

                    _db.SaveChanges();

                    if (!SendNotificationsToCustomer(boking))
                    {
                        ViewBag.Error = "Misslyckades att skicka avisering till kunden.";
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.Error = "Please check the inforamtions and try again. ";
                    ViewBag.Error += e.Message + ". " + e.InnerException?.Message;
                }
            }

            ViewBag.Delivery = boking.WayOfDelivery;

            return View(boking);
        }

        [HttpPost]
        public ActionResult AddProductbyId(int productId)
        {
            var product = ProductContainer.Products.FirstOrDefault(p => p.Id == productId);

            var productList = (List<StandarProduct>) Session["products"] ?? new List<StandarProduct>();
            productList.Add(product);
            Session["products"] = productList;

            return PartialView("~/Views/Bokings/ProductList.cshtml", productList);
        }
        [HttpPost]
        public ActionResult RemoveProductbyId(int productId)
        {
            var productList = (List<StandarProduct>)Session["products"] ?? new List<StandarProduct>();
            if (productId >= 0)
            {
                var product = productList.FirstOrDefault(p => p.Id == productId);

                productList.Remove(product);
            }
            Session["products"] = productList;

            return PartialView("ProductList", productList);
        }

        private bool SendNotificationsToCustomer(Boking boking, bool leverera = true)
        {
            //var name = boking.Customer.Name;
            var partOfDay = GetBookingPartOfTheDay(boking.PartOfTheDay, boking.BookingDay.DayOfWeek);
            
            var message = $"Leveranstyp: {LanguageDictionary.Translate[boking.WayOfDelivery]}\n" +
                          $" Datum: {boking.BookingDay.ToShortDateString()}\n" +
                          $"Tid: {partOfDay}\n" +
                          $"Adress: {boking.Customer.Address1}\n\n\n";

            if (boking.Type != BookingTypes.Return.ToString())
                message += $"{GetPriceZone(boking, leverera)}\n\n";

            if (boking.Type == BookingTypes.Monting.ToString())
                message += $"{GetPriceMonting()}\n\n";

            message += "Mvh Q Transport";

            if (boking.Type == BookingTypes.Return.ToString())
                message += $"\n\n" +
                            "För ytterligare frågor angående\n" +
                            "Leveransen vänligen ring:\n" +
                            "040-6555 000";

            var pdf = boking.Type == BookingTypes.Return.ToString()
                ? "LEVERANSVILLKOR REKLA Qtransport" 
                : boking.Type == BookingTypes.Monting.ToString() 
                ? "LEVERANSVILLKOR Qtransport 2"
                : "LEVERANSVILLKOR Qtransport";
            var status = Helpers.MailHandler.SendEmail(message, boking.Customer.Email, "Orderbekräftelse från XXXLutz", true, pdf);
            return string.IsNullOrEmpty(status);
            
            //message += "Detta sms kan du inte svara på.\nFör ytligare frågor kontakta oss på 040-191319";
            //string resultStr;
            //return Helpers.SmsHandler.SendSms(boking.Customer.Phone1, message, out resultStr) || status;

        }

        private string GetBookingPartOfTheDay(short bokingPartOfTheDay, DayOfWeek day)
        {

            if (day == DayOfWeek.Saturday)
                return "10.00 - 15.00";
            return bokingPartOfTheDay == 0
                ? "10.00 - 13.00"
                : (bokingPartOfTheDay == 1 ? "13.00 - 16.00" : "16.00 - 19.00");
        }

        private decimal GetPrices(string zone, string wayOfDelivey, decimal distance, bool pickup, bool xlutzPays, int nbrExtraItems, int nbrExtraItemsPickup, DateTime date, bool leverera = true)
        {
            var value = leverera ? Zones.PriceForDelivery(wayOfDelivey, zone, distance, nbrExtraItems, xlutzPays, date.DayOfWeek, date) : 0;

            if (pickup)
                value += Zones.PriceForPicups(wayOfDelivey, zone, distance, nbrExtraItemsPickup, xlutzPays, date);

            try
            {
                var productList = (List<StandarProduct>) Session["products"] ?? new List<StandarProduct>();
                if (productList != null && productList.Any())
                    value += GetPriceFrMonting(productList);
            }
            catch (Exception e)
            {
                ViewBag.Error = $"{e.Message}. {e.InnerException?.Message}";
            }

            return value;
        }

        private decimal GetPriceFrMonting(IEnumerable<StandarProduct> products)
        {
            return products.Sum(standarProduct => standarProduct.Price);
        }

        [HttpPost]
        public string GetPriceZone(Boking booking, bool leverera = true)
        {
            
            var user = AdminAuthenticationHelper.Current.GetAdminUserData().Role;
            if (user == Role.LogisticAdministrator.ToString())
                return GetPriceZoneReturn(booking, leverera);

            var priceDetails = new StringBuilder();
            try
            {
                //var nbrItems = booking.NbrItems 
                //    - (booking.Type == BookingTypes.Monting.ToString() 
                //    && booking.NbrItems > 0 ? 1 : 0);
                
                var price = Zones.PriceForDelivery(booking.WayOfDelivery, booking.Zone, booking.Distance,
                    booking.NbrItems, booking.PayBySupplier, booking.BookingDay.DayOfWeek, booking.BookingDay);

                if (booking.Type == BookingTypes.Monting.ToString() && !booking.Canceled)//use unused cancel property as delivery. 
                    price = 0;
                
                priceDetails.AppendLine("Pris för leverans:          " + price);
                //priceDetails.AppendLine("");
                if (booking.Type == BookingTypes.Monting.ToString())
                {
                    var productList = (List<StandarProduct>)Session["products"] ?? new List<StandarProduct>();
                    var priceMonting = GetPriceFrMonting(productList);
                    price += priceMonting;
                    priceDetails.AppendLine("Pris för montering:          " + priceMonting);
                }
               
                decimal pickup = 0;
                if (booking.Pickup)
                    priceDetails.AppendLine("Pris för bortforsling:     " + (pickup = Zones.PriceForPicups(booking.WayOfDelivery, booking.Zone, booking.Distance,booking.NbrItemsPickup, false, booking.BookingDay)));

                priceDetails.AppendLine("----------------------------------");
                priceDetails.AppendLine("Totalpris:                  " + (price + pickup));
            }
            catch (Exception e)
            {
                ViewBag.Error = $"{e.Message}. {e.InnerException?.Message}";
            }

            return priceDetails.ToString();
            //return JsonConvert.SerializeObject(priceDetails, Formatting.Indented);
        }

        [HttpPost]
        public string GetPriceZoneReturn(Boking booking, bool leverera)
        {
            var priceDetails = new StringBuilder();
            try
            {
                var price = leverera ? Zones.PriceForDelivery(booking.WayOfDelivery, booking.Zone, booking.Distance,
                    booking.NbrItems, booking.PayBySupplier, booking.BookingDay.DayOfWeek, booking.BookingDay) : 0;

                if (leverera)
                    priceDetails.AppendLine("Pris för hämtning:          " + price);

                decimal pickup = 0;
                if (booking.Pickup)
                    priceDetails.AppendLine("Pris för leverans:     " + (pickup = Zones.PriceForPicups(booking.WayOfDelivery, booking.Zone, booking.Distance, booking.NbrItemsPickup, true, booking.BookingDay)));

                priceDetails.AppendLine("----------------------------------");
                priceDetails.AppendLine("Totalpris:                  " + (price + pickup));
            }
            catch (Exception e)
            {
                ViewBag.Error = $"{e.Message}. {e.InnerException?.Message}";
            }

            return priceDetails.ToString();
        }

        [HttpPost]
        public string GetPriceMonting()
        {
            var productList = (List<StandarProduct>)Session["products"] ?? new List<StandarProduct>();
            var priceText = "Pris för montering:          " + GetPriceFrMonting(productList);
            return priceText;
        }

        // GET: Bokings/Edit/5
        [VotyAuthorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Boking boking = _db.BokingSet.Find(id);
            if (boking == null)
            {
                return HttpNotFound();
            }

            if (boking.Type == BookingTypes.Return.ToString())
                return View("EditReturn", boking);

            if (boking.Type == BookingTypes.Monting.ToString())
            {
                Session["products"] = new List<StandarProduct>(boking.Product.Select(p => new StandarProduct
                {
                    Name = p.Name,
                    Price = decimal.Parse(p.Price),
                    Text = p.Type,
                    Time = int.Parse(p.Quantity)
                }));
                return View("EditMonting", boking);
            }

            boking.Remarks = boking.Remarks ?? "";

            return View(boking);
        }

        // POST: Bokings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [VotyAuthorize]
        public ActionResult Edit(Boking boking)
        {
            if (!ModelState.IsValid) return View(boking);

            if (boking.BookingDay >= DateTime.Today)
            {

                var booking = _db.BokingSet.Find(boking.Id);
                if (booking == null)
                    return View();

                booking.NbrItems = boking.NbrItems;
                booking.Customer = boking.Customer;
                booking.DeliveryCost = GetPrices(boking.Zone, boking.WayOfDelivery, boking.Distance, boking.Pickup,
                    boking.PayBySupplier, boking.NbrItems, boking.NbrItemsPickup, boking.BookingDay).ToString(CultureInfo.InvariantCulture);
                booking.Distance = boking.Distance;
                booking.NbrItemsPickup = boking.NbrItemsPickup;
                booking.OrderAmount = boking.OrderAmount;
                booking.OrderNbr = boking.OrderNbr;
                booking.Pickup = boking.Pickup;
                booking.Remarks = boking.Remarks ?? "";
                booking.WayOfDelivery = boking.WayOfDelivery;
                booking.Zone = boking.Zone;
                booking.Type = boking.Type;


                //if (boking.Remarks == null)
                //    boking.Remarks = "";

                //db.BokingSet.Attach(boking);

                //db.Entry(boking).Property(p => p.Customer.PortCode).IsModified = true;
                //db.Entry(boking).State = EntityState.Modified;

                try
                {
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.Error = $"{e.Message}. {e.InnerException?.Message}";
                }
            }
            else
            {
                ViewBag.Error = "Nya datummet ska vara f.o.m idag.";
            }
            return View(boking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VotyAuthorize]
        public ActionResult EditReturn(Boking boking, bool leverera)
        {
            if (!ModelState.IsValid)
                return View(boking);

            if (boking.BookingDay >= DateTime.Today)
            {
                var booking = _db.BokingSet.Find(boking.Id);
                if(booking == null)
                {
                    ViewBag.Error = "Validation error.";
                    return View(boking);
                }

                booking.NbrItems = boking.NbrItems;
                booking.Customer = boking.Customer;
                booking.DeliveryCost = GetPrices(boking.Zone, boking.WayOfDelivery, boking.Distance, boking.Pickup,
                    boking.PayBySupplier, boking.NbrItems, boking.NbrItemsPickup, boking.BookingDay, leverera).ToString(CultureInfo.InvariantCulture);
                booking.Distance = boking.Distance;
                booking.NbrItemsPickup = boking.NbrItemsPickup;
                booking.OrderAmount = boking.OrderAmount;
                booking.OrderNbr = boking.OrderNbr;
                booking.Pickup = boking.Pickup;
                booking.Remarks = boking.Remarks ?? "";
                booking.WayOfDelivery = boking.WayOfDelivery;
                booking.Zone = boking.Zone;
                booking.Type = boking.Type;
                booking.Email = leverera;
                
                try
                {
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.Error = $"{e.Message}. {e.InnerException?.Message}";
                }
            }
            else
            {
                ViewBag.Error = "Nya datummet ska vara f.o.m idag.";
            }
            return View(boking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VotyAuthorize]
        public ActionResult EditMonting(Boking boking)
        {
            if (!ModelState.IsValid)
                return View(boking);

            if (boking.BookingDay >= DateTime.Today)
            {
                var booking = _db.BokingSet.Find(boking.Id);
                if (booking == null)
                    return View(boking);

                var cpt = booking.Product.Count;
                for (var i = cpt - 1; i >= 0; i--)
                {
                    var prod = _db.ProductSet.Find(booking.Product.ToList()[i].Id);
                    if (prod != null)
                        _db.ProductSet.Remove(prod);
                    _db.SaveChanges();
                }
                booking.Product.Clear();
                _db.SaveChanges();
                
                booking = _db.BokingSet.Find(boking.Id);
                if (booking == null)
                    return View(boking);

                booking.NbrItems = boking.NbrItems;
                booking.Customer = boking.Customer;
                booking.Distance = boking.Distance;
                booking.NbrItemsPickup = boking.NbrItemsPickup;
                booking.OrderAmount = boking.OrderAmount;
                booking.OrderNbr = boking.OrderNbr;
                booking.Pickup = boking.Pickup;
                booking.Remarks = boking.Remarks ?? "";
                booking.WayOfDelivery = boking.WayOfDelivery;
                booking.Zone = boking.Zone;
                booking.Type = boking.Type;
                booking.Canceled = boking.Canceled;

                var productList = (List<StandarProduct>)Session["products"] ?? new List<StandarProduct>();

                if (productList.Any())
                    booking.Product = productList.Select(p => new Product
                    {
                        Name = p.Name,
                        Price = p.Price.ToString(CultureInfo.InvariantCulture),
                        Type = p.Text,
                        Quantity = p.Time.ToString()
                    }).ToList();
                
                booking.DeliveryCost = GetPrices(boking.Zone, boking.WayOfDelivery, boking.Distance, boking.Pickup, false, boking.NbrItems, boking.NbrItemsPickup, boking.BookingDay, boking.Canceled).ToString(CultureInfo.InvariantCulture);//use unused cancel property as delivery. 

                _db.BokingSet.AddOrUpdate(booking);
                try
                {
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.Error = $"{e.Message}. {e.InnerException?.Message}";
                }
            }
            else
            {
                ViewBag.Error = "Nya datummet ska vara f.o.m idag.";
            }
            return View(boking);
        }

        // GET: Bokings/Delete/5
        [VotyAuthorize(Role.LogisticAdministrator, Role.Administrator, Role.qt)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var boking = _db.BokingSet.Find(id);
            if (boking == null)
            {
                return HttpNotFound();
            }
            return View(boking);
        }

        // POST: Bokings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [VotyAuthorize(Role.LogisticAdministrator, Role.Administrator, Role.qt)]
        public ActionResult DeleteConfirmed(int id)
        {
            var boking = _db.BokingSet.Find(id);

            if (boking == null || boking.BookingDay < DateTime.Today)
            {
                ViewBag.Error = "Får ej raderas. Bara framtida bokningar får raderas.";
                return View();
            }

            var cpt = boking.Product.Count;
            for (var i = cpt - 1; i >= 0; i--)
            {
                var prod = _db.ProductSet.Find(boking.Product.ToList()[i].Id);
                if (prod != null)
                    _db.ProductSet.Remove(prod);
                _db.SaveChanges();
            }
            boking.Product.Clear();
            _db.SaveChanges();

            _db.BokingSet.Remove(boking);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
