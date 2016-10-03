using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllContractArea
    {
        public List<SP_selectAllContractArea_Result> ReturnAllContractArea()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<SP_selectAllContractArea_Result> AllContractArea = (from contract_Area in Entidad.SP_selectAllContractArea() select contract_Area).ToList();
            return AllContractArea;
        }
    }
}