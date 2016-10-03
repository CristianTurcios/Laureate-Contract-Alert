using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class ProductById
    {
        public List<SP_selectProductById_Result> ReturnProduct(int ProductId)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectProductById_Result> Product = (from product in Entidad.SP_selectProductById(ProductId) select product).ToList();
            return Product;
        }

        public List<SP_selectProductByName_Result> ReturnProductByName(string ProductName)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectProductByName_Result> Product = (from product in Entidad.SP_selectProductByName(ProductName) select product).ToList();
            return Product;
        }


    }
}