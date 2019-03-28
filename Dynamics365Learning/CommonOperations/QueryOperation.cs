using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using CommonHelper;
using CommonHelper.EntityNameCollection;
using System.ServiceModel;

namespace CommonOperations
{
    /// <summary>
    /// Operations using QueryExpression and related tech
    /// </summary>
    public class QueryOperation
    {
        public void Run()
        {
            try
            {
                //Get crm configuration from app.config
                ServerConfiguration config = new ServerConfiguration();
                config.GetServerConfiguration();

                //Init crm service
                var service = CrmServiceHelper.GetOrganizationServiceProxy(config);

                //Basic Query Qxpression
                RetrieveMultipleByQueryExpression(service);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Console.WriteLine("OrganizationServiceFault Exception: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("The end");
                Console.ReadLine();
            }
        }

        #region Retrieve Operations by QueryExpression
        /// <summary>
        /// Basic Use of Query Expression
        /// </summary>
        /// <param name="service"></param>
        public void RetrieveMultipleByQueryExpression(IOrganizationService service)
        {
            var query = new QueryExpression()
            {
                EntityName = EntityName.Account,
                ColumnSet = new ColumnSet(new string[] { "name" }),
                Distinct = false,
                Criteria = new FilterExpression()
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression("statecode", ConditionOperator.Equal,0),
                        new ConditionExpression("name", ConditionOperator.Like,"a%"),
                    }
                },
                Orders =
                {
                    new OrderExpression("name", OrderType.Descending)
                }
            };

            var result = service.RetrieveMultiple(query);

            //Output the result
            Console.WriteLine("{0} accounts has/have been retrieved-------------", result.Entities.Count);
            foreach (var entity in result.Entities)
                Console.WriteLine("Print account: Guid:{0}; AccountName: {1}", entity.Id, entity.GetAttributeValue<string>("name"));
        }
        #endregion
    }
}
