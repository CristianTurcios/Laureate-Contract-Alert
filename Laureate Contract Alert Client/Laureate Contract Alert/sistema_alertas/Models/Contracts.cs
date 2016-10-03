using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class Contracts
    {
        public List<SP_selectContract_Result> ReturnContracts()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectContract_Result> Contract = (from contract in Entidad.SP_selectContract() select contract).ToList();
            return Contract;
        }

        public List<SP_selectDraftContract_Result> ReturnDraftContracts()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectDraftContract_Result> Contract = (from contract in Entidad.SP_selectDraftContract() select contract).ToList();
            return Contract;
        }

        public List<SP_selectContractByArea_Result> ReturnContractsByArea(int CodigoArea)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectContractByArea_Result> Contract = (from contract in Entidad.SP_selectContractByArea(CodigoArea) select contract).ToList();
            return Contract;
        }

        public List<SP_selectDraftContractByArea_Result> ReturnDraftContractsByArea(int CodigoArea)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectDraftContractByArea_Result> Contract = (from contract in Entidad.SP_selectDraftContractByArea(CodigoArea) select contract).ToList();
            return Contract;
        }
    }
}