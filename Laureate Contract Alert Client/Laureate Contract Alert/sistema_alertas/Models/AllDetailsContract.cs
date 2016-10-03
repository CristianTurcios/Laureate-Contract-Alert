using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllDetailsContract
    {
        public List<SP_selectAllDetailsContract_Result> ReturnAllDetailsContract(int ContractId)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectAllDetailsContract_Result> AllDetailsContract = (from ContractDetails in Entidad.SP_selectAllDetailsContract(ContractId) select ContractDetails).ToList();
            return AllDetailsContract;
        }
    }
}