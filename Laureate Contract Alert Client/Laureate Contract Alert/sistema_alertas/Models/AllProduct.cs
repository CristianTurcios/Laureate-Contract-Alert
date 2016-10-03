using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllProduct
    {
        public List<SP_selectAllProducts_Result> ReturnAllProduct()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectAllProducts_Result> AllProduct = (from product in Entidad.SP_selectAllProducts() select product).ToList();
            return AllProduct;
        }
    }
}