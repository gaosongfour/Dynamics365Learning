using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CommonOperations
{
    /// <summary>
    /// Include Basic CRUD operations using Organization Service
    /// </summary>
    public class CRUDOperations
    {
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
    }
}
