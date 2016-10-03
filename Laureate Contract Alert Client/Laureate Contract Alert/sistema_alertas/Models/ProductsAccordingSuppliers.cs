using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class ProductsAccordingSuppliers
    {
        public List<SP_selectProductsAccordingSuppliers_Result> ReturnProductsAccordingSuppliers(int ProviderManufacturing, int ProviderVendor)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectProductsAccordingSuppliers_Result> Products = (from products in Entidad.SP_selectProductsAccordingSuppliers(ProviderManufacturing, ProviderVendor) select products).ToList();
            return Products;
        }
    }
}