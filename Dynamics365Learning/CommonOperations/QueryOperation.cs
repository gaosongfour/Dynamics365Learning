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

                //Basic Query Expression
                //RetrieveMultipleByQueryExpression(service);

                //Query Expression with LinkEntity
                //RetrieveMultipleByQueryExpressionWithLinkEntity(service);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Console.WriteLine("OrganizationServiceFault Exception: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
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

        /// <summary>
        /// Retrieve Multiple with linkEntity
        /// </summary>
        /// <param name="service"></param>
        public void RetrieveMultipleByQueryExpressionWithLinkEntity(IOrganizationService service)
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
                        new ConditionExpression("statecode", ConditionOperator.Equal,0)
                    }
                },
                //Link entity =>Account.PrimaryContactid=Contact.ContactId
                LinkEntities =
                {
                    new LinkEntity
                    {
                         EntityAlias="primarycontact",
                         Columns =new ColumnSet("fullname"),
                         JoinOperator= JoinOperator.Inner,
                         LinkFromEntityName=EntityName.Account,
                         LinkFromAttributeName="primarycontactid",
                         LinkToEntityName=EntityName.Contact,
                         LinkToAttributeName="contactid",
                         LinkCriteria=
                         {
                            Conditions={
                                new ConditionExpression("fullname", ConditionOperator.Like,"%Lyon%")
                            }
                         }
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
            {
                var accountName = entity.GetAttributeValue<string>("name");
                var contactFullname = string.Empty;
                if (entity.Contains("primarycontact.fullname"))
                    contactFullname = entity.GetAttributeValue<AliasedValue>("primarycontact.fullname").Value.ToString();
                Console.WriteLine("Print account: Guid:{0}; AccountName: {1}; PrimaryContact: {2}", entity.Id, accountName, contactFullname);
            }
        }
        #endregion


    }
}
