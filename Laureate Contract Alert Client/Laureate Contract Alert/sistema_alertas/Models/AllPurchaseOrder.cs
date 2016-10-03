using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllPurchaseOrder
    {
        public List<SP_selectAllPurchaseOrder_Result> ReturnAllPurchaseOrder()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectAllPurchaseOrder_Result> AllPurchaseOrder = (from purchaseOrder in Entidad.SP_selectAllPurchaseOrder() select purchaseOrder).ToList();
            return AllPurchaseOrder;
        }
    }
}