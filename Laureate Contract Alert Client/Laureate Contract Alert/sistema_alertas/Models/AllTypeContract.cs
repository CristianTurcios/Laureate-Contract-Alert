using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllTypeContract
    {
        public List<SP_selectAllTypeContract_Result> ReturnAllTypeContract()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectAllTypeContract_Result> AlltypeContract = (from typeContract in Entidad.SP_selectAllTypeContract() select typeContract).ToList();
            return AlltypeContract;
        }
    }
}