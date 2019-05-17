using Newtonsoft.Json.Linq;

namespace CommonHelper
{
    public class JsonHelper
    {
        public static void PrintJsonResult(string jsonString)
        {
            var jsonResult = JObject.Parse(jsonString);
            if(jsonResult.ContainsKey("value"))
            {
                var entityCollection = jsonResult["value"] as JArray;
              
                foreach(JObject entity in entityCollection)
                {
                    foreach(var prop in entity.Properties())
                    {

                    }

                }
            }
        }
    }
}
