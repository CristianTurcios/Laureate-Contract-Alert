using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class ContractArea
    {
        public List<string> ReturnContractArea()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<string> ContractArea = (from contractarea in Entidad.SP_selectContractArea() select contractarea).ToList();
            return ContractArea;
        }
    }
}