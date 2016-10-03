using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class EditContracts
    {
        public List<SP_SelectContractForEdit_Result> ReturnContractsForEdit(int ContractId=0)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_SelectContractForEdit_Result> Contract = (from contract in Entidad.SP_SelectContractForEdit(ContractId) select contract).ToList();
            return Contract;
        }

        public List<SP_SelectDraftContractForEdit_Result> ReturnDraftContractsForEdit(int ContractId = 0)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_SelectDraftContractForEdit_Result> Contract = (from contract in Entidad.SP_SelectDraftContractForEdit(ContractId) select contract).ToList();
            return Contract;
        }
    }
}