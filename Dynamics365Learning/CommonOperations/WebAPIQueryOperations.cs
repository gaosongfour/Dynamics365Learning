using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using CommonHelper;

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
                Console.WriteLine($"The current user Id is {result}");

            }
            catch (Exception ex)
            {
                Console.WriteLine("The request is failed : {0}", ex.Message);
            }
        }

        public async void QueryEntityList()
        {
           
            var crmWebAPIHelper = new CrmWebAPIHelper();
            var request = "accounts?$select=name&$top=3";

            try
            {
                var result = await crmWebAPIHelper.SendGetRequestAsync(request);
                var jsonResult = JObject.Parse(result);
                if (jsonResult.ContainsKey("value"))
                {
                    var entityCollection = jsonResult["value"] as JArray;
                    foreach (JObject entity in entityCollection)
                    {
                        if (entity.ContainsKey("accountid") && entity.ContainsKey("name"))
                        {
                            Console.WriteLine($"Account: Id: {entity["accountid"].ToString()}-Name: {entity["name"].ToString()}");
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
