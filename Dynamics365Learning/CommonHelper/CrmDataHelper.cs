using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace CommonHelper
{
    public class CrmDataHelper
    {
        #region CRM Data Service
        /// <summary>
        /// Convert entity attribute to string format
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static string GetEntityAttributeStringValue(object attributeValue)
        {
            var dataType = attributeValue.GetType();
            //String
            if (dataType == typeof(string))
                return attributeValue.ToString();

            //OptionSetValue
            if (dataType == typeof(OptionSetValue))
                return ((OptionSetValue)attributeValue).Value.ToString();

            //Boolean
            if (dataType == typeof(bool))
                return ((bool)attributeValue).ToString();

            //EntityReference
            if (dataType == typeof(EntityReference))
            {
                var entityRef = ((EntityReference)attributeValue);
                return string.Format("EntityName: {0}; EntityId: {1}", entityRef.LogicalName, entityRef.Id.ToString());
            }

            //Int
            if (dataType == typeof(int))
                return ((int)attributeValue).ToString();

            //Money
            if (dataType == typeof(Money))
                return ((Money)attributeValue).Value.ToString();

            //Decimal
            if (dataType == typeof(decimal))
                return ((decimal)attributeValue).ToString();

            //Double
            if (dataType == typeof(double))
                return ((double)attributeValue).ToString();

            //Float
            if (dataType == typeof(float))
                return ((float)attributeValue).ToString();

            //DateTime=> UTC time
            if (dataType == typeof(DateTime))
            {
                var utc = ((DateTime)attributeValue);
                var local = utc.ToLocalTime();
                return string.Format("UTC:{0}; Local: {1}", utc, local);
            }

            //Guid
            if (dataType == typeof(Guid))
                return ((Guid)attributeValue).ToString();

            //Alaised Value
            if (dataType == typeof(AliasedValue))
            {
                var aliasedValue = ((AliasedValue)attributeValue).Value;
                return GetEntityAttributeStringValue(aliasedValue);
            }

            throw new Exception("Unknown DataType: " + dataType.ToString());
        }
        #endregion

        #region Print Service
        /// <summary>
        /// Print entity with all attributes in string format
        /// </summary>
        /// <param name="entity"></param>
        public static void PrintEntity(Entity entity)
        {
            Console.WriteLine("Print Entity Start **************************************************");
            Console.WriteLine("Entity Name: {0}", entity.LogicalName);
            Console.WriteLine("Entity Guid: {0}", entity.Id);
            Console.WriteLine("Entity RowVersion: {0}", entity.RowVersion);
            Console.WriteLine("Entity Attributes==============");

            foreach (var attribute in entity.Attributes)
            {
                var key = attribute.Key;
                try
                {
                    Console.WriteLine("{0}:{1}; {2}", key, attribute.Value.GetType().ToString(), GetEntityAttributeStringValue(attribute.Value));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}: Error Getting Value: {1}", key, ex.Message);
                }
            }
        }
        #endregion
    }
}
