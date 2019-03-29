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
                using (var service = CrmServiceHelper.GetOrganizationServiceProxy(config))
                {

                    //Basic Query Expression
                    //RetrieveMultipleByQueryExpression(service);

                    //Query Expression with LinkEntity
                    //RetrieveMultipleByQueryExpressionWithLinkEntity(service);

                    // QueryExpression Paging with cookie
                    //RetrieveMultipleWithPaging(service);

                    //Retrieve Contacts with linq
                    //RetrieveMultipleWithLinq(service);
                }
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

        /// <summary>
        /// QueryExpression Paging with cookie, As the total limit of RetriveMultiple is 5000, so consideration
        /// of using Paging in queryexpression is need in some senarios
        /// </summary>
        /// <param name="service"></param>
        public void RetrieveMultipleWithPaging(IOrganizationService service)
        {
            //The number of records per per page to retrive
            int queryCount = 4;

            //Initialize the page number
            int pageNumber = 1;

            //Initialize the number of records
            int recordCount = 0;

            //Define the queryexpression
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
                Orders =
                {
                    new OrderExpression("name", OrderType.Ascending)
                },
                //Define the Paging
                PageInfo = new PagingInfo()
                {
                    Count = queryCount,
                    PageNumber = pageNumber,
                    PagingCookie = null
                }
            };

            //Retrieve all accouts page by page
            Console.WriteLine("RetrieveMultipleWithPaging-----------------------");
            while (true)
            {
                var result = service.RetrieveMultiple(query);
                if (result.Entities != null)
                {
                    //Print the page info
                    Console.WriteLine("Page Number: {0}", query.PageInfo.PageNumber);

                    //Print account
                    foreach (var entity in result.Entities)
                        Console.WriteLine("Print account {0}#: Guid:{1}; AccountName: {2}",
                           ++recordCount, entity.Id, entity.GetAttributeValue<string>("name"));
                }

                if (result.MoreRecords)
                {
                    //Increase pageNumber
                    query.PageInfo.PageNumber++;

                    //Set the cookie
                    query.PageInfo.PagingCookie = result.PagingCookie;
                }
                else
                {
                    //exit the while loop
                    break;
                }
            }
        }
        #endregion

        #region QueryWithLinq
        /// <summary>
        /// Retrieve Contact with Linq
        /// </summary>
        /// <param name="service"></param>
        public void RetrieveMultipleWithLinq(IOrganizationService service)
        {
            OrganizationServiceContext serviceContext = new OrganizationServiceContext(service);
            var query = from c in serviceContext.CreateQuery(EntityName.Contact)
                        join a in serviceContext.CreateQuery(EntityName.Account)
                        on c["parentcustomerid"] equals a["accountid"]
                        select new
                        {
                            firstName = c.GetAttributeValue<string>("firstname"),
                            lastName = c.GetAttributeValue<string>("lastname"),
                            parentAccountName = a.GetAttributeValue<string>("name")
                        };

            foreach (var c in query)
            {
                Console.WriteLine("Retrieved Contact {0} {1} , Parent Account: {2}", c.firstName, c.lastName, c.parentAccountName);
            }
        }
        #endregion


    }
}
