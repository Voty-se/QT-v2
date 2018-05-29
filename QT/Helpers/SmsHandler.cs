using SMSApi.Api;
using System.Linq;

namespace QT.Helpers
{
    public static class SmsHandler
    {
        public static bool SendSms(string to, string message, out string resultStr)
        {
            resultStr = "Kundens telefonenummer saknas.";
            if (string.IsNullOrEmpty(to))
                return false;

            try
            {
                to = "46" + to.TrimStart('0');

                var client = new Client("purchase@voty.se");
                client.SetPasswordHash("07a0fbb4e40877cb77074eff3494b958");

                var smsApi = new SMSFactory(client);

                var result = smsApi.ActionSend()
                    .SetText(message)
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

                result = smsApi.ActionGet()
                    .Ids(ids)
                    .Execute();

                resultStr = result.List.Aggregate(resultStr,
                    (current, status) =>
                        current +
                        ("ID: " + status.ID + " NUmber: " + status.Number + " Points:" + status.Points + " Status:" +
                         status.Status + " IDx: " + status.IDx + ". "));

                return true;
            }
            catch (ActionException e)
            {
                resultStr = $"{e.Message}. {e.InnerException?.Message}";
            }
            catch (ClientException e)
            {
                resultStr = $"{e.Message}. {e.InnerException?.Message}";
            }
            catch (HostException e)
            {
                resultStr = $"{e.Message}. {e.InnerException?.Message}";
            }
            catch (ProxyException e)
            {
                resultStr = $"{e.Message}. {e.InnerException?.Message}";
            }

            return false;
        }
    }
}