using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class PurchaseOrderById
    {
        public List<SP_SelectOrdenComprasById_Result> ReturnPurchaseOrder(int OrdenCompraId)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_SelectOrdenComprasById_Result> PurchaseOrder = (from purchaseorder in Entidad.SP_SelectOrdenComprasById(OrdenCompraId) select purchaseorder).ToList();
            return PurchaseOrder;
        }
    }
}