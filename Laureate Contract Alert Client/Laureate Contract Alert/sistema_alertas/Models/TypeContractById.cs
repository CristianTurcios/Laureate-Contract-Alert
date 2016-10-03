using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class TypeContractById
    {
        public List<SP_selectTypeContractById_Result> ReturnTypeContract(int typeContractId)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectTypeContractById_Result> typeContract = (from typecontract in Entidad.SP_selectTypeContractById(typeContractId) select typecontract).ToList();
            return typeContract;
        }
    }
}