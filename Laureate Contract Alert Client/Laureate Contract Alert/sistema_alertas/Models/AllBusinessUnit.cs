using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllBusinessUnit
    {
        public List<SP_selectAllBusinessunit_Result> ReturnAllBusinessUnit()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectAllBusinessunit_Result> AllBusinessUnit = (from businessUnit in Entidad.SP_selectAllBusinessunit() select businessUnit).ToList();
            return AllBusinessUnit;
        }
    }
}