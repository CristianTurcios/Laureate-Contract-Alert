using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class ProviderVendor
    {
        public List<SP_selectProviderVender_Result> ReturnProviderVendor()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectProviderVender_Result> ProviderVendor = (from Prov_Vendor in Entidad.SP_selectProviderVender() select Prov_Vendor).ToList();
            return ProviderVendor;
        }
    }
}