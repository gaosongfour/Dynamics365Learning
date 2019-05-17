using CommonHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace CommonOperations
{
    public class WebAPIQueryOperations
    {
        public void Run()
        {
            SendWhoAmIRequest();
            QueryEntityList();
            QuerySingleEntity();

            Console.ReadLine();
        }

        public async void SendWhoAmIRequest()
        {

            var crmWebAPIHelper = new CrmWebAPIHelper();
            var request = "WhoAmI";

            try
            {
                var result = await crmWebAPIHelper.SendGetRequestAsync(request);

                //read the response body and parse it
                var body = JObject.Parse(result);
                var userId = (Guid)body["UserId"];
                Console.WriteLine($"The current user Id is {userId.ToString()}");

            }
            catch (Exception ex)
            {
                Console.WriteLine("The request is failed : {0}", ex.Message);
            }
        }

        public async void QueryEntityList()
        {

            var crmWebAPIHelper = new CrmWebAPIHelper();
            var request = new StringBuilder("accounts?");
            request.AppendLine("$select=name,revenue");
            request.AppendLine("&$orderby=name desc, revenue asc");
            request.AppendLine("&$top=3");
            request.AppendLine("&$filter=contains(name,'sample') and revenue gt 5000");

            string[] cols = { "accountid", "name", "revenue" };

            try
            {
                var result = await crmWebAPIHelper.SendGetRequestAsync(request.ToString());
                var jsonResult = JObject.Parse(result);
                if (jsonResult.ContainsKey("value"))
                {
                    var entityCollection = jsonResult["value"] as JArray;
                    foreach (JObject entity in entityCollection)
                    {
                        var printMessage = JsonHelper.PrintJsonResult(entity, cols);
                        Console.WriteLine(printMessage);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public async void QuerySingleEntity()
        {
            var crmWebAPIHelper = new CrmWebAPIHelper();

            //string[] cols = { "accountnumber", "name", "customersizecode", "createdon", "" };
            var entityid = "565F2550-9C4B-E911-9664-00155DDF3D03";
            var request = new StringBuilder($"accounts({entityid})?");
            request.Append($"$select=accountnumber,name,customersizecode,createdon");


            try
            {
                var result = await crmWebAPIHelper.SendGetRequestAsync(request.ToString());
                var jsonResult = JObject.Parse(result);
                var printMessage = JsonHelper.PrintJsonResult(jsonResult);
                Console.WriteLine(printMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}; InnerEx: {ex.InnerException?.Message}");
            }
        }
    }
}
