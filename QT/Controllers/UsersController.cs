using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using QT.Authentication;

namespace QT.Controllers
{
    public class UsersController : Controller
    {
        private QTransportModelContainer db = new QTransportModelContainer();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.UserSet.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Login(string username, string password)
        {
            var list = db.UserSet.Where(u => u.Username == username && u.Password == password && u.Active);
            if (!list.Any())
            {
                TempData["error"] = "Wrong username or password";
                return RedirectToAction("Index", "Home");
            }
            var userData = AdminAuthenticationHelper.GetUserData(list.First());
            AdminAuthenticationHelper.Current.Login(userData);
            if (userData.Role == Role.xlutz.ToString())
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            AdminAuthenticationHelper.Current.Logout();
            return RedirectToAction("Index", "Home");
        }
        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.UserSet.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Lastname,PersonalNbr,Username,EmployeeNbr,Email,Password,Role,Token")] User user)
        {
            if (!ModelState.IsValid)
                return View("Create");

            if (db.UserSet.Any(u => u.Username == user.Username || u.Email == user.Email))
            {
                ViewBag.Error = "Användarenamn eller epos redan registrerad.";
                return View("Create");
            }

            if (user.Token == "xxxlutz@3399" || user.Token == "qt@5252")
            {
                //user.Role = user.Token == "xxxlutz@3399" ? Role.xlutz.ToString() : Role.qt.ToString();

                user.Order = (short)(db.UserSet.Count(u => u.Role == user.Role) + 1);
                user.Active = true;
                db.UserSet.Add(user);
                try
                {
                    db.SaveChanges();
                    var userData = AdminAuthenticationHelper.GetUserData(user);
                    AdminAuthenticationHelper.Current.Login(userData);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message + "<br/>" + e.InnerException?.Message + "</br>";
                }
            }
            ViewBag.Error += "Unvalid Administrator password.";

            return View("Create");
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.UserSet.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Lastname,PersonalNbr,Username,EmployeeNbr,Email,Password,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }


        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string emailOrUsername)
        {
            var list = db.UserSet.Where(u => u.Username == emailOrUsername || u.Email == emailOrUsername);
            if (!list.Any())
            {
                ViewBag.Error = "Wrong username or password";
                return RedirectToAction("ResetPassword", "Users");
            }

            var user = list.First();
            user.Token = Guid.NewGuid().ToString();

            try
            {
                db.SaveChanges();
                //{Request?.Url?.Authority}{Url.Content("~")}";
                var link = new StringBuilder();
                link.Append(Request.Url?.AbsoluteUri.Replace(Request.Url.AbsolutePath, ""));
                link.Append("/resetpasswordverified/");
                link.Append(user.Id);
                link.Append("/");
                link.Append(user.Token);

                Helpers.MailHandler.SendEmail(link.ToString(), user.Email, "Lösenord återställning");
                ViewBag.Info = "Vi har skicakt ett eamil till: " + user.Email + " med återstllninglänk.";

                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                ViewBag.Error = "Misslyckad återställning. Kontakta administratör.\nMer: " + e.Message + ". " +
                                e.InnerException?.Message;
            }

            return ResetPassword();

        }

        public ActionResult ResetPasswordVerified(int id, string token)
        {
            var user = db.UserSet.First(u => u.Id == id && u.Token == token);
            return View(user);
        }

        [HttpPost]
        public ActionResult ResetPasswordVerified(int id, string password, string rePassword)
        {
            var user = db.UserSet.First(u => u.Id == id);
            if (password.Length < 4 || password != rePassword)
            {
                ViewBag.Error = "Lösenord är mindre än mindre än 4 tecken. Eller fälterna matchar inte. Prova igen.";
                return ResetPasswordVerified(user.Id, user.Token);
            }

            try
            {
                user.Password = password;
                user.Token = "";
                db.SaveChanges();
                
                var userData = AdminAuthenticationHelper.GetUserData(user);
                AdminAuthenticationHelper.Current.Login(userData);

                ViewBag.Message = "Nytt lösenord registrerat.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                ViewBag.Error = "Det gick inte att återställa lösenordet. Prova igen eller kontakta administratör.";
                return ResetPasswordVerified(user.Id, user.Token);
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.UserSet.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string password)
        {
            if (password == "xxxlutz@3399")
            {
                User user = db.UserSet.Find(id);
                db.UserSet.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Errors = "Fel AD lösenord";
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
