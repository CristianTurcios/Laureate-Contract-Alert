using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class BusinessUnitById
    {
        public List<SP_SelectUnidadNegociosById_Result> ReturnBusinessUnit(int UnidadNegocioId)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_SelectUnidadNegociosById_Result> BusinessUnit = (from bussinesUnit in Entidad.SP_SelectUnidadNegociosById(UnidadNegocioId) select bussinesUnit).ToList();
            return BusinessUnit;
        }
    }
}