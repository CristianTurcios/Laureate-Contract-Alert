using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class PurchaseOrder
    {
        public List<SP_selectPurchaseOrder_Result> ReturnPurchaseOrder()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectPurchaseOrder_Result> PurchaseOrder = (from purchaseorder in Entidad.SP_selectPurchaseOrder() select purchaseorder).ToList();
            return PurchaseOrder;
        }
    }
}