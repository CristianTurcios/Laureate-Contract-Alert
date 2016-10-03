using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class ProductByProviderId
    {
        public List<SP_selectProductByProviderId_Result> ReturnProductByProviderId(int ProviderId)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectProductByProviderId_Result> Products = (from product in Entidad.SP_selectProductByProviderId(ProviderId) select product).ToList();
            return Products;
        }
    }
}