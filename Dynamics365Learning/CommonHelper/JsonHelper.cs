using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace CommonHelper
{
    public class JsonHelper
    {
        public static string PrintJsonResult(JObject entity, string[] cols = null)
        {
            var printMessage = new StringBuilder();
            foreach (var prop in entity.Properties())
            {
                if (cols != null && Array.IndexOf(cols, prop.Name) == -1)
                    continue;
                printMessage.Append($"{prop.Name}:{prop.Value.ToString()};");
            }
            return printMessage.ToString();
        }
    }
}
