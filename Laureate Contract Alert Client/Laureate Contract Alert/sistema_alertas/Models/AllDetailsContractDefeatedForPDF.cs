using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sistema_alertas.Models
{
    public class AllDetailsContractDefeatedForPDF
    {
        public List<defeatedContractForReportPDF_Result> ReturnAllDetailsContract()
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<defeatedContractForReportPDF_Result> AllDetailsContract = (from ContractDetails in Entidad.defeatedContractForReportPDF() select ContractDetails).ToList();
            return AllDetailsContract;
        }

        public List<defeatedContractByAreaForReportPDF_Result> ReturnAllDetailsContractByArea(int Area)
        {
            sistema_alertasEntities Entidad = new sistema_alertasEntities();

            List<defeatedContractByAreaForReportPDF_Result> AllDetailsContract = (from ContractDetails in Entidad.defeatedContractByAreaForReportPDF(Area) select ContractDetails).ToList();
            return AllDetailsContract;
        }
    }
}