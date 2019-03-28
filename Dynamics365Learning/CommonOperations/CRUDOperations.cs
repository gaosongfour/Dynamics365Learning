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
    /// Include Basic CRUD operations using Organization Service
    /// </summary>
    public class CRUDOperations
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

                //Call who am I request
                SendWhoAmIRequest(service);

                //Create a new account
                var accountEntityToCreate = PrepareAccountEntityToCreate();
                var accountId = CreateEntity(service, accountEntityToCreate);

                //Retrieve
                var entityRetrieved = RetrieveEntity(service, accountId, EntityName.Account, null);

                //Update
                UpdateEntity(service, accountId, EntityName.Account);

                //Delete
                DeleteEntity(service, accountId, EntityName.Account);

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

        #region WhoAmI Request
        public Guid SendWhoAmIRequest(OrganizationServiceProxy service)
        {
            WhoAmIRequest request = new WhoAmIRequest();
            var response = (WhoAmIResponse)service.Execute(request);
            Console.WriteLine("WhoAmIRequest OK, CurrentUserId: " + response.UserId);
            return response.UserId;
        }
        #endregion

        #region Basic CRUD operions for an single entity
        public Guid CreateEntity(OrganizationServiceProxy serivce, Entity entity)
        {
            var recordId = serivce.Create(entity);
            Console.WriteLine("A record of {0} was created sucessfully, Guid: {1}", entity.LogicalName, recordId);
            return recordId;
        }

        public Entity RetrieveEntity(OrganizationServiceProxy service, Guid entityId, string entityName, string[] colsToRetrieve)
        {
            ColumnSet cols;
            if (colsToRetrieve == null || colsToRetrieve.Length == 0)
                cols = new ColumnSet(true); //Retrieve All
            else
                cols = new ColumnSet(colsToRetrieve);

            var entity = service.Retrieve(entityName, entityId, cols);
            Console.WriteLine("Retrieved record {0}, Guid: {1}", entityName, entityId);
            return entity;
        }

        public void UpdateEntity(OrganizationServiceProxy service, Guid entityId, string entityName)
        {
            Entity entity = new Entity(entityName, entityId);
            //define the attribute to update, if no attributes defined, the "modifiedon" will be updated
            entity["numberofemployees"] = 114;

            service.Update(entity);
            Console.WriteLine("The record of {0} was updated, Guid:{1}", entityName, entityId);
        }

        public void DeleteEntity(OrganizationServiceProxy service, Guid entityId, string entityName)
        {
            service.Delete(entityName, entityId);
            Console.WriteLine("The record of {0} was deleted, Guid:{1}", entityName, entityId);
        }
        #endregion

        #region Prepare Entity Methods
        private Entity PrepareAccountEntityToCreate()
        {
            Entity entity = new Entity(EntityName.Account);

            #region Define the attribute for new entity
            //Unique Identifier=> Guid of each record, we let the system to create it 

            //String Single line of test
            entity["accountnumber"] = DateTime.Now.ToShortTimeString();
            entity["name"] = "Demo Account";

            //String Multiple line of test
            entity["description"] = "Description,Multiple line of test, blablabla";

            //Date and Time
            entity["lastonholdtime"] = DateTime.Now;

            //OptionSetValue/PickList 
            entity["accountcategorycode"] = new OptionSetValue(1); //Preferred Customer

            //MultiSelect OptionSet 

            //Two options
            entity["creditonhold"] = false;

            //Currency
            entity["revenue"] = new Money(13000);

            //Decimal Number, ex. exchangerate


            //Whole Number  Int
            entity["numberofemployees"] = 276;

            //Floating Point Number
            entity["address1_latitude"] = 33.56;

            //StateCode

            //StatusCode

            //Image

            //Lookup
            //entity[""] = new EntityReference("", new Guid(""));

            //Customer=>Account or Contact

            //Owner=> User or Team

            //Party List=> Used in Phone and Appointment entity

            //Regarding => ref to multi entities

            #endregion

            return entity;
        }
        #endregion
    }
}
