using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class ContractAreaById
    {
        public List<SP_selectContractAreaById_Result> ReturnContractArea(int ContractAreaId)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectContractAreaById_Result> ContractArea = (from contractarea in Entidad.SP_selectContractAreaById(ContractAreaId) select contractarea).ToList();
            return ContractArea;
        }
    }
}