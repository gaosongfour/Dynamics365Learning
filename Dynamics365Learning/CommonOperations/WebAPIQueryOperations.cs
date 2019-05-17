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

            try
            {
                var result = await crmWebAPIHelper.SendGetRequestAsync(request.ToString());
                var jsonResult = JObject.Parse(result);
                if (jsonResult.ContainsKey("value"))
                {
                    var entityCollection = jsonResult["value"] as JArray;
                    foreach (JObject entity in entityCollection)
                    {
                        if (entity.ContainsKey("accountid") && entity.ContainsKey("name"))
                        {
                            Console.WriteLine($"Account Name: {entity["name"].ToString()} Revenue: {entity["revenue"].ToString()}");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
