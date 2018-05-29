using System.Web.Mvc;
using QT.Authentication;
using SMSApi.Api;

namespace QT.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Error = TempData["error"];
            if (AdminAuthenticationHelper.Current.IsAuthenticated())
            {
                return RedirectToAction("Index", "Bokings");
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public void TestSms()
        {
            const string to = "46734195147";

            try
            {
                var client = new Client("purchase@voty.se");
                //client.SetUsername("purchase@voty.se");
                client.SetPasswordHash("07a0fbb4e40877cb77074eff3494b958");

                var smsApi = new SMSFactory(client);

                var result = smsApi.ActionSend()
                    .SetText("Test")
                    .SetTo(to)
                    .SetSender("QTransport")
                    .Execute();

                var ids = new string[result.Count];

                for (int i = 0, l = 0; i < result.List.Count; i++)
                {
                    if (result.List[i].isError()) continue;
                    if (result.List[i].isFinal()) continue;
                    ids[l] = result.List[i].ID;
                    l++;
                }

                result =
                    smsApi.ActionGet()
                        .Ids(ids)
                        .Execute();

                foreach (var status in result.List)
                {
                    System.Console.WriteLine("ID: " + status.ID + " NUmber: " + status.Number + " Points:" + status.Points + " Status:" + status.Status + " IDx: " + status.IDx);
                }

                foreach (var t in result.List)
                {
                    if (t.isError())
                        continue;

                    var deleted =
                        smsApi.ActionDelete()
                            .Id(t.ID)
                            .Execute();
                    System.Console.WriteLine("Deleted: " + deleted.Count);
                }

            }
            catch (Exception e)
            {
                ViewBag.Error = $"{e.Message}. {e.InnerException?.Message}";
            }
        }
    }
}