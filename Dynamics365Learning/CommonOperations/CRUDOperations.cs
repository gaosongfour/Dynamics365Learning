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
        public Guid CreateEntity(OrganizationServiceProxy serivce)
        {
            Entity entity = new Entity("account");

            return serivce.Create(entity);
        }

        public Entity RetrieveEntity(OrganizationServiceProxy service, Guid entityId, string entityName, string[] colsToRetrieve)
        {
            ColumnSet cols = new ColumnSet();

            return service.Retrieve(entityName, entityId, cols);
        }

        public void UpdateEntity(OrganizationServiceProxy service, Guid entityId)
        {

        }

        public void DeleteEntity(OrganizationServiceProxy service)
        {

        }


        #endregion
    }
}
