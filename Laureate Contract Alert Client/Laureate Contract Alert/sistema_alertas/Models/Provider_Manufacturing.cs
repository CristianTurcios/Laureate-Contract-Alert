using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class Provider_Manufacturing
    {
        public List<SP_selectProviderManufacturing_Result> ReturnProviderManufacturing()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectProviderManufacturing_Result> Provider_Manufacturing = (from Prov_Manuf in Entidad.SP_selectProviderManufacturing() select Prov_Manuf).ToList();
            return Provider_Manufacturing;
        }
    }
}