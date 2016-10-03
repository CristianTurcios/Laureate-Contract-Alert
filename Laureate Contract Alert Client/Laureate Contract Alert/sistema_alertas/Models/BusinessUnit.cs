using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class BusinessUnit
    {
        public List<string> ReturnBusinessUnit()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<string> BusinessUnit = (from bussinesUnit in Entidad.SP_selectBusinessUnit() select bussinesUnit).ToList();
            return BusinessUnit;
        }
    }
}