using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class TypeContract
    {
        public List<SP_selectTypeContract_Result> ReturnTypeContract()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectTypeContract_Result> typeContract = (from typecontract in Entidad.SP_selectTypeContract() select typecontract).ToList();
            return typeContract;
        }
    }
}